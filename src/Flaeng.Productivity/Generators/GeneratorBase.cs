namespace Flaeng.Productivity.Generators;

public abstract class GeneratorBase : IIncrementalGenerator
{
    internal static readonly CSharpOptions DefaultCSharpOptions = new(IgnoreUnnecessaryUsingDirectives: true);
    public abstract void Initialize(IncrementalGeneratorInitializationContext context);

    protected void Initialize<T>(
        IncrementalGeneratorInitializationContext context,
        Action<IncrementalGeneratorPostInitializationContext> generateTriggerAttribute,
        Func<SyntaxNode, CancellationToken, bool> predicate,
        Func<GeneratorSyntaxContext, CancellationToken, T> transform,
        IEqualityComparer<T> instance,
        Action<SourceProductionContext, T> execute
    )
    {
        context.RegisterPostInitializationOutput(generateTriggerAttribute);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider<T>(predicate, transform)
            .Where(static x => x != null)
            .WithComparer(instance);

        context.RegisterSourceOutput(provider, execute);
    }

    protected static bool IsSystemObjectType(INamedTypeSymbol? sym)
    {
        if (sym is null)
            return false;

        return sym.ContainingNamespace.ToDisplayString().Equals("System", StringComparison.InvariantCultureIgnoreCase)
            && sym.ToDisplayString().Equals("object", StringComparison.InvariantCultureIgnoreCase);
    }

    internal static bool TryWriteNamespace(string? namespaceName, CSharpBuilder builder)
    {
        if (namespaceName is null || String.IsNullOrWhiteSpace(namespaceName))
            return false;
        builder.WriteNamespace(namespaceName);
        builder.StartScope();
        return true;
    }

    protected static bool TryWriteDiagnostics(SourceProductionContext context, ImmutableArray<Diagnostic> diagnostics)
    {
        if (diagnostics == default || diagnostics.Length == 0)
            return false;

        foreach (var dia in diagnostics)
            context.ReportDiagnostic(dia);
        return true;
    }

    protected static IEnumerable<INamedTypeSymbol> GetBaseTypeRecursively(INamedTypeSymbol? symbol)
    {
        if (symbol is null)
            yield break;

        yield return symbol;
        while ((symbol = symbol.BaseType) != null && IsSystemObjectType(symbol) == false)
        {
            yield return symbol;
        }
    }

    internal static void WriteWrapperClasses(
        ImmutableArray<ClassDefinition> parentClassList,
        CSharpBuilder builder,
        List<string> filenameParts
    )
    {
        foreach (var parentClass in parentClassList.Reverse())
        {
            builder.WriteClass(parentClass);
            builder.StartScope();

            if (parentClass.Name is not null)
                filenameParts.Add(parentClass.Name);
        }
    }

    internal static List<ClassDefinition> GetContainingTypeRecursively(INamedTypeSymbol symbol, CancellationToken ct)
    {
        List<ClassDefinition> parentClasses = new();
        var sym = symbol;
        while ((sym = sym.ContainingType) != null && IsSystemObjectType(sym) == false)
        {
            var cls = ClassDefinition.Parse(sym, ct);
            parentClasses.Add(cls);
        }

        return parentClasses;
    }

    protected static Dictionary<string, string> GetAttributeParameters(AttributeData data)
    {
        return data.NamedArguments.ToDictionary(x => x.Key, x => x.Value.ToCSharpString());
    }

    protected static Dictionary<string, string> GetAttributeParameters(
        ClassDeclarationSyntax syntax,
        string attributeName
        )
    {
        var attribute = syntax
            .ChildNodes()
            .OfType<AttributeListSyntax>()
            .SelectMany(x => x.ChildNodes())
            .OfType<AttributeSyntax>()
            .Single(x =>
            {
                var qualifiedName = x.ChildNodes().OfType<NameSyntax>().First();
                return NameSyntaxIsAttribute(qualifiedName, attributeName);
            });

        var attrArgumentListSyntaxes = attribute
            .ChildNodes()
            .OfType<AttributeArgumentListSyntax>()
            .SingleOrDefault();

        if (attrArgumentListSyntaxes is null)
            return new();

        var attrArgumentSyntaxes = attrArgumentListSyntaxes
            .ChildNodes()
            .OfType<AttributeArgumentSyntax>();

        return attrArgumentSyntaxes
            .Select(x => x.ToString())
            .Select(x => x.Split('='))
            .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
    }

