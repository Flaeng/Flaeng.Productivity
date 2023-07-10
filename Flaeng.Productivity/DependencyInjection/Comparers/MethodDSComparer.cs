namespace Flaeng.Productivity;

class MethodDSComparer : IEqualityComparer<MethodDeclarationSyntax>
{
    public static MethodDSComparer Instance = new();

    public readonly string[] SKIP_STRINGS = new[]
    {
        "public", "protected", "private",
        "virtual", "override"
    };

    public bool Equals(MethodDeclarationSyntax x, MethodDeclarationSyntax y)
    {
        return x.ToString() == y.ToString();
    }

    public int GetHashCode(MethodDeclarationSyntax obj)
    {
        var nodes = obj.ChildNodes().ToArray(); // 0, 1
        var tokens = obj.ChildTokens(); // Last

        // return nodes[0].ToString().GetHashCode() ^
        //     nodes[1].ToString().GetHashCode() ^
        //     tokens.Last().ToString().GetHashCode();

        var split = obj.ToString().Split(' ');
        var compareString = String.Join(" ", split.SkipWhile(x => SKIP_STRINGS.Contains(x)));
        return compareString.GetHashCode();
    }
}
