namespace Flaeng.Productivity;

public static class Helpers
{
    public static string GenerateFilename(ClassDeclarationSyntax cls, bool isInterface)
    {
        Stack<string> names = new(8);

        SyntaxNode? node = cls.Parent;
        while (node != null)
        {
            if (node is BaseNamespaceDeclarationSyntax nNode)
                names.Push(nNode.Name.ToString());
            else if (node is ClassDeclarationSyntax cds)
                names.Push(cds.GetClassName());
            node = node.Parent;
        }

        string filename = isInterface
            ? $"I{cls.GetClassName()}"
            : cls.GetClassName();

        return $"{String.Join(".", names)}.{filename}";
    }
}
