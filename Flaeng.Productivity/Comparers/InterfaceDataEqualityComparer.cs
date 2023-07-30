namespace Flaeng.Productivity.Comparers;

internal class InterfaceDataEqualityComparer : EqualityComparerBase<InterfaceGenerator.Data, InterfaceDataEqualityComparer>
{
    public override bool Equals(InterfaceGenerator.Data x, InterfaceGenerator.Data y)
    {
        return ClassDefinitionEqualityComparer.Instance.Equals(x.ClassDefinition, y.ClassDefinition)
            && SameLength(x.Diagnostics, y.Diagnostics)
            && SequenceEqual(x.Members, y.Members, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace
            && SequenceEqual(x.ParentClasses, y.ParentClasses, ClassDefinitionEqualityComparer.Instance)
            && x.InterfaceName == y.InterfaceName
            && x.Visibility == y.Visibility;
    }

    public override int GetHashCode(InterfaceGenerator.Data obj)
    {
        return ClassDefinitionEqualityComparer.Instance.GetHashCode(obj.ClassDefinition)
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ GetHashCode(obj.Members, IMemberDefinitionEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ GetHashCode(obj.ParentClasses, ClassDefinitionEqualityComparer.Instance)
            ^ (obj.InterfaceName?.GetHashCode() ?? 0)
            ^ obj.Visibility.GetHashCode();
    }
}
