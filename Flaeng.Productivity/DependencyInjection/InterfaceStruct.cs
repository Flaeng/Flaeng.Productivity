
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity;

internal record struct InterfaceStruct
(
    INamedTypeSymbol? Class,
    ImmutableArray<ISymbol> Members,
    ImmutableArray<IMethodSymbol> Methods
);

internal class InterfaceStructEqualityComparer : IEqualityComparer<InterfaceStruct>
{
    public static InterfaceStructEqualityComparer Instance = new();

    public bool Equals(InterfaceStruct x, InterfaceStruct y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return SymbolEqualityComparer.Default.Equals(x.Class, y.Class)
            && x.Members.SequenceEqual(y.Members)
            && x.Methods.SequenceEqual(y.Methods);
    }

    public int GetHashCode(InterfaceStruct obj)
    {
        throw new NotImplementedException();
    }
}