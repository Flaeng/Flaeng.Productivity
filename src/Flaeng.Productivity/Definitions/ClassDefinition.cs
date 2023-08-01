namespace Flaeng.Productivity.Definitions;

internal record struct ClassDefinition
{
    public Visibility Visibility { get; }
    public bool IsStatic { get; }
    public bool IsPartial { get; }
    public string? Name { get; }
    public ImmutableArray<MethodDefinition> Constructors { get; }
    public ImmutableArray<string> TypeArguments { get; }
    public ImmutableArray<InterfaceDefinition> Interfaces { get; set; }

    public ClassDefinition(
        Visibility visibility,
        bool isStatic,
        bool isPartial,
        string? name,
        ImmutableArray<string> typeArguments,
        ImmutableArray<InterfaceDefinition> interfaces,
        ImmutableArray<MethodDefinition> constructors
    )
    {
        this.Visibility = visibility;
        this.IsStatic = isStatic;
        this.IsPartial = isPartial;
        this.Name = name;
        this.TypeArguments = typeArguments;
        this.Interfaces = interfaces;
        this.Constructors = constructors;
    }

    public ClassDefinition WithName(string name)
        => new ClassDefinition(Visibility, IsStatic, IsPartial, name, TypeArguments, Interfaces, Constructors);

    public ClassDefinition WithIsPartial(bool isPartial)
        => new ClassDefinition(Visibility, IsStatic, isPartial, Name, TypeArguments, Interfaces, Constructors);

    internal static ClassDefinition Parse(INamedTypeSymbol symbol, CancellationToken ct)
    {
        return new ClassDefinition(
            MemberDefinitions.GetVisibilityFromAccessibility(symbol.DeclaredAccessibility),
            symbol.IsStatic,
            MemberDefinitions.IsPartial(symbol, ct),
            symbol.Name,
            symbol.TypeArguments.Select(arg => arg.Name).ToImmutableArray(),
            symbol.Interfaces.Select(x => InterfaceDefinition.Parse(x, ct)).ToImmutableArray(),
            symbol.Constructors.Select(MethodDefinition.Parse).ToImmutableArray()
        );
    }
}
