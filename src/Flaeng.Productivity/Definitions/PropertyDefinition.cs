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
}
