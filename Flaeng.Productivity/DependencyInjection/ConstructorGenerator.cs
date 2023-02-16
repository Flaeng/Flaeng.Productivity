using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Flaeng.Productivity.DependencyInjection;

[Generator(LanguageNames.CSharp)]
public sealed class ConstructorGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE_NAMESPACE = "Flaeng.Productivity.DependencyInjection";
    const string ATTRIBUTE_NAME = "InjectAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateDependencies);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider<ConstructorStruct>(HasMembersAndIsPartialAndNotStatic, Transform)
            .Where(static x => x.Class != null)
            .WithComparer(ConstructorStructEqualityComparer.Instance);

        context.RegisterSourceOutput(provider, Execute);
    }

    private static void Execute(SourceProductionContext context, ConstructorStruct data)
    {
        var cls = data.Class;
        if (cls is null)
            return;

        SourceBuilder sourceBuilder = new();

        AddUsingStatements(cls, sourceBuilder);
        bool isInNamespace = AddNamespace(cls, sourceBuilder);

        var className = cls.ChildTokens().First(x => x.IsKind(SyntaxKind.IdentifierToken));
        sourceBuilder.StartClass(TypeVisiblity.Public, className.Text, partial: true);

        var memberList = data.Members
            .Select(GetTypeNameAndMemberName)
            .ToImmutableArray();

        if (memberList.Length == 0)
            return;

        var parameters = memberList.Select(x => $"{x.TypeName} {x.MemberName}");

        sourceBuilder.AddGeneratedCodeAttribute();
        sourceBuilder.StartConstructor(MemberVisiblity.Public, className.Text, parameters);
        foreach (var member in memberList)
            sourceBuilder.AddLineOfCode($"this.{member.MemberName} = {member.MemberName};");
        sourceBuilder.EndConstructor();

        sourceBuilder.EndClass();

        if (isInNamespace)
            sourceBuilder.EndNamespace();

        var filename = Helpers.GenerateFilename(cls, false);
        context.AddSource($"{filename}.g.cs", sourceBuilder.ToString());
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

        var tokens2 = member.DescendantTokens().Reverse();
        var memberName2 = tokens2
            .Where(x => x.IsKind(SyntaxKind.IdentifierToken))
            .First();

        var typeName2 = tokens2
            .SkipWhile(x => x != memberName2)
            .Skip(1)
            .First();
        return new TypeAndName
        {
            TypeName = typeName2.ToString(),
            MemberName = memberName2.ToString()
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

    private static bool HasMembersAndIsPartialAndNotStatic(SyntaxNode node, CancellationToken ct)
    {
        if (node is not ClassDeclarationSyntax { Members.Count: > 0 } cds)
            return false;

        return cds.Modifiers.Any(SyntaxKind.PartialKeyword) &&
            cds.Modifiers.Any(SyntaxKind.StaticKeyword) == false;
    }

    private static readonly ConstructorStruct Default =
        new ConstructorStruct(null, new ImmutableArray<MemberDeclarationSyntax>());

    private static ConstructorStruct Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        if (context.Node is not ClassDeclarationSyntax cds)
            return Default;

        var namedType = context.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (namedType is null)
            return Default;

        var members = cds.Members
            .Where(x => x.AttributeLists
                .SelectMany(x => x.Attributes)
                .Any(HasInjectAttribute(context, ct)))
            .ToImmutableArray();

        return new ConstructorStruct(cds, members);
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
