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

    public static MethodParameterDefinition Parse(ParameterSyntax parameter)
    {
        string? parameterKind = null;
        string? type = null, name = null, defaultValue = null;

        foreach (var nodeToken in parameter.ChildNodesAndTokens())
        {
            switch (nodeToken.RawKind)
            {
                case (int)SyntaxKind.OutKeyword:
                    parameterKind = "out";
                    break;
                case (int)SyntaxKind.EqualsValueClause:
                    if (nodeToken.AsNode() is not EqualsValueClauseSyntax evc)
                        continue;
                    defaultValue = evc.ChildNodes().First().ToString();
                    break;
                case (int)SyntaxKind.RefKeyword:
                    parameterKind = "ref";
                    break;
                case (int)SyntaxKind.ParamsKeyword:
                    parameterKind = "params";
                    break;
                case (int)SyntaxKind.InKeyword:
                    parameterKind = "in";
                    break;
                case (int)SyntaxKind.VariableDeclaration:
                    MemberDefinitions.GetTypeAndName(nodeToken.AsNode(), out type, out name, out _);
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
