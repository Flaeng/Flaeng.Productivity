namespace Flaeng.Productivity.Definitions;

internal record struct MethodParameterDefinition
{
    public string? ParameterKind { get; }
    public string? Type { get; }
    public string? Name { get; }
    public string? DefaultValue { get; }

    public MethodParameterDefinition(
        string? parameterKind,
        string? type,
        string? name,
        string? defaultValue
    )
    {
        this.ParameterKind = parameterKind;
        this.Type = type;
        this.Name = name;
        this.DefaultValue = defaultValue;
    }

    public static MethodParameterDefinition Parse(IParameterSymbol symbol)
    {
        var parameterKind = symbol.RefKind switch
        {
            RefKind.Ref => "ref",
            RefKind.Out => "out",
            RefKind.In => "in",
            _ => null,
        };
        var type = FormatType(symbol);
        var name = symbol.Name;
        var defaultValue = GetDefaultValue(symbol);

        return new MethodParameterDefinition(
            parameterKind,
            type,
            name,
            defaultValue
        );
    }

    private static string FormatType(IParameterSymbol symbol)
    {
        var type = MemberDefinitions.FormatType(symbol.Type);
        return symbol.IsParams ? $"params {type}" : type;
    }

    public static string? GetDefaultValue(IParameterSymbol symbol)
    {
        if (symbol.HasExplicitDefaultValue == false)
            return null;

        return symbol.ExplicitDefaultValue is string strValue
            ? $"\"{strValue}\""
            : symbol.ExplicitDefaultValue?.ToString() ?? "default";
    }
}
