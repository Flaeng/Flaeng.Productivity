namespace Flaeng.Productivity;

internal sealed class SyntaxSerializer
{
    public IMemberDefinition? DeserializeMember(MemberDeclarationSyntax syntax)
    {
        return syntax switch
        {
            FieldDeclarationSyntax field => DeserializeField(field),
            PropertyDeclarationSyntax prop => DeserializeProperty(prop),
            MethodDeclarationSyntax method => DeserializeMethod(method),
            // IParameterSymbol parameter => MethodParameterDefinition.Parse(parameter),
            _ => default
        };
    }

    public ClassDefinition DeserializeClass(ClassDeclarationSyntax syntax)
    {
        Visibility visibility = Visibility.Default;
        bool isStatic = false, isPartial = false;
        string? name = null;

        foreach (var nodeToken in syntax.ChildNodesAndTokens())
        {
            if (TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.PartialKeyword:
                    isPartial = true;
                    break;
                case (int)SyntaxKind.IdentifierToken:
                    name = nodeToken.ToString();
                    break;
            }
        }

        return new ClassDefinition(
            visibility,
            isStatic,
            isPartial,
            name,
            ImmutableArray<string>.Empty, // TODO
            ImmutableArray<InterfaceDefinition>.Empty, // TODO
            ImmutableArray<MethodDefinition>.Empty // TODO
        );
    }

    public MethodDefinition? DeserializeMethod(MethodDeclarationSyntax method)
    {
        Visibility visibility = Visibility.Default;
        bool isStatic = false;
        string? returnType = null, name = null;
        ImmutableArray<MethodParameterDefinition> parameters = default;

        foreach (var nodeToken in method.ChildNodesAndTokens())
        {
            if (TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }
            if (TryGetReturnType(nodeToken, out var tmp_returnType))
            {
                returnType = tmp_returnType;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.IdentifierToken:
                    name = nodeToken.ToString();
                    break;
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.ParameterList:
                    var pl = nodeToken.AsNode();
                    if (pl is null)
                        continue;

                    parameters = pl.ChildNodes()
                        .OfType<ParameterSyntax>()
                        .Select(DeserializeMethodParameter)
                        .ToImmutableArray();
                    break;
            }
        }
        if (returnType is null || name is null)
            return default;

        return new MethodDefinition(
            visibility,
            isStatic,
            returnType,
            name,
            parameters.ToImmutableArray()
        );
    }

    public MethodParameterDefinition DeserializeMethodParameter(ParameterSyntax parameter)
    {
        string? parameterKind = null;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in parameter.ChildNodesAndTokens())
        {
            if (TryGetDefaultValue(nodeToken, out var tmp_defaultValue))
            {
                defaultValue = tmp_defaultValue;
                continue;
            }
            if (TryGetParameterKind(nodeToken, out var tmp_parameterKind))
            {
                parameterKind = tmp_parameterKind;
                continue;
            }
            if (TryGetReturnType(nodeToken, out var tmp_type))
            {
                type = tmp_type;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.VariableDeclaration:
                    GetTypeAndName(nodeToken.AsNode(), out type, out name, out _);
                    break;
                case (int)SyntaxKind.IdentifierToken:
                    name = nodeToken.ToString();
                    break;
            }
        }
        if (type is null || name is null)
            return default;

        return new MethodParameterDefinition(
            parameterKind,
            type,
            name,
            defaultValue
        );
    }

    public FieldDefinition? DeserializeField(FieldDeclarationSyntax field)
    {
        Visibility visibility = Visibility.Default;
        bool isStatic = false;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in field.ChildNodesAndTokens())
        {
            if (TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.VariableDeclaration:
                    GetTypeAndName(nodeToken.AsNode(), out type, out name, out defaultValue);
                    break;
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
            }
        }
        if (type is null || name is null)
            return default;

        return new FieldDefinition(
            visibility,
            isStatic,
            type,
            name,
            defaultValue
        );
    }

    public PropertyDefinition? DeserializeProperty(PropertyDeclarationSyntax prop)
    {
        Visibility visibility = Visibility.Default;
        Visibility? getterVisibility = null, setterVisibility = null;
        bool isStatic = false;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in prop.ChildNodesAndTokens())
        {
            if (TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }
            if (TryGetReturnType(nodeToken, out var tmp_returnType))
            {
                type = tmp_returnType;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.IdentifierToken:
                    name = nodeToken.ToString();
                    break;
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.AccessorList:
                    if (nodeToken.AsNode() is not AccessorListSyntax als)
                        continue;
                    GetAccessorVisiblity(als, ref getterVisibility, ref setterVisibility);
                    break;
            }
        }
        return new PropertyDefinition(
                visibility,
                isStatic,
                type,
                name,
                getterVisibility,
                setterVisibility,
                defaultValue
            );
    }

    private static void GetTypeAndName(
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

    private static void GetNameAndDefaultValueFromVariableDeclarator(SyntaxNode child, ref string? name, ref string? defaultValue)
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

    private bool TryGetReturnType(SyntaxNodeOrToken nodeToken, out string tmp_returnType)
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

    private bool TryGetVisibility(SyntaxNodeOrToken nodeOrToken, out Visibility visibility)
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

    private bool TryGetParameterKind(SyntaxNodeOrToken nodeOrToken, out string parameterKind)
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

    private bool TryGetDefaultValue(SyntaxNodeOrToken nodeOrToken, out string defaultValue)
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

    private static void GetAccessorVisiblity(AccessorListSyntax als, ref Visibility? getterVisibility, ref Visibility? setterVisibility)
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

    private static Visibility GetAccessorVisibility(SyntaxNode node)
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
