using System.Collections.Immutable;

namespace Flaeng.Productivity.DependencyInjection;

internal record struct ConstructorStruct
(
    ClassDeclarationSyntax? Class,
    ImmutableArray<MemberDeclarationSyntax> Members
);
