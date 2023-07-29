namespace Flaeng.Productivity.Comparers;

internal class MethodDefinitionEqualityComparer : EqualityComparerBase<MethodDefinition, MethodDefinitionEqualityComparer>
{
    public override bool Equals(MethodDefinition x, MethodDefinition y)
    {
        return x.Visibility == y.Visibility
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && x.Type == y.Type
            && SequenceEqual(x.Parameters, y.Parameters, MethodParameterDefinitionEqualityComparer.Instance);
    }

    public override int GetHashCode(MethodDefinition obj)
    {
        return obj.Visibility.GetHashCode()
            ^ obj.IsStatic.GetHashCode()
            ^ (obj.Name?.GetHashCode() ?? 0)
            ^ (obj.Type?.GetHashCode() ?? 0)
            ^ GetHashCode(obj.Parameters, MethodParameterDefinitionEqualityComparer.Instance);
    }
}
