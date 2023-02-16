using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flaeng.Productivity;

public static class Helpers
{
    public static string GenerateFilename(ClassDeclarationSyntax cls, bool isInterface)
    {
        StringBuilder filename = new();
        if (cls.Parent is NamespaceDeclarationSyntax nds)
        {
            filename.Append(nds.Name);
            filename.Append('.');
        }
        else if (cls.Parent is FileScopedNamespaceDeclarationSyntax fsnds)
        {
            filename.Append(fsnds.Name);
            filename.Append('.');
        }
        if (isInterface)
            filename.Append('I');

        var className = cls.ChildTokens().First(x => x.IsKind(SyntaxKind.IdentifierToken));
        filename.Append(className);
        return filename.ToString();
    }
}
