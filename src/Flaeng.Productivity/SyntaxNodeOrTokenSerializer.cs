namespace Flaeng.Productivity;

internal class SyntaxNodeOrTokenSerializer
{
    public void GetTypeAndName(
        SyntaxNode? syntaxNode,
        out string? type,
        out string? name,
        out string? defaultValue
        )
    {
        type = null;
        name = null;
        defaultValue = null;
        if (syntaxNode is null)
            return;

        foreach (var child in syntaxNode.ChildNodes())
        {
            switch (child.RawKind)
            {
                case (int)SyntaxKind.ArrayType:
                case (int)SyntaxKind.PredefinedType:
                case (int)SyntaxKind.QualifiedName:
                case (int)SyntaxKind.IdentifierName:
                case (int)SyntaxKind.GenericName:
                    type = child.ToString();
                    break;
                case (int)SyntaxKind.VariableDeclarator:
                    GetNameAndDefaultValueFromVariableDeclarator(child, ref name, ref defaultValue);
                    break;
            }
        }
    }

    public static void GetNameAndDefaultValueFromVariableDeclarator(SyntaxNode child, ref string? name, ref string? defaultValue)
    {
        foreach (var varDeclChild in child.ChildNodesAndTokens())
        {
            switch (varDeclChild.RawKind)
            {
                case (int)SyntaxKind.IdentifierToken:
                    name = varDeclChild.ToString();
                    break;
                case (int)SyntaxKind.EqualsValueClause:
                    defaultValue = varDeclChild.ToString();
                    break;
            }
        }
    }

    public bool TryGetReturnType(SyntaxNodeOrToken nodeToken, out string tmp_returnType)
    {
        tmp_returnType = String.Empty;
        switch (nodeToken.RawKind)
        {
            case (int)SyntaxKind.ArrayType:
            case (int)SyntaxKind.PredefinedType:
            case (int)SyntaxKind.QualifiedName:
            case (int)SyntaxKind.IdentifierName:
            case (int)SyntaxKind.GenericName:
                tmp_returnType = nodeToken.ToString();
                return true;
        }
        return false;
    }

    public bool TryGetVisibility(SyntaxNodeOrToken nodeOrToken, out Visibility visibility)
    {
        visibility = default;
        switch (nodeOrToken.RawKind)
        {
            case (int)SyntaxKind.PublicKeyword:
                visibility = Visibility.Public;
                return true;
            case (int)SyntaxKind.InternalKeyword:
                visibility = Visibility.Internal;
                return true;
            case (int)SyntaxKind.ProtectedKeyword:
                visibility = Visibility.Protected;
                return true;
            case (int)SyntaxKind.PrivateKeyword:
                visibility = Visibility.Private;
                return true;
        }
        return false;
    }

    public bool TryGetParameterKind(SyntaxNodeOrToken nodeOrToken, out string parameterKind)
    {
        parameterKind = String.Empty;
        switch (nodeOrToken.RawKind)
        {
            case (int)SyntaxKind.OutKeyword:
                parameterKind = "out";
                return true;
            case (int)SyntaxKind.RefKeyword:
                parameterKind = "ref";
                return true;
            case (int)SyntaxKind.ParamsKeyword:
                parameterKind = "params";
                return true;
            case (int)SyntaxKind.InKeyword:
                parameterKind = "in";
                return true;
        }
        return false;
    }

    public bool TryGetName(SyntaxNodeOrToken nodeOrToken, out string name)
    {
        name = String.Empty;
        if (nodeOrToken.RawKind != (int)SyntaxKind.IdentifierToken)
            return false;

        name = nodeOrToken.ToString();
        return true;
    }

    public bool TryGetDefaultValue(SyntaxNodeOrToken nodeOrToken, out string defaultValue)
    {
        defaultValue = String.Empty;
        switch (nodeOrToken.RawKind)
        {
            case (int)SyntaxKind.EqualsValueClause:
                if (nodeOrToken.AsNode() is not EqualsValueClauseSyntax evc)
                    return false;
                defaultValue = evc.ChildNodes().First().ToString();
                return true;
        }
        return false;
    }

    public void GetAccessorVisiblity(AccessorListSyntax als, ref Visibility? getterVisibility, ref Visibility? setterVisibility)
    {
        foreach (var alsChild in als.ChildNodes())
        {
            switch (alsChild.RawKind)
            {
                case (int)SyntaxKind.GetAccessorDeclaration:
                    getterVisibility = GetAccessorVisibility(alsChild);
                    break;
                case (int)SyntaxKind.SetAccessorDeclaration:
                    setterVisibility = GetAccessorVisibility(alsChild);
                    break;
                case (int)SyntaxKind.InitAccessorDeclaration:
                    setterVisibility = Visibility.Init;
                    break;
            }
        }
    }

    public static Visibility GetAccessorVisibility(SyntaxNode node)
    {
        foreach (var token in node.ChildTokens())
        {
            switch (token.RawKind)
            {
                case (int)SyntaxKind.ProtectedKeyword:
                    return Visibility.Protected;
                case (int)SyntaxKind.InternalKeyword:
                    return Visibility.Internal;
                case (int)SyntaxKind.PrivateKeyword:
                    return Visibility.Private;
            }
        }
        return Visibility.Default;
    }
}
