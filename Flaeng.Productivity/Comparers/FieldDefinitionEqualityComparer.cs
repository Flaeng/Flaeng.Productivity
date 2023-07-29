namespace Flaeng.Productivity.Comparers;

internal class FieldDefinitionEqualityComparer : EqualityComparerBase<FieldDefinition, FieldDefinitionEqualityComparer>
{
    public override bool Equals(FieldDefinition x, FieldDefinition y)
    {
        return x.IsStatic == y.IsStatic
            && x.DefaultValue == y.DefaultValue
            && x.Name == y.Name
            && x.Type == y.Type
            && x.Visibility == y.Visibility;
    }

    public override int GetHashCode(FieldDefinition obj)
    {
        return obj.IsStatic.GetHashCode()
            ^ (obj.Name?.GetHashCode() ?? 0)
            ^ (obj.Type?.GetHashCode() ?? 0)
            ^ (obj.DefaultValue?.GetHashCode() ?? 0)
            ^ obj.Visibility.GetHashCode();
    }
}
