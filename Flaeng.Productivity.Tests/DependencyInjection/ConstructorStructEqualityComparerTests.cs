

using System.Collections.Immutable;

using Flaeng.Productivity.DependencyInjection;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flaeng.Productivity.Tests.DependencyInjection;

public class ConstructorStructEqualityComparerTests
{
    readonly ConstructorStructEqualityComparer Instance = ConstructorStructEqualityComparer.Instance;

    [Fact]
    public void Returns_false_when_compared_with_null()
    {
        var item = new ConstructorStruct(null, new ImmutableArray<MemberDeclarationSyntax>());
        Assert.False(item.Equals(null));
    }

    [Fact]
    public void Returns_true_when_compared_with_itself()
    {
        var item = new ConstructorStruct(null, new ImmutableArray<MemberDeclarationSyntax>());
        Assert.True(item.Equals(item));
    }

    [Fact]
    public void Returns_true_when_compared_with_copy_of_itself()
    {
        var item1 = new ConstructorStruct(null, new ImmutableArray<MemberDeclarationSyntax>());
        var item2 = new ConstructorStruct(null, new ImmutableArray<MemberDeclarationSyntax>());
        Assert.True(item1.Equals(item2));
    }

    [Fact]
    public void Returns_false_when_first_param_is_null()
    {
        var item = new ConstructorStruct(null, new ImmutableArray<MemberDeclarationSyntax>());
        Assert.Throws<NotImplementedException>(() =>
            Instance.GetHashCode(item)
        );
    }
}
