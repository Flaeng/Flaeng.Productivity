namespace Flaeng.Productivity;

internal record struct InterfaceData
(
    ClassDeclarationSyntax? Class,
    ImmutableArray<MemberDeclarationSyntax> Members,
    ImmutableArray<MethodDeclarationSyntax> Methods,
    ImmutableArray<string> InterfaceNames,
    ImmutableArray<WrapperClassData> WrapperClasses
);
