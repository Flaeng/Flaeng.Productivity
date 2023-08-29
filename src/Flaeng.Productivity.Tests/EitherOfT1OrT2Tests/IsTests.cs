namespace Flaeng.Productivity.EitherOfT1OrT2Tests;

public class IsTests
{
    public class Result
    {
        public int Count { get; init; }
        public List<string> Strings { get; init; } = new();
    }
    public struct Error
    {
        public string ErrorMessage;
    }

    [Fact]
    public void Can_get_value_on_result()
    {
        // Given
        Either<Result, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When
        var bResult1 = either.Is(out Result? result);
        var bResult2 = either.Is(out Error error);

        // Then
        Assert.True(bResult1);
        Assert.NotNull(result);
        Assert.False(bResult2);
        Assert.Equal(default, error);
    }

    [Fact]
    public void Can_get_value_on_error()
    {
        // Given
        Either<Result, Error> either = new Error { ErrorMessage = "Failed" };

        // When
        var bResult1 = either.Is(out Result? result);
        var bResult2 = either.Is(out Error error);

        // Then
        Assert.False(bResult1);
        Assert.Null(result);
        Assert.True(bResult2);
        // Assert.NotNull(error);
    }
}
