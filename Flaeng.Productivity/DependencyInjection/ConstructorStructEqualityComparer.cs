using System.Collections.Generic;
using System.Linq;

namespace Flaeng.Productivity.DependencyInjection;

internal class ConstructorStructEqualityComparer : IEqualityComparer<ConstructorStruct>
{
    public static ConstructorStructEqualityComparer Instance = new();

    public bool Equals(ConstructorStruct x, ConstructorStruct y)
    {
        return x.Class == y.Class
            && x.Members.SequenceEqual(y.Members)
            && x.WrapperClasses.SequenceEqual(y.WrapperClasses);
    }

    public int GetHashCode(ConstructorStruct obj)
    {
        throw new NotImplementedException();
    }
}
