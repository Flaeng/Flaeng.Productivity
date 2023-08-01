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

    internal static ClassDefinition Parse(ClassDeclarationSyntax syntax, CancellationToken ct)
    {
        Visibility visibility = Visibility.Default;
        bool isStatic = false, isPartial = false;
        string? name = null;
        List<string> typeArguments = new();
        List<InterfaceDefinition> interfaces = new();
        List<MethodDefinition> constructors = new();

        foreach (var child in syntax.ChildNodesAndTokens())
        {
            switch (child.RawKind)
            {
                case (int)SyntaxKind.PublicKeyword:
                    visibility = Visibility.Public;
                    break;
                case (int)SyntaxKind.InternalKeyword:
                    visibility = Visibility.Internal;
                    break;
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.PartialKeyword:
                    isPartial = true;
                    break;
                case (int)SyntaxKind.IdentifierToken:
                    name = child.ToString();
                    break;
            }
        }

        return new ClassDefinition(
            visibility,
            isStatic,
            isPartial,
            name,
            typeArguments.ToImmutableArray(),
            interfaces.ToImmutableArray(),
            constructors.ToImmutableArray()
        );
    }

}
