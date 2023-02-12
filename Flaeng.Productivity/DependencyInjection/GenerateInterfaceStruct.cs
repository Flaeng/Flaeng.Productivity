
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Flaeng.Productivity;

internal record struct GenerateInterfaceStruct
(
    ClassDeclarationSyntax? Class,
    ImmutableArray<MemberDeclarationSyntax> Members,
    ImmutableArray<MethodDeclarationSyntax> Methods
);

internal class GenerateInterfaceEqualityComparer : IEqualityComparer<GenerateInterfaceStruct>
{
    public static GenerateInterfaceEqualityComparer Instance = new();

    public bool Equals(GenerateInterfaceStruct x, GenerateInterfaceStruct y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return x.Class == y.Class
            && x.Members.SequenceEqual(y.Members)
            && x.Methods.SequenceEqual(y.Methods);
    }

    public int GetHashCode(GenerateInterfaceStruct obj)
    {
        throw new NotImplementedException();
    }
}