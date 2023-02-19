using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity;

class FieldComparer : IEqualityComparer<IFieldSymbol>
{
    public static FieldComparer Instance = new();

    public bool Equals(IFieldSymbol x, IFieldSymbol y)
    {
        return SymbolEqualityComparer.Default.Equals(x.Type, y.Type)
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name;
    }

    public int GetHashCode(IFieldSymbol obj)
    {
        throw new NotImplementedException();
    }
}
