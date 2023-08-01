namespace Flaeng.Productivity.Comparers;

internal class ClassDefinitionEqualityComparer : EqualityComparerBase<ClassDefinition, ClassDefinitionEqualityComparer>
{
    public override bool Equals(ClassDefinition x, ClassDefinition y)
    {
        return x.IsPartial == y.IsPartial
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && x.Visibility == y.Visibility
            && SequenceEqual(x.TypeArguments, y.TypeArguments, StringComparer.InvariantCulture);
    }

    public override int GetHashCode(ClassDefinition obj)
    {
        return CalculateHashCode(
            obj.IsPartial,
            obj.IsStatic,
            obj.Name,
            obj.Visibility,
            GetHashCode(obj.TypeArguments, comparer: null)
        );
    }
}
