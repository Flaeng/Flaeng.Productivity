namespace Flaeng.Productivity.Comparers;

internal class InterfaceDataEqualityComparer : IEqualityComparer<InterfaceGenerator.Data>
{
    public static readonly InterfaceDataEqualityComparer Instance = new();

    public bool Equals(InterfaceGenerator.Data x, InterfaceGenerator.Data y)
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
            && x.Namespace == y.Namespace
            && (
                (x.ParentClasses == default && y.ParentClasses == default)
                || (
                    x.ParentClasses != default
                    && y.ParentClasses != default
                    && x.ParentClasses.SequenceEqual(y.ParentClasses, ClassDefinitionEqualityComparer.Instance)
                )
            )
            && x.InterfaceName == y.InterfaceName
            && x.Visibility == y.Visibility;
    }

    public int GetHashCode(InterfaceGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ (
                obj.Members == default
                    ? 0
                    : obj.Members.Aggregate(0, (x, y) => x ^ IMemberDefinitionEqualityComparer.Instance.GetHashCode(y))
            )
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ (
                obj.ParentClasses == default
                    ? 0
                    : obj.ParentClasses.Aggregate(0, (x, y) => x ^ ClassDefinitionEqualityComparer.Instance.GetHashCode(y))
            )
            ^ (obj.InterfaceName?.GetHashCode() ?? 0)
            ^ obj.Visibility.GetHashCode();
    }
}
