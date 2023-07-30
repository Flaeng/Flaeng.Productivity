namespace Flaeng.Productivity.Comparers;

internal class ConstructorDataEqualityComparer : EqualityComparerBase<ConstructorGenerator.Data, ConstructorDataEqualityComparer>
{
    public override bool Equals(ConstructorGenerator.Data x, ConstructorGenerator.Data y)
    {
        return x.ClassDefinition.IsPartial == y.ClassDefinition.IsPartial
            && x.ClassDefinition.Name == y.ClassDefinition.Name
            && x.ClassDefinition.Visibility == y.ClassDefinition.Visibility
            && SameLength(x.Diagnostics, y.Diagnostics)
            && SequenceEqual(x.InjectableMembers, y.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace
            && SequenceEqual(x.ContainingClasses, y.ContainingClasses, ClassDefinitionEqualityComparer.Instance);
    }

    public override int GetHashCode(ConstructorGenerator.Data obj)
    {
        return obj.ClassDefinition.GetHashCode()
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ GetHashCode(obj.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ GetHashCode(obj.ContainingClasses, ClassDefinitionEqualityComparer.Instance);
    }
}
