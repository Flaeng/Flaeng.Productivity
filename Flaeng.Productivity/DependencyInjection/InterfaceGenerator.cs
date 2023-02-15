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
        context.RegisterPostInitializationOutput(generateAttribute);

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
        var classSymbol = ctx.SemanticModel.GetDeclaredSymbol(cds, ct);

        if (cds.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(HasGenerateAttribute(ctx, ct))
            .Any() == false)

            return new InterfaceStruct(
                classSymbol,
                new ImmutableArray<ISymbol>(),
                new ImmutableArray<IMethodSymbol>());

        List<ISymbol> members = new();
        List<IMethodSymbol> methods = new();
        foreach (var child in cds.Members)
        {
            if (child is MethodDeclarationSyntax method)
            {
                var symbol = ctx.SemanticModel.GetDeclaredSymbol(method, ct);
                if (symbol != null)
                    methods.Add(symbol);
            }
            else if (child is MemberDeclarationSyntax member)
            {
                var symbol = ctx.SemanticModel.GetDeclaredSymbol(member, ct);
                if (symbol != null)
                    members.Add(symbol);
            }
        }
        return new InterfaceStruct(
            classSymbol,
            members.ToImmutableArray(),
            methods.ToImmutableArray());
    }

    private static Func<AttributeSyntax, bool> HasGenerateAttribute(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        return attr =>
        {
            var si = ctx.SemanticModel.GetSymbolInfo(attr, ct);
            if (si.Symbol is null)
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

        var isInNamespace = cls.ContainingNamespace != null;
        if (isInNamespace)
            sourceBuilder.StartNamespace(cls.ContainingNamespace!.ToDisplayString());

        makeInterface(data, cls, sourceBuilder);

        if (isInNamespace)
            sourceBuilder.EndNamespace();

        string qualifiedName = cls.ContainingNamespace is null
            ? $"I{cls.Name}" :
            $"{cls.ContainingNamespace}.I{cls.Name}";
        context.AddSource($"{qualifiedName}.g.cs", sourceBuilder.ToString());
    }

    private static void makeInterface(InterfaceStruct data, INamedTypeSymbol cls, SourceBuilder sourceBuilder)
    {
        var interfaceName = $"I{cls.Name}";
        sourceBuilder.StartInterface(TypeVisiblity.Public, interfaceName, partial: true);

        var publicMethods = data.Methods == null ? new IMethodSymbol[0] : data.Methods
            .Where(x => x.DeclaredAccessibility == Accessibility.Public);

        foreach (var method in publicMethods)
            writeMethod(sourceBuilder, method);

        sourceBuilder.EndInterface();

        sourceBuilder.StartClass(
            TypeVisiblity.Public,
            cls.Name,
            partial: true,
            interfaces: new[] { interfaceName });
        sourceBuilder.EndClass();

    }

    private static void writeMethod(SourceBuilder sourceBuilder, IMethodSymbol method)
    {
        var parameterText = method.Parameters
            .Select(x =>
            {
                // var result = $"global::{x.Type.ContainingNamespace}.{x.Type.Name} {x.Name}";
                var result = $"{TypeAndName.GetTypeDisplayString(x.Type)} {x.Name}";
                return x.RefKind == RefKind.Out ? $"out {result}"
                    : x.RefKind == RefKind.Ref ? $"ref {result}"
                    : result;
            });

        var isVoid = method.ReturnType.ToString() == "void";

        sourceBuilder.DeclareMethod(
            isVoid
                ? "void"
                : TypeAndName.GetTypeDisplayString(method.ReturnType),
            method.Name,
            parameterText);
    }

    private static void generateAttribute(IncrementalGeneratorPostInitializationContext context)
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