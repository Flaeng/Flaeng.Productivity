namespace Flaeng.Productivity.EitherOfT1OrT2OrT3Tests;

public class ValueTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    record Pending(string Key);
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Can_get_value_on_result()
    {
        // Given
        Either<Result, Pending, Error> result = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When

        // Then
        Assert.IsType<Result>(result.Value);
        Assert.Equal(1, ((Result)result.Value).Count);
    }

    [Fact]
    public void Can_get_value_on_pending()
    {
        // Given
        Either<Result, Pending, Error> result = new Pending("IG78");

        // When

        // Then
        Assert.IsType<Pending>(result.Value);
        Assert.Equal("IG78", ((Pending)result.Value).Key);
    }

    [Fact]
    public void Can_get_value_on_error()
    {
        // Given
        Either<Result, Pending, Error> result = new Error { ErrorMessage = "Failed" };

        // When

        // Then
        Assert.IsType<Error>(result.Value);
        Assert.Equal("Failed", ((Error)result.Value).ErrorMessage);
    }
}
