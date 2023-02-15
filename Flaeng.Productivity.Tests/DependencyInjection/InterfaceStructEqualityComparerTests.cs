using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flaeng.Productivity.Tests.DependencyInjection;

public class InterfaceStructEqualityComparerTests
{
    readonly InterfaceStructEqualityComparer Instance = InterfaceStructEqualityComparer.Instance;

    [Fact]
    public void Returns_false_when_compared_with_null()
    {
        var item = new InterfaceStruct(null, new ImmutableArray<MemberDeclarationSyntax>(), new ImmutableArray<MethodDeclarationSyntax>());
        Assert.False(item.Equals(null));
    }

    [Fact]
    public void Returns_true_when_compared_with_itself()
    {
        var item = new InterfaceStruct(null, new ImmutableArray<MemberDeclarationSyntax>(), new ImmutableArray<MethodDeclarationSyntax>());
        Assert.True(item.Equals(item));
    }

    [Fact]
    public void Returns_true_when_compared_with_copy_of_itself()
    {
        var item1 = new InterfaceStruct(null, new ImmutableArray<MemberDeclarationSyntax>(), new ImmutableArray<MethodDeclarationSyntax>());
        var item2 = new InterfaceStruct(null, new ImmutableArray<MemberDeclarationSyntax>(), new ImmutableArray<MethodDeclarationSyntax>());
        Assert.True(item1.Equals(item2));
    }

    [Fact]
    public void Returns_false_when_first_param_is_null()
    {
        var item = new InterfaceStruct(null, new ImmutableArray<MemberDeclarationSyntax>(), new ImmutableArray<MethodDeclarationSyntax>());
        Assert.Throws<NotImplementedException>(() =>
            Instance.GetHashCode(item)
        );
    }
}
