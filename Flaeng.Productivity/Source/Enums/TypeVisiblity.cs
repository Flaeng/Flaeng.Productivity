namespace Flaeng.Productivity;

public enum TypeVisibility
{
    Default,
    Public,
    Internal,
    Private
}
internal static class TypeVisiblityHelper
{
    public static TypeVisibility GetFromTokens(IEnumerable<SyntaxToken> tokens)
    {
        foreach (var item in tokens)
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
