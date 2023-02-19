using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flaeng.Productivity.DependencyInjection;
[Generator(LanguageNames.CSharp)]

public sealed class InterfaceGenerator : IIncrementalGenerator
{
    const string ATTRIBUTE_NAMESPACE = "Flaeng.Productivity.DependencyInjection";
    const string ATTRIBUTE_NAME = "GenerateInterfaceAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(GenerateAttribute);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider<InterfaceStruct>(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: 1 },
                Transform)
            .Where(static x => x.Class != null)
            .WithComparer(InterfaceStructEqualityComparer.Instance);

        context.RegisterSourceOutput<InterfaceStruct>(provider, Execute);
    }

    readonly static InterfaceStruct Default = new InterfaceStruct(
            null,
            new ImmutableArray<MemberDeclarationSyntax>(),
            new ImmutableArray<MethodDeclarationSyntax>(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassStruct>());

    private static InterfaceStruct Transform(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(ctx.Node);
        if (cds.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(HasGenerateAttribute(ctx, ct))
            .Any() == false)
            return Default;

        var symbol = ctx.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (symbol == null)
            return Default;

        var interfaces = symbol.Interfaces
            .Select(x => x.Name)
            .ToImmutableArray();

        var excludeMembers = symbol.AllInterfaces
            .SelectMany(x => x.GetMembers())
            .ToImmutableArray();

        bool shouldBeExcluded(SyntaxNode node)
        {
            var methodSymbol = ctx.SemanticModel.GetDeclaredSymbol(node, ct);
            if (methodSymbol is IMethodSymbol mSymbol)
            {
                return excludeMembers
                    .OfType<IMethodSymbol>()
                    .Contains(mSymbol, MethodComparer.Instance);
            }
            else if (methodSymbol is IPropertySymbol pSymbol)
            {
                return excludeMembers
                    .OfType<IPropertySymbol>()
                    .Contains(pSymbol, PropertyComparer.Instance);
            }
            else if (methodSymbol is IFieldSymbol fSymbol)
            {
                return excludeMembers
                    .OfType<IFieldSymbol>()
                    .Contains(fSymbol, FieldComparer.Instance);
            }
            return false;
        }

        List<MemberDeclarationSyntax> members = new();
        List<MethodDeclarationSyntax> methods = new();
        foreach (var child in cds.DescendantNodes())
        {
            if (child is MethodDeclarationSyntax method)
            {
                if (shouldBeExcluded(child) == false)
                    methods.Add(method);
            }
            else if (child is MemberDeclarationSyntax member)
            {
                if (shouldBeExcluded(child) == false)
                    members.Add(member);
            }
        }

        Stack<WrapperClassStruct> wrapperClasses = new();
        var parent = cds.Parent;
        while (parent is ClassDeclarationSyntax parentCDS)
        {
            var name = Helpers.GetClassName(parentCDS);
            var visiblity = TypeVisiblityHelper.GetFromTokens(parentCDS.ChildTokens());
            wrapperClasses.Push(new WrapperClassStruct(name, visiblity));
            parent = parent.Parent;
        }

        return new InterfaceStruct(
            cds,
            members.ToImmutableArray(),
            methods.ToImmutableArray(),
            interfaces,
            wrapperClasses.ToImmutableArray());
    }

    private static Func<AttributeSyntax, bool> HasGenerateAttribute(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        return cls =>
        {
            var si = ctx.SemanticModel.GetSymbolInfo(cls, ct);
            if (si.Symbol == null)
                return false;

            return si.Symbol.ContainingType
                .ToDisplayString()
                .Equals($"{ATTRIBUTE_NAMESPACE}.{ATTRIBUTE_NAME}", StringComparison.Ordinal);
        };
    }

    private static void Execute(SourceProductionContext context, InterfaceStruct data)
    {
        var cls = data.Class;
        if (cls is null)
            return;
        SourceBuilder sourceBuilder = new();

        var childNodes = cls.SyntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>();

        // Add using statements
        sourceBuilder.AddUsingStatement(childNodes.Select(x => x.Name.ToString()));

        // Add namespace
        var namespaceNode = cls.Parent?.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        var isInNamespace = namespaceNode != null;
        if (namespaceNode != null)
            sourceBuilder.StartNamespace(namespaceNode.Name.ToString());

        // Add wrapper-classes
        foreach (var wrapper in data.WrapperClasses)
            sourceBuilder.StartClass(new ClassOptions(wrapper.Name)
            {
                Visibility = wrapper.Visibility,
                Partial = true
            });

        // Write new source code
        createInterfaceAndClass(data, cls, sourceBuilder);

        // End wrapper-classes
        for (int i = 0; i < data.WrapperClasses.Length; i++)
            sourceBuilder.AppendEndTag();

        // End namespace
        if (isInNamespace)
            sourceBuilder.EndNamespace();

        var filename = Helpers.GenerateFilename(cls, true);
        context.AddSource($"{filename}.g.cs", sourceBuilder.ToString());
    }


    private static void createInterfaceAndClass(
        InterfaceStruct data,
        ClassDeclarationSyntax cls,
        SourceBuilder sourceBuilder
        )
    {
        var childTokens = cls.ChildTokens();
        var className = childTokens.First(x => x.IsKind(SyntaxKind.IdentifierToken));
        var interfaceName = $"I{className}";
        if (data.InterfaceNames.Contains(interfaceName))
        {
            string tmp = interfaceName;
            for (int i = 2; i < 10; i++)
            {
                tmp = $"{interfaceName}{i}";
                if (data.InterfaceNames.Contains(tmp) == false)
                    break;
            }
            interfaceName = tmp;
        }

        sourceBuilder.AddGeneratedCodeAttribute();
        var visibility = TypeVisiblityHelper.GetFromTokens(childTokens);
        var interfaceBuilder = sourceBuilder.StartInterface(new InterfaceOptions(interfaceName)
        {
            Visibility = visibility,
            // Partial = true
        });

        foreach (var member in data.Members)
            writeMember(interfaceBuilder, member);

        foreach (var method in data.Methods)
            writeMethod(interfaceBuilder, method);

        interfaceBuilder.EndInterface();

        var classBuilder = sourceBuilder.StartClass(new ClassOptions(className.ToString())
        {
            Visibility = visibility,
            Partial = true,
            Interfaces = new[] { interfaceName }
        });
        classBuilder.EndClass();
    }

    private static void writeMember(InterfaceBuilder interfaceBuilder, MemberDeclarationSyntax member)
    {
        if (member is PropertyDeclarationSyntax pds && pds.AccessorList != null)
        {
            writeMemberFromProperty(interfaceBuilder, pds);
        }
        else if (member is FieldDeclarationSyntax fds)
        {
            writeMemberFromField(interfaceBuilder, fds);
        }
    }

    private static void writeMemberFromField(InterfaceBuilder interfaceBuilder, FieldDeclarationSyntax fds)
    {
        var isPublic = fds.Modifiers.Any(x => x.Text == "public");
        if (isPublic == false)
            return;

        var nodes = fds.Declaration.DescendantNodes();
        if (nodes.Count() < 2)
            return;

        var type = nodes.First();

        var isStatic = fds.Modifiers.Any(x => x.Text.Equals("static", StringComparison.InvariantCultureIgnoreCase));

        interfaceBuilder.AddField(new FieldOptions(type.ToString(), fds.Declaration.Variables.ToString())
        {
            Static = isStatic
        });
    }

    private static void writeMemberFromProperty(InterfaceBuilder interfaceBuilder, PropertyDeclarationSyntax pds)
    {
        if (pds.AccessorList == null)
            return;

        bool publicGetter = isPublicAccessor(pds, SyntaxKind.GetAccessorDeclaration);
        bool publicSetter = isPublicAccessor(pds, SyntaxKind.SetAccessorDeclaration);

        if (publicGetter == false && publicSetter == false)
            return;

        var isStatic = pds.Modifiers.Any(x => x.Text.Equals("static", StringComparison.InvariantCultureIgnoreCase));

        var name = pds.ChildTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
            .FirstOrDefault();

        interfaceBuilder.AddProperty(new PropertyOptions(pds.Type.ToString(), name.ToString())
        {
            Static = isStatic,
            Getter = publicGetter ? GetterSetterVisiblity.Public : GetterSetterVisiblity.None,
            Setter = publicSetter ? GetterSetterVisiblity.Public : GetterSetterVisiblity.None,
        });
    }

    private static bool isPublicAccessor(PropertyDeclarationSyntax pds, SyntaxKind kind)
    {
        if (pds.AccessorList == null)
            return false;

        var accessor = pds.AccessorList.Accessors.FirstOrDefault(x => x.IsKind(kind));
        if (accessor == null)
            return false;

        var modifier = accessor.Modifiers.FirstOrDefault();
        if (String.IsNullOrWhiteSpace(modifier.Text))
            modifier = pds.Modifiers.FirstOrDefault();

        return modifier.IsKind(SyntaxKind.PublicKeyword);
    }

    private static void writeMethod(InterfaceBuilder interfaceBuilder, MethodDeclarationSyntax method)
    {
        var name = method.ChildTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
            .FirstOrDefault();

        var isPublic = method.ChildTokens()
            .Where(x => x.IsKind(SyntaxKind.PublicKeyword))
            .Any();

        if (isPublic == false)
            return;

        var parameterList = method.DescendantNodes()
            .OfType<ParameterListSyntax>()
            .First();

        var parameters = parameterList.DescendantNodes()
            .Where(x => x.IsKind(SyntaxKind.Parameter));

        var parameterText = parameters.Select(x => x.ToFullString());

        interfaceBuilder.AddMethodStub(new MethodOptions(method.ReturnType.ToString(), name.Text)
        {
            Parameters = new List<string>(parameterText)
        });
    }

    private static void GenerateAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource($"{ATTRIBUTE_NAME}.g.cs", @$"// <auto-generated/>

#nullable enable

namespace {ATTRIBUTE_NAMESPACE}
{{
    {Constants.GeneratedCodeAttribute}
    [global::System.AttributeUsageAttribute(
        global::System.AttributeTargets.Class, 
        AllowMultiple = false,
        Inherited = false)]
    internal sealed class {ATTRIBUTE_NAME} : global::System.Attribute
    {{
        public {ATTRIBUTE_NAME}()
        {{ }}
    }}
}}");
    }
}
