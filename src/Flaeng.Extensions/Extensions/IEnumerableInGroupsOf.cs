namespace Flaeng.Extensions;

public static class IEnumerableInGroupsOf
{
    public static IEnumerable<IEnumerable<T>> InGroupsOf<T>(this IEnumerable<T> coll, int groupSize)
    {
        List<T> curr = new(groupSize);
        foreach (var item in coll)
        {
            curr.Add(item);
            if (curr.Count == groupSize)
            {
                yield return curr;
                curr.Clear();
            }
        }
        if (curr.Count != 0)
            yield return curr;
    }
}
