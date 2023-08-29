namespace Flaeng.Productivity.EitherOfT1OrT2Tests;

public class CaseTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Case_all_when_result()
    {
        // Given
        Either<Result, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When
        var msg = either.Case(
            res => res.Strings.First(),
            err => err.ErrorMessage
        );

        // Then
        Assert.Equal("Hey", msg);
    }

    [Fact]
    public void Case_all_when_error()
    {
        // Given
        Either<Result, Error> either = new Error { ErrorMessage = "Failed" };

        // When
        var msg = either.Case(
            res => res.Strings.First(),
            err => err.ErrorMessage
        );

        // Then
        Assert.Equal("Failed", msg);
    }

    [Fact]
    public void Case_result()
    {
        // Given
        Either<Result, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When
        var all = either.Case(res => res.Strings.First(), res => res.ErrorMessage);
        var resultMsg = either.Case(res => res.Strings.First());
        var errorMessage = either.Case(res => res.ErrorMessage);

        // Then
        Assert.Equal("Hey", all);
        Assert.Equal("Hey", resultMsg);
        Assert.Null(errorMessage);
    }

    [Fact]
    public void Case_error()
    {
        // Given
        Either<Result, Error> either = new Error { ErrorMessage = "Failed" };

        // When
        var all = either.Case(res => res.Strings.First(), res => res.ErrorMessage);
        var resultMsg = either.Case(res => res.Strings.First());
        var errorMessage = either.Case(res => res.ErrorMessage);

        // Then
        Assert.Equal("Failed", all);
        Assert.Null(resultMsg);
        Assert.Equal("Failed", errorMessage);
    }

    [Fact]
    public void Void_case_result()
    {
        // Given
        Either<Result, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };
        string? all = null, resultMsg = null, errorMessage = null;

        // When
        either.Case(res => { all = res.Strings.First(); }, res => { all = res.ErrorMessage; });
        either.Case(res => { resultMsg = res.Strings.First(); });
        either.Case(res => { errorMessage = res.ErrorMessage; });

        // Then
        Assert.Equal("Hey", all);
        Assert.Equal("Hey", resultMsg);
        Assert.Null(errorMessage);
    }

    [Fact]
    public void Void_case_error()
    {
        // Given
        Either<Result, Error> either = new Error { ErrorMessage = "Failed" };
        string? all = null, resultMsg = null, errorMessage = null;

        // When
        either.Case(res => { all = res.Strings.First(); }, res => { all = res.ErrorMessage; });
        either.Case(res => { resultMsg = res.Strings.First(); });
        either.Case(res => { errorMessage = res.ErrorMessage; });

        // Then
        Assert.Equal("Failed", all);
        Assert.Null(resultMsg);
        Assert.Equal("Failed", errorMessage);
    }

}
