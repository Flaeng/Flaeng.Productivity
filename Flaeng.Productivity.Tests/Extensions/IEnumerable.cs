namespace Flaeng.Productivity.Tests;

public static class IEnumerableExtensions
{
    public static string Join(this IEnumerable<string> coll, string seperator)
    {
        return String.Join(seperator, coll);
    }
}
