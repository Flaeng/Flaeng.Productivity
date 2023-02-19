namespace Flaeng.Productivity;

internal record struct ConstructorData
(
    ClassDeclarationSyntax? Class,
    ImmutableArray<MemberDeclarationSyntax> Members,
    ImmutableArray<WrapperClassData> WrapperClasses
);
