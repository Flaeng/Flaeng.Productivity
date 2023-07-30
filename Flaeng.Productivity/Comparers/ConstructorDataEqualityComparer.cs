namespace Flaeng.Productivity.Comparers;

internal class ConstructorDataEqualityComparer : EqualityComparerBase<ConstructorGenerator.Data, ConstructorDataEqualityComparer>
{
    public override bool Equals(ConstructorGenerator.Data x, ConstructorGenerator.Data y)
    {
        return ClassDefinitionEqualityComparer.Instance.Equals(x.ClassDefinition, y.ClassDefinition)
            && SameLength(x.Diagnostics, y.Diagnostics)
            && SequenceEqual(x.InjectableMembers, y.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
            && x.Namespace == y.Namespace
            && SequenceEqual(x.ContainingClasses, y.ContainingClasses, ClassDefinitionEqualityComparer.Instance);
    }

    public override int GetHashCode(ConstructorGenerator.Data obj)
    {
        return ClassDefinitionEqualityComparer.Instance.GetHashCode(obj.ClassDefinition)
            ^ (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ GetHashCode(obj.InjectableMembers, IMemberDefinitionEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0)
            ^ GetHashCode(obj.ContainingClasses, ClassDefinitionEqualityComparer.Instance);
    }
}
