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
            .WithComparer(ConstructorEqualityComparer.Instance);

        context.RegisterSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, ConstructorStruct data)
    {
        if (data.Class is null || data.Members == null || data.Members.Any() == false)
            return;

        SourceBuilder sourceBuilder = new();

        var isInNamespace = data.Class.ContainingNamespace != null;
        if (isInNamespace)
            sourceBuilder.StartNamespace(data.Class.ContainingNamespace!.ToDisplayString());

        sourceBuilder.StartClass(
            visibility: TypeVisiblity.Public,
            name: data.Class.Name,
            @static: false,
            partial: true);

        var members = data.Members
            .Select(TypeAndName.Parse)
            .ToImmutableArray();

        var parameters = members
            .Select(x => $"{x.TypeName} {x.Name}")
            .ToImmutableArray();

        sourceBuilder.AddGeneratedCodeAttribute();
        sourceBuilder.StartConstructor(MemberVisiblity.Public, data.Class.Name, (System.Collections.Generic.IEnumerable<string>)parameters);

        foreach (var member in members)
            sourceBuilder.AddLineOfCode($"this.{member.Name} = {member.Name};");

        sourceBuilder.EndConstructor();

        sourceBuilder.EndClass();

        if (isInNamespace)
            sourceBuilder.EndNamespace();

        string qualifiedName = data.Class.ContainingNamespace is null
            ? data.Class.Name :
            $"{data.Class.ContainingNamespace}.{data.Class.Name}";
        context.AddSource($"{qualifiedName}.g.cs", sourceBuilder.ToString());
    }

    class TypeAndName
    {
        public string TypeName { get; private set; } = "";
        public string Name { get; private set; } = "";

        private TypeAndName() { }

        private static string GetTypeDisplayString(ITypeSymbol symbol)
        {
            StringBuilder builder = new();

            if (symbol is INamedTypeSymbol ints && ints.ContainingNamespace.ToString() == "System")
            {
                return $"global::System.{symbol.Name}";
            }

            var parts = symbol.ToDisplayParts(SymbolDisplayFormat.FullyQualifiedFormat);
            var typeName = String.Join("", parts.TakeWhile(x => x.ToString() != "<"));
            builder.Append(typeName);

            if (symbol is INamedTypeSymbol nts && nts.TypeArguments.Any())
            {
                builder.Append("<");
                bool isFirst = true;
                foreach (var arg in nts.TypeArguments)
                {
                    if (!isFirst)
                        builder.Append(", ");
                    isFirst = false;
                    builder.Append(GetTypeDisplayString(arg));
                }
                builder.Append(">");
            }
            return builder.ToString();
        }

        internal static TypeAndName Parse(IFieldSymbol arg)
        {
            return new TypeAndName
            {
                TypeName = GetTypeDisplayString(arg.Type),
                Name = arg.AssociatedSymbol?.Name ?? arg.Name
            };
        }
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
        new ConstructorStruct(null, new ImmutableArray<IFieldSymbol>());

    private static ConstructorStruct Transform(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        if (ctx.Node is not ClassDeclarationSyntax cds)
            return Default;

        if (cds.Members
            .SelectMany(x => x.AttributeLists)
            .SelectMany(x => x.Attributes)
            .Where(HasInjectAttribute(ctx, ct))
            .Any() == false)
            return Default;

        var namedType = ctx.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (namedType is null)
            return Default;

        var members = namedType
            .GetMembers()
            .Where(x => x is IFieldSymbol)
            .Cast<IFieldSymbol>()
            .ToImmutableArray();

        return new ConstructorStruct(namedType, members);
    }

    private static Func<AttributeSyntax, bool> HasInjectAttribute(GeneratorSyntaxContext ctx, CancellationToken ct)
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
}