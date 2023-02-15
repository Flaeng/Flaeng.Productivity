using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity.DependencyInjection;

internal record struct ConstructorStruct
(
    INamedTypeSymbol? Class,
    ImmutableArray<IFieldSymbol> Members
);

internal class ConstructorStructEqualityComparer : IEqualityComparer<ConstructorStruct>
{
    public static ConstructorStructEqualityComparer Instance = new();

    public bool Equals(ConstructorStruct x, ConstructorStruct y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return SymbolEqualityComparer.Default.Equals(x.Class, y.Class)
            && x.Members.SequenceEqual(y.Members);
    }

    public int GetHashCode(ConstructorStruct obj)
    {
        throw new NotImplementedException();
    }
}