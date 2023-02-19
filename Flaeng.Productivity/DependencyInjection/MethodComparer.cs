using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity;


class MethodComparer : IEqualityComparer<IMethodSymbol>
{
    public static MethodComparer Instance = new();
    public bool Equals(IMethodSymbol x, IMethodSymbol y)
    {
        if (!SymbolEqualityComparer.Default.Equals(x.ReturnType, y.ReturnType))
            return false;

        if (x.IsStatic != y.IsStatic)
            return false;

        if (x.Name != y.Name)
            return false;

        return x.Parameters.SequenceEqual(y.Parameters, ParameterComparer.Instance);
    }

    public int GetHashCode(IMethodSymbol obj)
    {
        throw new NotImplementedException();
    }
}
class ParameterComparer : IEqualityComparer<IParameterSymbol>
{
    public static ParameterComparer Instance = new();

    public bool Equals(IParameterSymbol x, IParameterSymbol y)
    {
        return SymbolEqualityComparer.Default.Equals(x.Type, y.Type)
            && x.Name == y.Name
            && x.RefKind == y.RefKind;
    }

    public int GetHashCode(IParameterSymbol obj)
    {
        throw new NotImplementedException();
    }
}
