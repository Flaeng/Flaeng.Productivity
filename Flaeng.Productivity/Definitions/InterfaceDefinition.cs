namespace Flaeng.Productivity.Definitions;

internal record struct InterfaceDefinition
{
    public Visibility Visibility { get; }
    public bool IsPartial { get; }
    public string Name { get; }
    public ImmutableArray<string> TypeArguments { get; }
    public ImmutableArray<IMemberDefinition> Members { get; }

    private bool isInitialized = false;

    public InterfaceDefinition(
        Visibility visibility,
        bool isPartial, 
        string name,
        ImmutableArray<string> typeArguments,
        ImmutableArray<IMemberDefinition> members
    )
    {
        isInitialized = true;
        this.Visibility = visibility;
        this.IsPartial = isPartial;
        this.Name = name;
        this.TypeArguments = typeArguments;
        this.Members = members;
    }

    public bool IsDefault() => isInitialized == false;

    internal static InterfaceDefinition Parse(INamedTypeSymbol symbol, CancellationToken ct)
    {
        return new InterfaceDefinition(
            visibility: MemberDefinitions.GetVisibilityFromAccessibility(symbol.DeclaredAccessibility),
            isPartial: MemberDefinitions.IsPartial(symbol, ct),
            name: symbol.Name,
            typeArguments: symbol.TypeArguments.Select(arg => arg.Name).ToImmutableArray(),
            members: symbol.GetMembers().Select(x => MemberDefinitions.Parse(x, ct)).OfType<IMemberDefinition>().ToImmutableArray()
        );
    }
}
