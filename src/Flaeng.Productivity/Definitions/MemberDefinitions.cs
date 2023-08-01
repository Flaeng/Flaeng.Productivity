namespace Flaeng.Productivity.Definitions;

internal static class MemberDefinitions
{
    public static IMemberDefinition? Parse(ISymbol symbol, CancellationToken ct)
    {
        return symbol switch
        {
            IMethodSymbol method => MethodDefinition.Parse(method),
            IPropertySymbol prop => PropertyDefinition.Parse(prop, ct),
            IFieldSymbol field => FieldDefinition.Parse(field, ct),
            // IParameterSymbol parameter => MethodParameterDefinition.Parse(parameter),
            _ => default
        };
    }

    public static bool IsPartial(INamedTypeSymbol symbol, CancellationToken ct)
    {
        if (symbol.DeclaringSyntaxReferences.Length != 1)
            return true;

        return symbol.DeclaringSyntaxReferences
            .Select(x => x.GetSyntax(ct))
            .Any(x => x.ChildNodesAndTokens().Any(nt => nt.IsKind(SyntaxKind.PartialKeyword)));
    }

    public static string FormatType(ISymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arrayType)
            return $"{FormatType(arrayType.ElementType)}[]";

        if (symbol is not INamedTypeSymbol namedType)
            throw new Exception("GetTypeStringFromSymbol was passed not INamedTypeSymbol");

        if (namedType.IsGenericType == false)
            return FormatTypeName(namedType);

        List<string> typeArgs = new();
        foreach (var typeArg in namedType.TypeArguments)
        {
            var typeString = (typeArg is ITypeParameterSymbol tps)
                ? tps.Name
                : FormatType(typeArg);
            typeArgs.Add(typeString);
        }

        return $"{FormatTypeName(namedType)}<{(String.Join(", ", typeArgs))}>";
    }

    private static string FormatTypeName(INamedTypeSymbol symbol)
        => $"global::{symbol.ContainingNamespace}.{symbol.Name}";

    public static Visibility GetVisibilityFromAccessibility(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => Visibility.Public,
            Accessibility.Internal => Visibility.Internal,
            Accessibility.Private => Visibility.Private,
            Accessibility.Protected => Visibility.Protected,
            _ => Visibility.Default
        };
    }

    public static string? GetDefaultValue(ISymbol symbol, CancellationToken ct)
    {
        if (symbol is IParameterSymbol param)
            return MethodParameterDefinition.GetDefaultValue(param);

        return symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(ct) switch
        {
            PropertyDeclarationSyntax property => property.Initializer?.Value.ToString(),
            VariableDeclaratorSyntax variable => variable.Initializer?.Value.ToString(),
            _ => null
        };
    }

}
