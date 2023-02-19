namespace Flaeng.Productivity;

public static class SyntaxNodeExtensions
{
    public static IEnumerable<SyntaxNode> GetParents(this SyntaxNode node)
    {
        var parent = node.Parent;
        while (parent != null)
        {
            yield return parent;
            parent = parent.Parent;
        }
    }

    public static TypeVisibility GetTypeVisiblity(this SyntaxNode node)
    {
        foreach (var item in node.ChildTokens())
        {
            switch (item.Kind())
            {
                case SyntaxKind.PublicKeyword:
                    return TypeVisibility.Public;

                case SyntaxKind.InternalKeyword:
                    return TypeVisibility.Internal;

                case SyntaxKind.PrivateKeyword:
                    return TypeVisibility.Private;
            }
        }
        return TypeVisibility.Default;
    }
}
