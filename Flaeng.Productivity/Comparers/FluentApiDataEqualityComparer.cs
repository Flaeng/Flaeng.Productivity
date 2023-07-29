namespace Flaeng.Productivity.Comparers;

internal class FluentApiDataEqualityComparer : IEqualityComparer<FluentApiGenerator.Data>
{
    public static readonly FluentApiDataEqualityComparer Instance = new();

    public bool Equals(FluentApiGenerator.Data x, FluentApiGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && (
                (x.Diagnostics == null && y.Diagnostics == null)
                || (x.Diagnostics != null
                    && y.Diagnostics != null
                    && x.Diagnostics.Length == y.Diagnostics.Length
                )
            )
            && (
                (x.Members == default && y.Members == default)
                || (
                    x.Members != default
                    && y.Members != default
                    && x.Members.SequenceEqual(y.Members, IMemberDefinitionEqualityComparer.Instance)
                )
            )
            && x.Namespace == y.Namespace;
    }

    public int GetHashCode(FluentApiGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ (
                obj.Members == default
                    ? 0
                    : obj.Members.Aggregate(0, (x, y) => x ^ IMemberDefinitionEqualityComparer.Instance.GetHashCode(y))
            )
            ^ (obj.Namespace?.GetHashCode() ?? 0);
    }
}
