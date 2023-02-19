using System.Collections.Generic;
using System.Linq;

namespace Flaeng.Productivity;

internal class InterfaceStructEqualityComparer : IEqualityComparer<InterfaceStruct>
{
    public static InterfaceStructEqualityComparer Instance = new();

    public bool Equals(InterfaceStruct x, InterfaceStruct y)
    {
        return x.Class == y.Class
            && x.Members.SequenceEqual(y.Members)
            && x.Methods.SequenceEqual(y.Methods)
            && x.InterfaceNames.SequenceEqual(y.InterfaceNames);
    }

    public int GetHashCode(InterfaceStruct obj)
    {
        throw new NotImplementedException();
    }
}
