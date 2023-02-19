using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flaeng.Productivity;

public static class ClassDeclarationSyntaxExtensions
{
    public static string GetClassName(this ClassDeclarationSyntax cds)
    {
        return cds.ChildTokens()
            .First(x => x.IsKind(SyntaxKind.IdentifierToken)).Text;
    }
}
