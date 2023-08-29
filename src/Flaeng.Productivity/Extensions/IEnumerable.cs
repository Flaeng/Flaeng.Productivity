namespace Flaeng.Productivity;

internal static class IEnumerableExtensions
{
    public static string Join(this IEnumerable<string> coll)
        => coll.Join(String.Empty);

    public static string Join(this IEnumerable<string> coll, char seperator)
        => coll.Join(seperator.ToString());

    public static string Join(this IEnumerable<string> coll, string seperator)
        => String.Join(seperator, coll);
}
