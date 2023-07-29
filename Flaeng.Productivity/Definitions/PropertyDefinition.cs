namespace Flaeng.Productivity.Definitions;

internal struct PropertyDefinition : IMemberDefinition, IHasDefaultValue, IHasPrettyName
{
    public Visibility Visibility { get; }
    public bool IsStatic { get; }
    public string? Type { get; }
    public string? Name { get; }
    public Visibility? GetterVisibility { get; }
    public Visibility? SetterVisibility { get; }
    public string? DefaultValue { get; }

    public PropertyDefinition(
        Visibility visibility,
        bool isStatic,
        string? type,
        string? name,
        Visibility? getterVisibility,
        Visibility? setterVisibility,
        string? defaultValue
    )
    {
        this.Visibility = visibility;
        this.IsStatic = isStatic;
        this.Type = type;
        this.Name = name;
        this.GetterVisibility = getterVisibility;
        this.SetterVisibility = setterVisibility;
        this.DefaultValue = defaultValue;
    }

    public string? GetPrettyName()
    {
        if (Name is null)
            return null;

        var name = Name.TrimStart('_');
        return Char.ToLower(name[0]) + name.Substring(1);
    }

    public static PropertyDefinition Parse(IPropertySymbol symbol, CancellationToken ct)
    {
        var visibility = MemberDefinitions.GetVisibilityFromAccessibility(symbol.DeclaredAccessibility);
        var isStatic = symbol.IsStatic;
        var type = MemberDefinitions.FormatType(symbol.Type);
        var name = symbol.Name;
        string? defaultValue = MemberDefinitions.GetDefaultValue(symbol, ct);
        
        Visibility? getterVisibility = symbol.GetMethod is null
                            ? null
                            : MemberDefinitions.GetVisibilityFromAccessibility(symbol.GetMethod.DeclaredAccessibility);

        Visibility? setterVisibility = symbol.SetMethod is null
                            ? null
                            : MemberDefinitions.GetVisibilityFromAccessibility(symbol.SetMethod.DeclaredAccessibility);

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

    internal static PropertyDefinition? Parse(PropertyDeclarationSyntax prop)
    {
        Visibility visibility = Visibility.Default;
        Visibility? getterVisibility = null, setterVisibility = null;
        bool isStatic = false;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in prop.ChildNodesAndTokens())
        {
            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.PublicKeyword:
                    visibility = Visibility.Public;
                    break;
                case (int)SyntaxKind.InternalKeyword:
                    visibility = Visibility.Internal;
                    break;
                case (int)SyntaxKind.ProtectedKeyword:
                    visibility = Visibility.Protected;
                    break;
                case (int)SyntaxKind.PrivateKeyword:
                    visibility = Visibility.Private;
                    break;
                case (int)SyntaxKind.ArrayType:
                case (int)SyntaxKind.PredefinedType:
                case (int)SyntaxKind.QualifiedName:
                case (int)SyntaxKind.IdentifierName:
                case (int)SyntaxKind.GenericName:
                    type = nodeToken.ToString();
                    break;
                case (int)SyntaxKind.IdentifierToken:
                    name = nodeToken.ToString();
                    break;
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.EqualsValueClause:
                    if (nodeToken.AsNode() is not EqualsValueClauseSyntax evc)
                        continue;
                    defaultValue = evc.ChildNodes().First().ToString();
                    break;
                case (int)SyntaxKind.AccessorList:
                    if (nodeToken.AsNode() is not AccessorListSyntax als)
                        continue;

                    foreach (var alsChild in als.ChildNodes())
                    {
                        switch (alsChild.RawKind)
                        {
                            case (int)SyntaxKind.GetAccessorDeclaration:
                                getterVisibility = GetVisibility(alsChild);
                                break;
                            case (int)SyntaxKind.SetAccessorDeclaration:
                                setterVisibility = GetVisibility(alsChild);
                                break;
                            case (int)SyntaxKind.InitAccessorDeclaration:
                                setterVisibility = Visibility.Init;
                                break;
                        }
                    }
                    break;
            }
        }
        if (type is null || name is null)
#if DEBUG
            throw new Exception("Failed to find type or name");
#else
            return default;
#endif

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

    private static Visibility GetVisibility(SyntaxNode node)
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
