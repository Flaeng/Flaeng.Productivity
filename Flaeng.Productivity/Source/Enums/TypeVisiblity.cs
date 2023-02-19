using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Flaeng.Productivity;

public enum TypeVisiblity
{
    Default,
    Public,
    Internal,
    Private
}
internal static class TypeVisiblityHelper
{
    public static TypeVisiblity GetFromTokens(IEnumerable<SyntaxToken> tokens)
    {
        foreach (var item in tokens)
        {
            switch (item.Kind())
            {
                case SyntaxKind.PublicKeyword:
                    return TypeVisiblity.Public;

                case SyntaxKind.InternalKeyword:
                    return TypeVisiblity.Internal;

                case SyntaxKind.PrivateKeyword:
                    return TypeVisiblity.Private;
            }
        }
        return TypeVisiblity.Default;
    }
}
