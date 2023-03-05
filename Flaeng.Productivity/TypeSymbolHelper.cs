namespace Flaeng.Productivity;

internal static class TypeSymbolHelper
{
    public static string WriteType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol named)
        {
            if (named.TypeArguments.Any())
            {
                var ori = named.OriginalDefinition.ToString();
                var args = named.TypeArguments.Select(WriteType).ToImmutableArray();
                return $"{ori.TrimEnd('>')}{String.Join(", ", args)}>";
            }
        }

        var typeName = typeSymbol.ContainingNamespace == null 
            || typeSymbol.ContainingNamespace.ToString() == "<global namespace>"
            || typeSymbol.ContainingNamespace.ToString() == "System"
            ? typeSymbol.ToString()
            : $"global::{typeSymbol}";

        return typeName;
    }

    public static string WriteParameter(IParameterSymbol param)
    {
        string result = $"{WriteType(param.Type)} {param.Name}";
        switch (param.RefKind)
        {
            case RefKind.None: 
                return result;
            case RefKind.Ref: 
                return $"ref {result}";
            case RefKind.Out: 
                return $"out {result}";
            case RefKind.In: 
                return $"in {result}";
        }
        throw new Exception($"Unknown refkind for parameter with name: '{param.Name}'");
    }
}
