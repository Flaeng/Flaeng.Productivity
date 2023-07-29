namespace Flaeng.Productivity.Comparers;

internal class ClassDefinitionEqualityComparer : IEqualityComparer<ClassDefinition>
{
    public static readonly ClassDefinitionEqualityComparer Instance = new();

    public bool Equals(ClassDefinition x, ClassDefinition y)
    {
        return x.IsPartial == y.IsPartial
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && x.Visibility == y.Visibility
            && EqualityComparerHelper.Equals(x.TypeArguments, y.TypeArguments, StringComparer.InvariantCulture);
    }

    public int GetHashCode(ClassDefinition obj)
    {
        return obj.IsPartial.GetHashCode()
            ^ obj.IsStatic.GetHashCode()
            ^ (obj.Name?.GetHashCode() ?? 0)
            ^ obj.Visibility.GetHashCode()
            ^ EqualityComparerHelper.GetHashCode(obj.TypeArguments, comparer: null);
    }
}
