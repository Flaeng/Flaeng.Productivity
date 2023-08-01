namespace Flaeng.Productivity;

internal sealed class SyntaxSerializer
{
    private readonly SyntaxNodeOrTokenSerializer nodeOrTokenSerializer = new();

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
            if (nodeOrTokenSerializer.TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetName(nodeToken, out var tmp_name))
            {
                name = tmp_name;
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
            if (nodeOrTokenSerializer.TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetReturnType(nodeToken, out var tmp_returnType))
            {
                returnType = tmp_returnType;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetName(nodeToken, out var tmp_name))
            {
                name = tmp_name;
                continue;
            }

            switch (nodeToken.RawKind)
            {
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

        return new MethodDefinition(visibility, isStatic, returnType, name, parameters);
    }

    public MethodParameterDefinition DeserializeMethodParameter(ParameterSyntax parameter)
    {
        string? parameterKind = null;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in parameter.ChildNodesAndTokens())
        {
            if (nodeOrTokenSerializer.TryGetDefaultValue(nodeToken, out var tmp_defaultValue))
            {
                defaultValue = tmp_defaultValue;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetParameterKind(nodeToken, out var tmp_parameterKind))
            {
                parameterKind = tmp_parameterKind;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetReturnType(nodeToken, out var tmp_type))
            {
                type = tmp_type;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetName(nodeToken, out var tmp_name))
            {
                name = tmp_name;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.VariableDeclaration:
                    nodeOrTokenSerializer.GetTypeAndName(nodeToken.AsNode(), out type, out name, out _);
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
            if (nodeOrTokenSerializer.TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.VariableDeclaration:
                    nodeOrTokenSerializer.GetTypeAndName(nodeToken.AsNode(), out type, out name, out defaultValue);
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
            if (nodeOrTokenSerializer.TryGetVisibility(nodeToken, out var tmp_visibility))
            {
                visibility = tmp_visibility;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetReturnType(nodeToken, out var tmp_returnType))
            {
                type = tmp_returnType;
                continue;
            }
            if (nodeOrTokenSerializer.TryGetName(nodeToken, out var tmp_name))
            {
                name = tmp_name;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.AccessorList:
                    if (nodeToken.AsNode() is not AccessorListSyntax als)
                        continue;
                    nodeOrTokenSerializer.GetAccessorVisiblity(als, ref getterVisibility, ref setterVisibility);
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
}
