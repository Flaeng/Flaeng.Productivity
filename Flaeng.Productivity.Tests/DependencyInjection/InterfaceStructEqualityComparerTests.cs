using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flaeng.Productivity.Tests.DependencyInjection;

public class InterfaceStructEqualityComparerTests
{
    readonly InterfaceDataEqualityComparer Instance = InterfaceDataEqualityComparer.Instance;

    [Fact]
    public void Returns_false_when_compared_with_null()
    {
        var item = new InterfaceData(
            null,
            new ImmutableArray<MemberDeclarationSyntax>(),
            new ImmutableArray<MethodDeclarationSyntax>(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassData>());
        Assert.False(item.Equals(null));
    }

    [Fact]
    public void Returns_true_when_compared_with_itself()
    {
        var item = new InterfaceData(
            null,
            new ImmutableArray<MemberDeclarationSyntax>(),
            new ImmutableArray<MethodDeclarationSyntax>(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassData>());
        Assert.True(item.Equals(item));
    }

    [Fact]
    public void Returns_true_when_compared_with_copy_of_itself()
    {
        var item1 = new InterfaceData(
            null,
            new ImmutableArray<MemberDeclarationSyntax>(),
            new ImmutableArray<MethodDeclarationSyntax>(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassData>());
        var item2 = new InterfaceData(
            null,
            new ImmutableArray<MemberDeclarationSyntax>(),
            new ImmutableArray<MethodDeclarationSyntax>(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassData>());
        Assert.True(item1.Equals(item2));
    }

    [Fact]
    public void Returns_false_when_first_param_is_null()
    {
        var item = new InterfaceData(
            null,
            new ImmutableArray<MemberDeclarationSyntax>(),
            new ImmutableArray<MethodDeclarationSyntax>(),
            new ImmutableArray<string>(),
            new ImmutableArray<WrapperClassData>());
        Assert.Throws<NotImplementedException>(() =>
            Instance.GetHashCode(item)
        );
    }
}
