namespace Flaeng.Productivity.Generators;

public sealed partial class StartupGenerator
{
    public enum InjectType { Transient, Scoped, Singleton }

    public record struct InjectData(InjectType InjectType, string TypeName, ImmutableArray<string> Interfaces);

    public record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string? Namespace,
        string? MethodName,
        ImmutableArray<InjectData> Injectables
    );

    public static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        var namespaceName = context.SemanticModel.Compilation.GlobalNamespace.Name;

        ImmutableArray<ClassDeclarationSyntax> classDeclarations = GetClassDeclarations(context, ct);

        if (classDeclarations.First() != context.Node)
            return new Data();

        ImmutableArray<InjectData> injectables = GetInjectables(context, classDeclarations, ct);

        var settingsAttr = context.SemanticModel.Compilation.Assembly
            .GetAttributes()
            .Where(IsStartupExtensionAttribute)
            .FirstOrDefault();
        Dictionary<string, string> settings = settingsAttr == null ? new() : GetAttributeParameters(settingsAttr);

        string? methodName;
        if (settings.TryGetValue("MethodName", out methodName))
            methodName = methodName.Substring(1, methodName.Length - 2); // Remove quotes

        return new Data(
            Diagnostics: ImmutableArray<Diagnostic>.Empty,
            Namespace: namespaceName,
            MethodName: methodName,
            Injectables: injectables
        );
    }

    private static bool IsStartupExtensionAttribute(AttributeData data)
    {
        return data.AttributeClass is not null
            && data.AttributeClass.ContainingNamespace.Name == "Flaeng"
            && data.AttributeClass.Name == "StartupExtensionAttribute";
    }

    private static ImmutableArray<InjectData> GetInjectables(GeneratorSyntaxContext context, ImmutableArray<ClassDeclarationSyntax> classDeclarations, CancellationToken ct)
    {
        return classDeclarations
            .Select(x => TryGetDeclaredSymbol(context, x, ct))
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>()
            .Select(x => ToInjectData(x, ct))
            .ToImmutableArray();
    }

    private static ImmutableArray<ClassDeclarationSyntax> GetClassDeclarations(GeneratorSyntaxContext context, CancellationToken ct)
    {
        return context.SemanticModel.Compilation.SyntaxTrees
            .Select(x => x.GetRoot(ct))
            .SelectMany(x => x.DescendantNodesAndSelf(x =>
                x is ClassDeclarationSyntax
                || x is NamespaceDeclarationSyntax
                || x is FileScopedNamespaceDeclarationSyntax
                || x is CompilationUnitSyntax
            ))
            .OfType<ClassDeclarationSyntax>()
            .Where(HasTriggerAttribute)
            .ToImmutableArray();
    }

    private static INamedTypeSymbol? TryGetDeclaredSymbol(GeneratorSyntaxContext context, ClassDeclarationSyntax syntax, CancellationToken ct)
    {
        var root = context.SemanticModel.SyntaxTree.GetRoot(ct);
        if (root.Contains(syntax))
            return context.SemanticModel.GetDeclaredSymbol(syntax, ct);

        var compilcation = context.SemanticModel.Compilation;
        var semanticModels = compilcation.SyntaxTrees
            .Where(x => x.GetRoot(ct).Contains(syntax))
            .Select(x => compilcation.GetSemanticModel(x));

        return semanticModels
            .Select(x => x.GetDeclaredSymbol(syntax, ct))
            .FirstOrDefault();
    }

    private static InjectType GetInjectType(ISymbol symbol, CancellationToken ct)
    {
        var clsDeclarations = symbol.DeclaringSyntaxReferences
            .Select(x => x.GetSyntax(ct))
            .OfType<ClassDeclarationSyntax>();

        var clsDeclarationWithAttr = clsDeclarations
            .Where(HasTriggerAttribute)
            .Single();

        var dictionary = GetAttributeParameters(clsDeclarationWithAttr, "RegisterService");
        var item = dictionary.SingleOrDefault();
        return item.Value?.Split('.')?.Last() switch
        {
            "Transient" => InjectType.Transient,
            "Scoped" => InjectType.Scoped,
            "Singleton" => InjectType.Singleton,
            _ => InjectType.Scoped,
        };
    }

    private static InjectData ToInjectData(INamedTypeSymbol symbol, CancellationToken ct)
    {
        static string FormatName(INamedTypeSymbol sym, CancellationToken ct)
        {
            StringBuilder builder = new("global::");
            if (sym.ContainingNamespace.IsGlobalNamespace == false)
            {
                builder.Append(sym.ContainingNamespace);
                builder.Append(".");
            }
            foreach (var containingType in GetContainingTypeRecursively(sym, ct).AsEnumerable().Reverse())
            {
                builder.Append(containingType.Name);
                builder.Append(".");
            }
            builder.Append(sym.Name);
            return builder.ToString();
        }

        return new InjectData(
            GetInjectType(symbol, ct),
            FormatName(symbol, ct),
            symbol.Interfaces.Select(x => FormatName(x, ct)).ToImmutableArray()
        );
    }
}