    protected static string? GetNamespace(INamedTypeSymbol symbol)
    {
        return symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();
    }

    protected static ImmutableArray<ClassDeclarationSyntax> GetSyntaxes(INamedTypeSymbol symbol, ClassDeclarationSyntax cds)
    {
        return symbol.DeclaringSyntaxReferences.Length == 1
            ? new[] { cds }.ToImmutableArray()
            : GetAllDeclarations(symbol);
    }

    protected static bool HasAttribute(MemberDeclarationSyntax syntax, string attrName)
    {
        return syntax.AttributeLists
            .SelectMany(x => x.Attributes)
            .Any(attr =>
            {
                var qualifiedName = attr.ChildNodes().OfType<NameSyntax>().First();
                bool result = NameSyntaxIsAttribute(qualifiedName, attrName);
                return result;
            });
    }

    private static bool NameSyntaxIsAttribute(NameSyntax qualifiedName, string attrName)
    {
        var name = qualifiedName.ToString();

        // If user uses normal usings 
        return name == attrName
            || name == $"{attrName}Attribute"
            // If user uses name with namespace 
            || name == $"Flaeng.{attrName}"
            || name == $"Flaeng.{attrName}Attribute"
            // If user uses fully qualified name 
            || name == $"global::Flaeng.{attrName}"
            || name == $"global::Flaeng.{attrName}Attribute"
            // If user uses using-alias 
            || name.Split('.').Last() == attrName
            || name.Split('.').Last() == $"{attrName}Attribute";
    }

    protected static ImmutableArray<ClassDeclarationSyntax> GetAllDeclarations(INamedTypeSymbol symbol)
    {
        return symbol.DeclaringSyntaxReferences
                .Select(x => x.GetSyntax())
                .OfType<ClassDeclarationSyntax>()
                .ToImmutableArray();
    }

    protected static List<string> GetUsings(ImmutableArray<ClassDeclarationSyntax> syntaxes)
    {
        var roots = syntaxes
            .Select(x => x.SyntaxTree)
            .Select(x => x.TryGetRoot(out var root) ? root : null)
            .OfType<SyntaxNode>()
            .ToImmutableArray();

        List<string> usings = new();
        foreach (var child in roots.SelectMany(x => x.ChildNodesAndTokens()))
        {
            switch (child.RawKind)
            {
                case (int)SyntaxKind.UsingDirective:
                    usings.Add(child.ToString());
                    break;
            }
        }
        return usings;
    }

    private static string GetTypeStringFromSymbol(ISymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arrayType)
        {
            var nSpace = arrayType.ContainingNamespace;
            var cType = arrayType.ContainingType;
            return $"{GetTypeStringFromSymbol(arrayType.ElementType)}[]";
        }

        if (symbol is not INamedTypeSymbol namedType)
            throw new Exception("GetTypeStringFromSymbol was passed not INamedTypeSymbol");

        if (namedType.IsGenericType == false)
            return $"global::{symbol.ContainingNamespace}.{symbol.Name}";

        List<string> typeArgs = new();
        foreach (var typeArg in namedType.TypeArguments)
        {
            var typeString = (typeArg is ITypeParameterSymbol tps)
                ? tps.Name
                : GetTypeStringFromSymbol(typeArg);
            typeArgs.Add(typeString);
        }

        return $"global::{symbol.ContainingNamespace}.{symbol.Name}<{(String.Join(", ", typeArgs))}>";
    }
}
