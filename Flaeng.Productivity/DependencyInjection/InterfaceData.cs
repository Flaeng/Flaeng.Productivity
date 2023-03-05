namespace Flaeng.Productivity;

internal record struct InterfaceData
(
    ClassDeclarationSyntax? Class,
    ImmutableDictionary<MemberDeclarationSyntax, ISymbol> Members,
    ImmutableDictionary<MethodDeclarationSyntax, IMethodSymbol> Methods,
    ImmutableArray<string> InterfaceNames,
    ImmutableArray<WrapperClassData> WrapperClasses
);
