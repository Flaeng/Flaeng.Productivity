namespace Flaeng.Productivity.EitherOfT1OrT2OrT3Tests;

public class IsTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    record Pending(string Key);
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Can_get_value_on_result()
    {
        // Given
        Either<Result, Pending, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When
        var bResult1 = either.Is(out Result? result);
        var bResult2 = either.Is(out Pending? pending);
        var bResult3 = either.Is(out Error error);

        // Then
        Assert.True(bResult1);
        Assert.NotNull(result);
        Assert.False(bResult2);
        Assert.Null(pending);
        Assert.False(bResult3);
        Assert.Equal(default, error);
    }

    [Fact]
    public void Can_get_value_on_pending()
    {
        // Given
        Either<Result, Pending, Error> either = new Pending("IG78");

        // When
        var bResult1 = either.Is(out Result? result);
        var bResult2 = either.Is(out Pending? pending);
        var bResult3 = either.Is(out Error error);

        // Then
        Assert.False(bResult1);
        Assert.Null(result);
        Assert.True(bResult2);
        Assert.NotNull(pending);
        Assert.False(bResult3);
        Assert.Equal(default, error);
    }

    [Fact]
    public void Can_get_value_on_error()
    {
        // Given
        Either<Result, Pending, Error> either = new Error { ErrorMessage = "Failed" };

        // When
        var bResult1 = either.Is(out Result? result);
        var bResult2 = either.Is(out Pending? pending);
        var bResult3 = either.Is(out Error error);

        // Then
        Assert.False(bResult1);
        Assert.Null(result);
        Assert.False(bResult2);
        Assert.Null(pending);
        Assert.True(bResult3);
        // Assert.NotNull(error);
    }
}
