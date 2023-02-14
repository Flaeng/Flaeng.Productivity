using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class ClassDeclarationSyntaxExtensions
{
    public static string GetClassName(this ClassDeclarationSyntax node)
        => node.ChildTokens()
            .Where(x => x.IsKind(SyntaxKind.IdentifierToken))
            .First()
            .ToString();
}