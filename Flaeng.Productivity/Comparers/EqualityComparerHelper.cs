namespace Flaeng.Productivity.Comparers;

internal static class EqualityComparerHelper
{
    public static bool SameLength<T>(ImmutableArray<T> collection1, ImmutableArray<T> collection2)
    {
        if (collection1 == default && collection2 == default)
            return true;

        if (collection1 == default || collection2 == default)
            return false;

        return collection1.Length == collection2.Length;
    }

    public static bool Equals<T>(ImmutableArray<T> collection1, ImmutableArray<T> collection2, IEqualityComparer<T> comparer)
    {
        if (collection1 == default && collection2 == default)
            return true;

        if (collection1 == default || collection2 == default)
            return false;

        return collection1.SequenceEqual(collection2, comparer);
    }

    public static int GetHashCode<T>(ImmutableArray<T> collection, IEqualityComparer<T>? comparer)
    {
        if (collection == default)
            return 0;

        return collection.Aggregate(0, (x, y) => x ^ (comparer is null ? (y?.GetHashCode() ?? 0) : comparer.GetHashCode(y)));
    }
}
