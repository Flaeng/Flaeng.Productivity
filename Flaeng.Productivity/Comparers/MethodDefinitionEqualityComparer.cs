namespace Flaeng.Productivity.Comparers;

internal class MethodDefinitionEqualityComparer : IEqualityComparer<MethodDefinition>
{
    public static readonly MethodDefinitionEqualityComparer Instance = new();

    public bool Equals(MethodDefinition x, MethodDefinition y)
    {
        return x.Visibility == y.Visibility
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && x.Type == y.Type
            && (
                (
                    x.Parameters == null 
                    && y.Parameters == null
                ) || (
                    x.Parameters != null
                    && y.Parameters != null
                    && x.Parameters.SequenceEqual(y.Parameters, MethodParameterDefinitionEqualityComparer.Instance)
                )
            );
    }

    public int GetHashCode(MethodDefinition obj)
    {
        return obj.Visibility.GetHashCode()
            ^ obj.IsStatic.GetHashCode()
            ^ (obj.Name?.GetHashCode() ?? 0) 
            ^ (obj.Type?.GetHashCode() ?? 0)
            ^ (
                obj.Parameters == null 
                    ? 0
                    : obj.Parameters.Aggregate(0, (x, y) => x ^ y.GetHashCode())
            );
    }
}
