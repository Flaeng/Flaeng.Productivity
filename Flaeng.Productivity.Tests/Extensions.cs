namespace Flaeng.Productivity.Tests;

// public static class StringExtensions
// {
//     public static string CleanupForComparison(this string str)
//     {
//         return str
//             .Split(Environment.NewLine)
//             .Where(x => x.Contains("GeneratedCodeAttribute", StringComparison.CurrentCulture) == false)
//             .Where(x => x.StartsWith("#pragma", StringComparison.CurrentCulture) == false)
//             .Where(x => x != "// <auto-generated/>")
//             .Where(x => x != "#nullable enable")
//             .SkipWhile(x => x.Length == 0)
//             .Join(Environment.NewLine);
//     }
// }

public static class EnumerableExtensions
{
    public static IEnumerable<SourceFile> ExcludeTriggerAttribute(
        this IEnumerable<SourceFile> collection
    )
    {
        return collection.Where(x => x.Filename.EndsWith("Attribute.g.cs") == false);
    }
}