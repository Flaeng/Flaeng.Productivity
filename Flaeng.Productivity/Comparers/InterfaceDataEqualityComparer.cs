namespace Flaeng.Productivity.Comparers;

internal class InterfaceDataEqualityComparer : IEqualityComparer<InterfaceGenerator.Data>
{
    public static readonly InterfaceDataEqualityComparer Instance = new();

    public bool Equals(InterfaceGenerator.Data x, InterfaceGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && EqualityComparerHelper.SameLength(x.Diagnostics, y.Diagnostics)
            && EqualityComparerHelper.Equals(x.Members, y.Members, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace
            && EqualityComparerHelper.Equals(x.ParentClasses, y.ParentClasses, ClassDefinitionEqualityComparer.Instance)
            && x.InterfaceName == y.InterfaceName
            && x.Visibility == y.Visibility;
    }

    public int GetHashCode(InterfaceGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ EqualityComparerHelper.GetHashCode(obj.Members, IMemberDefinitionEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ EqualityComparerHelper.GetHashCode(obj.ParentClasses, ClassDefinitionEqualityComparer.Instance)
            ^ (obj.InterfaceName?.GetHashCode() ?? 0)
            ^ obj.Visibility.GetHashCode();
    }
}
