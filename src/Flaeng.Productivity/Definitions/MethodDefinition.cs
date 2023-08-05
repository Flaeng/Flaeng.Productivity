namespace Flaeng.Productivity.Definitions;

internal record struct MethodDefinition : IMemberDefinition
{
    public Visibility Visibility { get; }
    public bool IsStatic { get; }
    public string Type { get; }
    public string Name { get; }
    public ImmutableArray<MethodParameterDefinition> Parameters { get; }

    public MethodDefinition(Visibility visibility, bool isStatic, string type, string name, ImmutableArray<MethodParameterDefinition> parameters)
    {
        this.Visibility = visibility;
        this.IsStatic = isStatic;
        this.Type = type;
        this.Name = name;
        this.Parameters = parameters;
    }

    public MethodDefinition WithName(string methodName)
    {
        return new MethodDefinition(Visibility, IsStatic, Type, methodName, Parameters);
    }

    public static MethodDefinition Parse(IMethodSymbol symbol)
    {
        return new MethodDefinition(
            visibility: MemberDefinitions.GetVisibilityFromAccessibility(symbol.DeclaredAccessibility),
            isStatic: symbol.IsStatic,
            type: symbol.ReturnsVoid ? "void" : MemberDefinitions.FormatType(symbol.ReturnType),
            name: symbol.Name,
            parameters: symbol.Parameters.Select(MethodParameterDefinition.Parse).ToImmutableArray()
        );
    }
}
