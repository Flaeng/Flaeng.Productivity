namespace Flaeng.Productivity.Comparers;

internal class FluentApiDataEqualityComparer : EqualityComparerBase<FluentApiGenerator.Data, FluentApiDataEqualityComparer>
{
    public override bool Equals(FluentApiGenerator.Data x, FluentApiGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && SameLength(x.Diagnostics, y.Diagnostics)
            && SequenceEqual(x.Members, y.Members, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace;
    }

    public override int GetHashCode(FluentApiGenerator.Data obj)
    {
        return CalculateHashCode(
            ClassDefinitionEqualityComparer.Instance.GetHashCode(obj.ClassDefinition),
            obj.Diagnostics,
            GetHashCode(obj.Members, IMemberDefinitionEqualityComparer.Instance)
        );
    }
}
