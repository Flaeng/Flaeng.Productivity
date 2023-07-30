namespace Flaeng.Productivity.Comparers;

internal abstract class EqualityComparerBase<T, TEqualityComparer> : IEqualityComparer<T>
    where TEqualityComparer : EqualityComparerBase<T, TEqualityComparer>, new()
{
    public static TEqualityComparer Instance = new TEqualityComparer();

    public abstract bool Equals(T x, T y);
    public abstract int GetHashCode(T obj);

    protected static bool SameLength<TData>(ImmutableArray<TData> collection1, ImmutableArray<TData> collection2)
    {
        if (collection1 == default && collection2 == default)
            return true;

        if (collection1 == default || collection2 == default)
            return false;

        return collection1.Length == collection2.Length;
    }

    protected static bool SequenceEqual<TData>(ImmutableArray<TData> collection1, ImmutableArray<TData> collection2, IEqualityComparer<TData> comparer)
    {
        if (collection1 == default && collection2 == default)
            return true;

        if (collection1 == default || collection2 == default)
            return false;

        return collection1.SequenceEqual(collection2, comparer);
    }

    public static int GetHashCode<TData>(ImmutableArray<TData> collection, IEqualityComparer<TData>? comparer)
    {
        if (collection == default)
            return 0;

        return collection.Aggregate(0, (x, y) => x ^ (comparer is null ? (y?.GetHashCode() ?? 0) : comparer.GetHashCode(y)));
    }
}
