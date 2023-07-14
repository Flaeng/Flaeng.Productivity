using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis.Text;

namespace Flaeng.Productivity.DependencyInjection;

[Generator(LanguageNames.CSharp)]
public sealed class ConstructorGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE_NAMESPACE = "Flaeng";
    const string ATTRIBUTE_NAME = "InjectAttribute";
    readonly HashSet<string> files = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateDependencies);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider<ConstructorData>(
                static (node, _) => node is ClassDeclarationSyntax { Members.Count: > 0 } cds
                    && cds.Modifiers.Any(SyntaxKind.PartialKeyword)
                    && cds.Modifiers.Any(SyntaxKind.StaticKeyword) == false,
                Transform
            )
            .Where(static x => x.Class != null)
            .WithComparer(ConstructorDataEqualityComparer.Instance);

        context.RegisterSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, ConstructorData data)
    {
        var cls = data.Class;
        if (cls is null || data.ClassSymbol is null)
            return;

        SourceBuilder sourceBuilder = new();

        var memberList = data.Members
            .Select(GetTypeNameAndMemberName)
            .ToImmutableArray();

        if (memberList.Length == 0)
            return;

        // Add using statements
        AddUsingStatements(cls, sourceBuilder);

        // Add namespace
        bool isInNamespace = AddNamespace(cls, sourceBuilder);

        // Add wrapper-classes
        foreach (var wrapper in data.WrapperClasses)
        {
            sourceBuilder.StartClass(new ClassOptions(wrapper.Name)
            {
                Visibility = wrapper.Visibility,
                Partial = true
            });
        }

        // Write new source code
        createClassAndConstructor(cls, data.ClassSymbol, sourceBuilder, memberList);

        // End wrapper-classes
        for (int i = 0; i < data.WrapperClasses.Length; i++)
            sourceBuilder.AppendEndTag();

        // End namespace
        if (isInNamespace)
            sourceBuilder.EndNamespace();

        var filename = Helpers.GenerateFilename(cls, false);
        if (files.Contains(filename))
            return;

        files.Add(filename);
        context.AddSource($"{filename}.g.cs", sourceBuilder.ToString());
    }

    private static void createClassAndConstructor(ClassDeclarationSyntax cls, INamedTypeSymbol clsSymbol, SourceBuilder sourceBuilder, ImmutableArray<TypeAndName> memberList)
    {
        var childTokens = cls.ChildTokens();
        var className = childTokens.First(x => x.IsKind(SyntaxKind.IdentifierToken));
        var visibility = TypeVisiblityHelper.GetFromTokens(childTokens);
        var classBuilder = sourceBuilder.StartClass(new ClassOptions(className.Text)
        {
            Visibility = visibility,
            Partial = true
        });

        var parameters = memberList.Select(x => $"{x.TypeName} {x.MemberName}");

        sourceBuilder.AddGeneratedCodeAttribute();
        classBuilder.StartConstructor(new ConstructorOptions(className.Text, false, new List<string>())
        {
            Visibility = MemberVisibility.Public,
            Parameters = new List<string>(parameters)
        });
        foreach (var member in memberList)
            sourceBuilder.AddLineOfCode($"this.{member.MemberName} = {member.MemberName};");
        classBuilder.EndConstructor();

        classBuilder.EndClass();
    }

    private static TypeAndName GetTypeNameAndMemberName(ISymbol member)
    {
        if (member is IFieldSymbol field)
        {
            var typeName = field.Type.ContainingNamespace.ToString() == "<global namespace>"
                ? field.Type.ToString()
                : $"global::{field.Type}";
            return new TypeAndName
            {
                TypeName = typeName,
                MemberName = field.Name
            };
        }
        if (member is IPropertySymbol prop)
        {
            var typeName = prop.Type.ContainingNamespace.ToString() == "<global namespace>"
                ? prop.Type.ToString()
                : $"global::{prop.Type}";
            return new TypeAndName
            {
                TypeName = typeName,
                MemberName = prop.Name
            };
        }
        throw new Exception("Unsupported symbol in parameterlist");
    }

    private static TypeAndName GetTypeNameAndMemberName(MemberDeclarationSyntax member)
    {
        var varDeclaration = member.ChildNodes()
            .OfType<VariableDeclarationSyntax>()
            .FirstOrDefault();
        if (varDeclaration != null)
            return GetTypeNameAndMemberNameFromVarDeclaration(varDeclaration);

        var genericNameSyntax = member.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();
        if (genericNameSyntax != null)
            return GetTypeNameAndMemberNameWithGenericName(member, genericNameSyntax);

        var tokens = member.DescendantTokens().Reverse();
        var memberName = tokens
            .Where(x => x.IsKind(SyntaxKind.IdentifierToken))
            .First();

        var typeName = tokens
            .SkipWhile(x => x != memberName)
            .Skip(1)
            .First();
        return new TypeAndName
        {
            TypeName = typeName.ToString(),
            MemberName = memberName.ToString()
        };
    }

    private static TypeAndName GetTypeNameAndMemberNameWithGenericName(MemberDeclarationSyntax member, GenericNameSyntax genericNameSyntax)
    {
        var memberName = member.DescendantTokens()
                        .Where(x => x.IsKind(SyntaxKind.IdentifierToken))
                        .Where(x => x.Parent is PropertyDeclarationSyntax)
                        .Single().ToString();
        return new TypeAndName
        {
            TypeName = genericNameSyntax.ToString(),
            MemberName = memberName
        };
    }

    private static TypeAndName GetTypeNameAndMemberNameFromVarDeclaration(VariableDeclarationSyntax varDeclaration)
    {
        var tokens = varDeclaration.DescendantTokens().ToArray();

        var typeName = String.Join("", tokens.Take(tokens.Length - 1)
            .Select(x => x.ToString()))
            .Replace(",", ", ");

        return new TypeAndName
        {
            TypeName = typeName,
            MemberName = tokens.Last().ToString()
        };
    }

    private static void AddUsingStatements(ClassDeclarationSyntax cls, SourceBuilder sourceBuilder)
    {
        var childNodes = cls.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
        sourceBuilder.AddUsingStatement(childNodes.Select(x => x.Name.ToString()));
    }

    private static bool AddNamespace(ClassDeclarationSyntax cls, SourceBuilder sourceBuilder)
    {
        var namespaceNode = cls.Parent?.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        var isInNamespace = namespaceNode != null;
        if (namespaceNode != null)
            sourceBuilder.StartNamespace(namespaceNode.Name.ToString());
        return isInNamespace;
    }

    private static void GenerateDependencies(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("InjectAttribute.g.cs", SourceText.From(@$"// <auto-generated/>
#nullable enable

namespace {ATTRIBUTE_NAMESPACE}
{{
    {Constants.GeneratedCodeAttribute}
    [global::System.AttributeUsageAttribute(
        global::System.AttributeTargets.Property | global::System.AttributeTargets.Field, 
        AllowMultiple = false, 
        Inherited = false)]
    internal sealed class {ATTRIBUTE_NAME} : global::System.Attribute
    {{
        public {ATTRIBUTE_NAME}()
        {{ }}
    }}
}}", Encoding.UTF8));
    }

    private static readonly ConstructorData Default =
        new ConstructorData(
            null,
            null,
            new ImmutableArray<ISymbol>(),
            new ImmutableArray<WrapperClassData>());

    private static ConstructorData Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        if (context.Node is not ClassDeclarationSyntax cds)
            return Default;

        var namedType = context.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (namedType is null)
            return Default;

        List<INamedTypeSymbol> allTypes = new(new[] { namedType });
        var baseType = namedType.BaseType;
        while (baseType != null)
        {
            if (baseType.ToString().Equals("object") && baseType.ContainingNamespace.ToString().Equals("System"))
                break;

            allTypes.Add(baseType);
            baseType = baseType.BaseType;
        }

        var members = allTypes.SelectMany(x => x.GetMembers())
            .Where(x => x.GetAttributes().Any(HasInjectAttribute2))
            .ToImmutableArray();

        var wrapperClasses = WrapperClassData.From(cds).ToImmutableArray();

        return new ConstructorData(cds, namedType, members, wrapperClasses);
    }

    private static bool HasInjectAttribute2(AttributeData attr)
    {
        return attr.ToString() == "Flaeng.InjectAttribute";
    }

    private static Func<AttributeSyntax, bool> HasInjectAttribute(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        return attr =>
        {
            var si = ctx.SemanticModel.GetSymbolInfo(attr, ct);
            if (si.Symbol is not IMethodSymbol ms)
                return false;

            return ms.ContainingType
                .ToDisplayString()
                .Equals($"{ATTRIBUTE_NAMESPACE}.{ATTRIBUTE_NAME}", StringComparison.Ordinal);
        };
    }
}
