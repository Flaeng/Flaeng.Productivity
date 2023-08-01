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
}
