using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            .CreateSyntaxProvider<GenerateInterfaceStruct>(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: 1 },
                static (ctx, ct) =>
                {
                    var cds = Unsafe.As<ClassDeclarationSyntax>(ctx.Node);
                    if (cds.AttributeLists
                        .SelectMany(x => x.Attributes)
                        .Where(HasGenerateAttribute(ctx, ct))
                        .Any() == false)
                        return new GenerateInterfaceStruct(
                            null,
                            new ImmutableArray<MemberDeclarationSyntax>(),
                            new ImmutableArray<MethodDeclarationSyntax>());

                    List<MemberDeclarationSyntax> members = new();
                    List<MethodDeclarationSyntax> methods = new();
                    foreach (var child in cds.DescendantNodes())
                    {
                        if (child is MemberDeclarationSyntax member)
                            members.Add(member);
                        else if (child is MethodDeclarationSyntax method)
                            methods.Add(method);
                    }
                    return new GenerateInterfaceStruct(
                        cds,
                        members.ToImmutableArray(),
                        methods.ToImmutableArray());
                })
            .Where(static x => x.Class != null)
            .WithComparer(GenerateInterfaceEqualityComparer.Instance);

        context.RegisterSourceOutput<GenerateInterfaceStruct>(provider, Execute);
    }

    private static Func<AttributeSyntax, bool> HasGenerateAttribute(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        return cls =>
        {
            var si = ctx.SemanticModel.GetSymbolInfo(cls, ct);
            if (si.Symbol == null)
                return false;
            // if (si.Symbol is not IMethodSymbol ms)
            //     return false;

            // return ms.ContainingType
            return si.Symbol.ContainingType
                .ToDisplayString()
                .Equals($"{ATTRIBUTE_NAMESPACE}.{ATTRIBUTE_NAME}", StringComparison.Ordinal);
        };
    }

    private void Execute(SourceProductionContext context, GenerateInterfaceStruct data)
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

        var interfaceName = "I" + cls.GetClassName();
        sourceBuilder.StartInterface(TypeVisiblity.Public, interfaceName, partial: true);

        var publicMethods = cls.DescendantNodes()
            .OfType<MethodDeclarationSyntax>();

        foreach (var method in publicMethods)
            writeMethod(sourceBuilder, method);

        sourceBuilder.EndInterface();

        sourceBuilder.StartClass(TypeVisiblity.Public, cls.GetClassName(), partial: true, interfaces: new[] { interfaceName });
        sourceBuilder.EndClass();

        if (isInNamespace)
            sourceBuilder.EndNamespace();

        StringBuilder filename = new();
        if (cls.Parent is NamespaceDeclarationSyntax nds)
        {
            filename.Append(nds.Name);
            filename.Append('.');
        }
        else if (cls.Parent is FileScopedNamespaceDeclarationSyntax fsnds)
        {
            filename.Append(fsnds.Name);
            filename.Append('.');
        }
        filename.Append('I');
        filename.Append(cls.GetClassName());

        // var filename = Path.GetFileNameWithoutExtension(cls.SyntaxTree.FilePath);
        context.AddSource($"{filename}.g.cs", sourceBuilder.ToString());
    }

    private void writeMethod(SourceBuilder sourceBuilder, MethodDeclarationSyntax method)
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

    private void GenerateAttribute(IncrementalGeneratorPostInitializationContext context)
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
