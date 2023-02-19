using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace Flaeng.Productivity;

class PropertyComparer : IEqualityComparer<IPropertySymbol>
{
    public static PropertyComparer Instance = new();

    public bool Equals(IPropertySymbol x, IPropertySymbol y)
    {
        return SymbolEqualityComparer.Default.Equals(x.Type, y.Type)
            && x.IsStatic == y.IsStatic
            && x.Name == y.Name;
        // compare getter and setter visibility
    }

    public int GetHashCode(IPropertySymbol obj)
    {
        throw new NotImplementedException();
    }
}
