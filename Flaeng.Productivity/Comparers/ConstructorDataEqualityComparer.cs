namespace Flaeng.Productivity.Comparers;

internal class ConstructorDataEqualityComparer : IEqualityComparer<ConstructorGenerator.Data>
{
    public static readonly ConstructorDataEqualityComparer Instance = new();

    public bool Equals(ConstructorGenerator.Data x, ConstructorGenerator.Data y)
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
                (x.InjectableMembers == default && y.InjectableMembers == default)
                || (
                    x.InjectableMembers != default
                    && y.InjectableMembers != default
                    && x.InjectableMembers.SequenceEqual(y.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
                )
            )
            && x.Namespace == y.Namespace
            && (
                (x.ContainingClasses == default && y.ContainingClasses == default)
                || (
                    x.ContainingClasses != default
                    && y.ContainingClasses != default
                    && x.ContainingClasses.SequenceEqual(y.ContainingClasses, ClassDefinitionEqualityComparer.Instance)
                )
            );
    }

    public int GetHashCode(ConstructorGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ (
                obj.InjectableMembers == default
                    ? 0
                    : obj.InjectableMembers.Aggregate(0, (x, y) => x ^ IMemberDefinitionEqualityComparer.Instance.GetHashCode(y))
            )
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ (
                obj.ContainingClasses == default
                    ? 0
                    : obj.ContainingClasses.Aggregate(0, (x, y) => x ^ ClassDefinitionEqualityComparer.Instance.GetHashCode(y))
            );
    }
}
