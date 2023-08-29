namespace Flaeng.Productivity.EitherOfT1OrT2OrT3Tests;

public class MapTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    record Pending(string Key);
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Can_map_result_when_success()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Result { Count = 1, Strings = new List<string> { "Hey" } });

        // When
        var result = either.Map(x => x.Strings);

        // Then
        Assert.Equal("Hey", result.AsT1()!.ElementAt(0));
        Assert.Null(result.AsT2());
        Assert.Equal(default, result.AsT3());
    }

    [Fact]
    public void Can_attempt_map_result_when_pending()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Pending("AB23"));

        // When
        var result = either.Map(x => x.Strings);

        // Then
        Assert.Null(result.AsT1());
        Assert.Equal("AB23", result.AsT2()!.Key);
        Assert.Equal(default, result.AsT3());
    }

    [Fact]
    public void Can_attempt_map_result_when_error()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Error { ErrorMessage = "Failed" });

        // When
        var result = either.Map(x => x.Strings);

        // Then
        Assert.Null(result.AsT1());
        Assert.Null(result.AsT2());
        Assert.Equal("Failed", result.AsT3()!.ErrorMessage);
    }

    [Fact]
    public void Can_attempt_map_pending_when_success()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Result { Count = 1, Strings = new List<string> { "Hey" } });

        // When
        var result = either.Map(x => x.Key);

        // Then
        Assert.Equal("Hey", result.AsT1()!.Strings.ElementAt(0));
        Assert.Null(result.AsT2());
        Assert.Equal(default, result.AsT3());
    }

    [Fact]
    public void Can_map_pending_when_pending()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Pending("AB23"));

        // When
        var result = either.Map(x => x.Key);

        // Then
        Assert.Null(result.AsT1());
        Assert.Equal("AB23", result.AsT2());
        Assert.Equal(default, result.AsT3());
    }

    [Fact]
    public void Can_attempt_map_pending_when_error()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Error { ErrorMessage = "Failed" });

        // When
        var result = either.Map(x => x.Key);

        // Then
        Assert.Null(result.AsT1());
        Assert.Null(result.AsT2());
        Assert.Equal("Failed", result.AsT3()!.ErrorMessage);
    }

    [Fact]
    public void Can_attempt_map_error_when_success()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Result { Count = 1, Strings = new List<string> { "Hey" } });

        // When
        var result = either.Map(x => x.ErrorMessage);

        // Then
        Assert.Equal("Hey", result.AsT1()!.Strings.ElementAt(0));
        Assert.Null(result.AsT2());
        Assert.Equal(default, result.AsT3());
    }

    [Fact]
    public void Can_attempt_map_error_when_pending()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Pending("AB23"));

        // When
        var result = either.Map(x => x.ErrorMessage);

        // Then
        Assert.Null(result.AsT1());
        Assert.Equal("AB23", result.AsT2()!.Key);
        Assert.Equal(default, result.AsT3());
    }

    [Fact]
    public void Can_map_error_when_error()
    {
        // Given
        var either = new Either<Result, Pending, Error>(new Error { ErrorMessage = "Failed" });

        // When
        var result = either.Map(x => x.ErrorMessage);

        // Then
        Assert.Null(result.AsT1());
        Assert.Null(result.AsT2());
        Assert.Equal("Failed", result.AsT3());
    }

}
