using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Flaeng.Productivity.DependencyInjection;

internal record struct ConstructorStruct
(
    ClassDeclarationSyntax? Class,
    ImmutableArray<MemberDeclarationSyntax> Members
);

internal class ConstructorEqualityComparer : IEqualityComparer<ConstructorStruct>
{
    public static ConstructorEqualityComparer Instance = new();

    public bool Equals(ConstructorStruct x, ConstructorStruct y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return x.Class == y.Class
            && x.Members.SequenceEqual(y.Members);
    }

    public int GetHashCode(ConstructorStruct obj)
    {
        throw new NotImplementedException();
    }
}
