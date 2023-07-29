namespace Flaeng.Productivity.Comparers;

internal class FluentApiDataEqualityComparer : IEqualityComparer<FluentApiGenerator.Data>
{
    public static readonly FluentApiDataEqualityComparer Instance = new();

    public bool Equals(FluentApiGenerator.Data x, FluentApiGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && EqualityComparerHelper.SameLength(x.Diagnostics, y.Diagnostics)
            && EqualityComparerHelper.Equals(x.Members, y.Members, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace;
    }

    public int GetHashCode(FluentApiGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ EqualityComparerHelper.GetHashCode(obj.Members, IMemberDefinitionEqualityComparer.Instance);
    }
}
