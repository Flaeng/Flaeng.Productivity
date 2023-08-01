namespace Flaeng.Productivity;

internal sealed partial class SyntaxSerializer
{
    public IMemberDefinition? Deserialize(MemberDeclarationSyntax syntax)
    {
        return syntax switch
        {
            FieldDeclarationSyntax field => Deserialize(field),
            PropertyDeclarationSyntax prop => Deserialize(prop),
            MethodDeclarationSyntax method => Deserialize(method),
            // IParameterSymbol parameter => MethodParameterDefinition.Parse(parameter),
            _ => default
        };
    }

    public ClassDefinition? Deserialize(ClassDeclarationSyntax syntax)
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
            if (TryGetName(nodeToken, out var tmp_name))
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
        if (name is null)
            return default;

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

    public MethodDefinition? Deserialize(MethodDeclarationSyntax method)
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
            if (TryGetName(nodeToken, out var tmp_name))
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
                        .Select(Deserialize)
                        .OfType<MethodParameterDefinition>()
                        .ToImmutableArray();
                    break;
            }
        }
        if (returnType is null || name is null)
            return default;

        return new MethodDefinition(visibility, isStatic, returnType, name, parameters);
    }

    public MethodParameterDefinition? Deserialize(ParameterSyntax parameter)
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
            if (TryGetName(nodeToken, out var tmp_name))
            {
                name = tmp_name;
                continue;
            }

            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.VariableDeclaration:
                    GetTypeAndName(nodeToken.AsNode(), out type, out name, out _);
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

    public FieldDefinition? Deserialize(FieldDeclarationSyntax field)
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

    public PropertyDefinition? Deserialize(PropertyDeclarationSyntax prop)
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
            if (TryGetName(nodeToken, out var tmp_name))
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
}
