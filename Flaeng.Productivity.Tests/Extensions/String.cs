namespace Flaeng.Productivity.Tests;

public static class StringExtensions
{
    public static string WithoutGeneratedCodeAttribute(this string str)
    {
        return str
            .Split(Environment.NewLine)
            .Where(x => x.IndexOf("GeneratedCodeAttribute") == -1)
            .Join(Environment.NewLine);
    }
}
