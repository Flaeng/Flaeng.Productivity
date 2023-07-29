namespace Flaeng.Productivity.Comparers;

internal class InterfaceDataEqualityComparer : EqualityComparerBase<InterfaceGenerator.Data, InterfaceDataEqualityComparer>
{
    public override bool Equals(InterfaceGenerator.Data x, InterfaceGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && SameLength(x.Diagnostics, y.Diagnostics)
            && SequenceEqual(x.Members, y.Members, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace
            && SequenceEqual(x.ParentClasses, y.ParentClasses, ClassDefinitionEqualityComparer.Instance)
            && x.InterfaceName == y.InterfaceName
            && x.Visibility == y.Visibility;
    }

    public override int GetHashCode(InterfaceGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ GetHashCode(obj.Members, IMemberDefinitionEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ GetHashCode(obj.ParentClasses, ClassDefinitionEqualityComparer.Instance)
            ^ (obj.InterfaceName?.GetHashCode() ?? 0)
            ^ obj.Visibility.GetHashCode();
    }
}
