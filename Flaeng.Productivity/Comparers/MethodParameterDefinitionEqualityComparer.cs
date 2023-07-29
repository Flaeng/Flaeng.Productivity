namespace Flaeng.Productivity.Comparers;

internal class MethodParameterDefinitionEqualityComparer : EqualityComparerBase<MethodParameterDefinition, MethodParameterDefinitionEqualityComparer>
{
    public override bool Equals(MethodParameterDefinition x, MethodParameterDefinition y)
    {
        return x.DefaultValue == y.DefaultValue
            && x.Name == y.Name
            && x.ParameterKind == y.ParameterKind
            && x.Type == y.Type;
    }

    public override int GetHashCode(MethodParameterDefinition obj)
    {
        return (obj.DefaultValue?.GetHashCode() ?? 0)
            ^ (obj.Name?.GetHashCode() ?? 0)
            ^ (obj.ParameterKind?.GetHashCode() ?? 0)
            ^ (obj.Type?.GetHashCode() ?? 0);
    }
}
