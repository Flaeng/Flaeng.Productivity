namespace Flaeng.Productivity.Generators;

public sealed partial class StartupGenerator
{
    public enum InjectType { Transient, Scoped, Singleton }

    public record struct InjectData(InjectType InjectType, string TypeName, ImmutableArray<string> Interfaces);

    public record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string? Namespace,
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

        return new Data(
            Diagnostics: ImmutableArray<Diagnostic>.Empty,
            Namespace: namespaceName,
            Injectables: injectables
        );
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

    // Has poor performance due to try-catch and looping through all syntax trees
    // TODO: Optimize this method
    private static INamedTypeSymbol? TryGetDeclaredSymbol(GeneratorSyntaxContext context, ClassDeclarationSyntax syntax, CancellationToken ct)
    {
        try
        {
            return context.SemanticModel.GetDeclaredSymbol(syntax, ct);
        }
        catch
        {
            foreach (var tree in context.SemanticModel.Compilation.SyntaxTrees)
            {
                var treeModel = context.SemanticModel.Compilation.GetSemanticModel(tree);
                try
                {
                    return treeModel.GetDeclaredSymbol(syntax, ct); ;
                }
                catch
                { }
            }
            return null;
        }
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
