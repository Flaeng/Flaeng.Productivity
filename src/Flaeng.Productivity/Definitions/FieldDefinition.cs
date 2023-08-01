namespace Flaeng.Productivity.Definitions;

internal struct FieldDefinition : IMemberDefinition, IHasDefaultValue, IHasPrettyName
{
    public Visibility Visibility { get; }
    public bool IsStatic { get; }
    public string Type { get; }
    public string Name { get; }
    public string? DefaultValue { get; }

    public FieldDefinition(Visibility visibility, bool isStatic, string type, string name, string? defaultValue)
    {
        this.Visibility = visibility;
        this.IsStatic = isStatic;
        this.Type = type;
        this.Name = name;
        this.DefaultValue = defaultValue;
    }

    public string GetPrettyName()
    {
        var name = Name.TrimStart('_');
        return Char.ToLower(name[0]) + name.Substring(1);
    }

    public static FieldDefinition Parse(IFieldSymbol symbol, CancellationToken ct)
    {
        var visibility = MemberDefinitions.GetVisibilityFromAccessibility(symbol.DeclaredAccessibility);
        var isStatic = symbol.IsStatic;
        var defaultValue = MemberDefinitions.GetDefaultValue(symbol, ct);
        var type = MemberDefinitions.FormatType(symbol.Type);
        var name = symbol.Name;

        return new FieldDefinition(
            visibility,
            isStatic,
            type,
            name,
            defaultValue
        );
    }

    internal static FieldDefinition? Parse(FieldDeclarationSyntax field)
    {
        Visibility visibility = Visibility.Default;
        bool isStatic = false;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in field.ChildNodesAndTokens())
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
                case (int)SyntaxKind.VariableDeclaration:
                    MemberDefinitions.GetTypeAndName(nodeToken.AsNode(), out type, out name, out defaultValue);
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
}
