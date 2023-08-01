namespace Flaeng.Productivity.Comparers;

internal class PropertyDefinitionEqualityComparer : EqualityComparerBase<PropertyDefinition, PropertyDefinitionEqualityComparer>
{
    public override bool Equals(PropertyDefinition x, PropertyDefinition y)
    {
        return x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && x.Type == y.Type
            && x.DefaultValue == y.DefaultValue
            && x.GetterVisibility == y.GetterVisibility
            && x.SetterVisibility == y.SetterVisibility
            && x.Visibility == y.Visibility;
    }

    public override int GetHashCode(PropertyDefinition obj)
    {
        return obj.IsStatic.GetHashCode()
            ^ (obj.Name?.GetHashCode() ?? 0)
            ^ (obj.Type?.GetHashCode() ?? 0)
            ^ (obj.DefaultValue?.GetHashCode() ?? 0)
            ^ obj.GetterVisibility.GetHashCode()
            ^ obj.SetterVisibility.GetHashCode()
            ^ obj.Visibility.GetHashCode();
    }
}
