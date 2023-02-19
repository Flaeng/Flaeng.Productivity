using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flaeng.Productivity;

public static class Helpers
{
    public static string GetClassName(ClassDeclarationSyntax cls)
    {
        return cls
            .ChildTokens()
            .First(x => x.IsKind(SyntaxKind.IdentifierToken)).Text;
    }

    public static string GenerateFilename(ClassDeclarationSyntax cls, bool isInterface)
    {
        Stack<string> names = new(8);

        SyntaxNode? node = cls.Parent;
        while (node != null)
        {
            if (node is BaseNamespaceDeclarationSyntax nNode)
                names.Push(nNode.Name.ToString());
            else if (node is ClassDeclarationSyntax cds)
                names.Push(GetClassName(cds));
            node = node.Parent;
        }

        string filename = isInterface
            ? $"I{GetClassName(cls)}"
            : GetClassName(cls);

        return $"{String.Join(".", names)}.{filename}";
    }
}
