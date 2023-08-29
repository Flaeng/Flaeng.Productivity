namespace Flaeng.Productivity.EitherOfT1OrT2Tests;

public class ValueTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Can_get_value_on_result()
    {
        // Given
        Either<Result, Error> result = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When

        // Then
        Assert.IsType<Result>(result.Value);
        Assert.Equal(1, ((Result)result.Value).Count);
    }

    [Fact]
    public void Can_get_value_on_error()
    {
        // Given
        Either<Result, Error> result = new Error { ErrorMessage = "Failed" };

        // When

        // Then
        Assert.IsType<Error>(result.Value);
        Assert.Equal("Failed", ((Error)result.Value).ErrorMessage);
    }
}
