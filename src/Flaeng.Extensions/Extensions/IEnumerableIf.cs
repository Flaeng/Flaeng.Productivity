namespace Flaeng.Extensions;

public static class IEnumerableIf
{
    public static IEnumerable<T> If<T>(this IEnumerable<T> coll, bool condition, Func<IEnumerable<T>, IEnumerable<T>> transform)
    {
        return condition ? transform(coll) : coll;
    }
}
