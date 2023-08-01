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
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ GetHashCode(obj.Members, IMemberDefinitionEqualityComparer.Instance);
    }
}
