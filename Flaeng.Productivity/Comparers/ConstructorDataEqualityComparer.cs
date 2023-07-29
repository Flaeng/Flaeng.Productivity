namespace Flaeng.Productivity.Comparers;

internal class ConstructorDataEqualityComparer : IEqualityComparer<ConstructorGenerator.Data>
{
    public static readonly ConstructorDataEqualityComparer Instance = new();

    public bool Equals(ConstructorGenerator.Data x, ConstructorGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && EqualityComparerHelper.SameLength(x.Diagnostics, y.Diagnostics)
            && EqualityComparerHelper.Equals(x.InjectableMembers, y.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace
            && EqualityComparerHelper.Equals(x.ContainingClasses, y.ContainingClasses, ClassDefinitionEqualityComparer.Instance);
    }

    public int GetHashCode(ConstructorGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ EqualityComparerHelper.GetHashCode(obj.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ EqualityComparerHelper.GetHashCode(obj.ContainingClasses, ClassDefinitionEqualityComparer.Instance);
    }
}
