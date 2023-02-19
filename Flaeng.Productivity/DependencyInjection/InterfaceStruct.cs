using System.Collections.Immutable;

namespace Flaeng.Productivity;

internal record struct InterfaceStruct
(
    ClassDeclarationSyntax? Class,
    ImmutableArray<MemberDeclarationSyntax> Members,
    ImmutableArray<MethodDeclarationSyntax> Methods,
    ImmutableArray<string> InterfaceNames,
    ImmutableArray<WrapperClassStruct> WrapperClasses
);
