using System.Collections.Generic;
using System.Linq;

namespace Flaeng.Productivity;

class MethodComparer : IEqualityComparer<IMethodSymbol>
{
    public static MethodComparer Instance = new();
    public bool Equals(IMethodSymbol x, IMethodSymbol y)
    {
        return SymbolEqualityComparer.Default.Equals(x.ReturnType, y.ReturnType)
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && x.Parameters.SequenceEqual(y.Parameters, ParameterComparer.Instance);
    }

    public int GetHashCode(IMethodSymbol obj)
    {
        throw new NotImplementedException();
    }
}

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
