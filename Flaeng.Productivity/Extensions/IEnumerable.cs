namespace Flaeng.Productivity;

public static class IEnumerableExtensions
{
    public static Stack<T> ToStack<T>(this IEnumerable<T> coll)
    {
        var result = new Stack<T>();
        foreach (var item in coll)
            result.Push(item);
        return result;
    }
}
