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
