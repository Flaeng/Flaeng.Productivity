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

    public static MethodDefinition Parse(MethodDeclarationSyntax method)
    {
        Visibility visibility = Visibility.Default;
        bool isStatic = false;
        string? returnType = null, name = null;
        ImmutableArray<MethodParameterDefinition> parameters = default;

        foreach (var child in method.ChildNodesAndTokens())
        {
            switch (child.RawKind)
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
                    returnType = child.ToString();
                    break;
                case (int)SyntaxKind.IdentifierToken:
                    name = child.ToString();
                    break;
                case (int)SyntaxKind.StaticKeyword:
                    isStatic = true;
                    break;
                case (int)SyntaxKind.ParameterList:
                    var pl = child.AsNode();
                    if (pl is null)
                        continue;

                    parameters = pl.ChildNodes()
                        .OfType<ParameterSyntax>()
                        .Select(x => MethodParameterDefinition.Parse(x))
                        .ToImmutableArray();
                    break;
            }
        }
        if (returnType is null || name is null)
#if DEBUG
            throw new Exception("Failed to find type or name");
#else
            return default;
#endif

        return new MethodDefinition(
            visibility,
            isStatic,
            returnType,
            name,
            parameters.ToImmutableArray()
        );
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
