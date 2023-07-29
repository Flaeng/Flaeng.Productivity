namespace Flaeng.Productivity.Comparers;

internal class MethodParameterDefinitionEqualityComparer : IEqualityComparer<MethodParameterDefinition>
{
    public static readonly MethodParameterDefinitionEqualityComparer Instance = new();

    public bool Equals(MethodParameterDefinition x, MethodParameterDefinition y)
    {
        return x.DefaultValue == y.DefaultValue
            && x.Name == y.Name
            && x.ParameterKind == y.ParameterKind
            && x.Type == y.Type;
    }

    public int GetHashCode(MethodParameterDefinition obj)
    {
        return (obj.DefaultValue?.GetHashCode() ?? 0)
            ^ (obj.Name?.GetHashCode() ?? 0)
            ^ (obj.ParameterKind?.GetHashCode() ?? 0)
            ^ (obj.Type?.GetHashCode() ?? 0);
    }
}
