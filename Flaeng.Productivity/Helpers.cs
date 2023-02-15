using System.Text;

namespace Flaeng.Productivity;

public static class Helpers
{
    public static string GenerateFilename(ClassDeclarationSyntax cls)
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
        filename.Append('I');
        filename.Append(cls.GetClassName());
        return filename.ToString();
    }
}
