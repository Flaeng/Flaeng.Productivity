namespace Flaeng.Productivity.EitherOfT1OrT2Tests;

public class MapTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Can_map_result_when_success()
    {
        // Given
        var either = new Either<Result, Error>(new Result { Count = 1, Strings = new List<string> { "Hey" } });

        // When
        var result = either.Map(x => x.Strings);

        // Then
        Assert.Equal("Hey", result.AsT1()!.ElementAt(0));
        Assert.Equal(default, result.AsT2());
    }

    [Fact]
    public void Can_attempt_map_result_when_error()
    {
        // Given
        var either = new Either<Result, Error>(new Error { ErrorMessage = "Failed" });

        // When
        var result = either.Map(x => x.Strings);

        // Then
        Assert.Null(result.AsT1());
        Assert.Equal("Failed", result.AsT2()!.ErrorMessage);
    }

    [Fact]
    public void Can_attempt_map_error_when_success()
    {
        // Given
        var either = new Either<Result, Error>(new Result { Count = 1, Strings = new List<string> { "Hey" } });

        // When
        var result = either.Map(x => x.ErrorMessage);

        // Then
        Assert.Equal("Hey", result.AsT1()!.Strings.ElementAt(0));
        Assert.Equal(default, result.AsT2());
    }

    [Fact]
    public void Can_map_error_when_error()
    {
        // Given
        var either = new Either<Result, Error>(new Error { ErrorMessage = "Failed" });

        // When
        var result = either.Map(x => x.ErrorMessage);

        // Then
        Assert.Null(result.AsT1());
        Assert.Equal("Failed", result.AsT2());
    }

}
