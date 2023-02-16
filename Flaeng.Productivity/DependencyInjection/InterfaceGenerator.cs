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

    private static InterfaceStruct Transform(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(ctx.Node);
        if (cds.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(HasGenerateAttribute(ctx, ct))
            .Any() == false)
            return new InterfaceStruct(
                null,
                new ImmutableArray<MemberDeclarationSyntax>(),
                new ImmutableArray<MethodDeclarationSyntax>());

        List<MemberDeclarationSyntax> members = new();
        List<MethodDeclarationSyntax> methods = new();
        foreach (var child in cds.DescendantNodes())
        {
            if (child is MethodDeclarationSyntax method)
                methods.Add(method);
            else if (child is MemberDeclarationSyntax member)
                members.Add(member);
        }
        return new InterfaceStruct(
            cds,
            members.ToImmutableArray(),
            methods.ToImmutableArray());
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

        sourceBuilder.AddUsingStatement(childNodes.Select(x => x.Name.ToString()));

        var namespaceNode = cls.Parent?.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        var isInNamespace = namespaceNode != null;
        if (namespaceNode != null)
            sourceBuilder.StartNamespace(namespaceNode.Name.ToString());

        createInterfaceAndClass(data, cls, sourceBuilder);

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
        var className = cls.ChildTokens().First(x => x.IsKind(SyntaxKind.IdentifierToken));
        var interfaceName = $"I{className}";
        sourceBuilder.StartInterface(TypeVisiblity.Public, interfaceName, partial: true);

        foreach (var member in data.Members)
            writeMember(sourceBuilder, member);

        foreach (var method in data.Methods)
            writeMethod(sourceBuilder, method);

        sourceBuilder.EndInterface();

        sourceBuilder.StartClass(TypeVisiblity.Public, className.ToString(), partial: true, interfaces: new[] { interfaceName });
        sourceBuilder.EndClass();
    }

    private static void writeMember(SourceBuilder sourceBuilder, MemberDeclarationSyntax member)
    {
        if (member is PropertyDeclarationSyntax pds && pds.AccessorList != null)
        {
            writeMemberFromProperty(sourceBuilder, pds);
        }
        else if (member is FieldDeclarationSyntax fds)
        {
            writeMemberFromField(sourceBuilder, fds);
        }
    }

    private static void writeMemberFromField(SourceBuilder sourceBuilder, FieldDeclarationSyntax fds)
    {
        var isPublic = fds.Modifiers.Any(x => x.Text == "public");
        if (isPublic == false)
            return;

        var nodes = fds.Declaration.DescendantNodes();
        if (nodes.Count() < 2)
            return;

        var type = nodes.First();

        var isStatic = fds.Modifiers.Any(x => x.Text.Equals("static", StringComparison.InvariantCultureIgnoreCase));

        sourceBuilder.AddLineOfCode($"{(isStatic ? "static " : "")}{type} {fds.Declaration.Variables};");
    }

    private static void writeMemberFromProperty(SourceBuilder sourceBuilder, PropertyDeclarationSyntax pds)
    {
        if (pds.AccessorList == null)
            return;

        var getter = pds.AccessorList.Accessors.FirstOrDefault(x => x.Keyword.Text == "get");
        var getterMod = getter.Modifiers.FirstOrDefault();
        if (String.IsNullOrWhiteSpace(getterMod.Text))
            getterMod = pds.Modifiers.FirstOrDefault();

        var setter = pds.AccessorList.Accessors.FirstOrDefault(x => x.Keyword.Text == "set");
        var setterMod = setter.Modifiers.FirstOrDefault();
        if (String.IsNullOrWhiteSpace(setterMod.Text))
            setterMod = pds.Modifiers.FirstOrDefault();

        bool publicGetter = getterMod.Text.Equals("public", StringComparison.InvariantCultureIgnoreCase),
            publicSetter = setterMod.Text.Equals("public", StringComparison.InvariantCultureIgnoreCase);

        if (publicGetter == false && publicSetter == false)
            return;

        var isStatic = pds.Modifiers.Any(x => x.Text.Equals("static", StringComparison.InvariantCultureIgnoreCase));

        var name = pds.ChildTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
            .FirstOrDefault();
        sourceBuilder.AddLineOfCode($"{(isStatic ? "static " : "")}{pds.Type} {name} {{ {(publicGetter ? "get; " : "")}{(publicSetter ? "set; " : "")}}}");
    }

    private static void writeMethod(SourceBuilder sourceBuilder, MethodDeclarationSyntax method)
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

        sourceBuilder.DeclareMethod(method.ReturnType.ToString(), name.Text, parameterText);
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
