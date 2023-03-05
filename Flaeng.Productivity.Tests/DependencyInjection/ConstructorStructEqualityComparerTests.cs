using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Flaeng.Productivity.Tests.DependencyInjection;

public class ConstructorStructEqualityComparerTests
{
    readonly ConstructorDataEqualityComparer Instance = ConstructorDataEqualityComparer.Instance;

    [Fact]
    public void Returns_false_when_compared_with_null()
    {
        var item = new ConstructorData(
            null,
            null,
            new ImmutableArray<ISymbol>(),
            new ImmutableArray<WrapperClassData>()
            );
        Assert.False(item.Equals(null));
    }

    [Fact]
    public void Returns_true_when_compared_with_itself()
    {
        var item = new ConstructorData(
            null,
            null,
            new ImmutableArray<ISymbol>(),
            new ImmutableArray<WrapperClassData>()
            );
        Assert.True(item.Equals(item));
    }

    [Fact]
    public void Returns_true_when_compared_with_copy_of_itself()
    {
        var item1 = new ConstructorData(
            null,
            null,
            new ImmutableArray<ISymbol>(),
            new ImmutableArray<WrapperClassData>());
        var item2 = new ConstructorData(
            null,
            null,
            new ImmutableArray<ISymbol>(),
            new ImmutableArray<WrapperClassData>());
        Assert.True(item1.Equals(item2));
    }

    [Fact]
    public void Returns_false_when_first_param_is_null()
    {
        var item = new ConstructorData(
            null,
            null,
            new ImmutableArray<ISymbol>(),
            new ImmutableArray<WrapperClassData>());
        Assert.Throws<NotImplementedException>(() =>
            Instance.GetHashCode(item)
        );
    }
}
