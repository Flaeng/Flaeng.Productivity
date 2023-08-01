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
        return CalculateHashCode(
            obj.IsStatic,
            obj.Name,
            obj.Type,
            obj.DefaultValue,
            obj.Visibility
        );
    }
}
