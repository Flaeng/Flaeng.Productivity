namespace Flaeng.Productivity;

internal record struct ConstructorData
(
    ClassDeclarationSyntax? Class,
    INamedTypeSymbol? ClassSymbol,
    ImmutableArray<ISymbol> Members,
    ImmutableArray<WrapperClassData> WrapperClasses
);
