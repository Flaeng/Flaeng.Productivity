namespace Flaeng.Extensions;

public static class ObjectEqualsAny
{
    public static bool EqualsAny<T>(this T value, params T[] matches)
    {
        return matches.Contains(value);
    }
}
