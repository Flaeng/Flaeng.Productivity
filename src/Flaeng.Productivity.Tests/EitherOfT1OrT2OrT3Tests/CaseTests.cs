namespace Flaeng.Productivity.EitherOfT1OrT2OrT3Tests;

public class CaseTests
{
    class Result { public int Count { get; init; } public List<string> Strings { get; init; } = new(); }
    record Pending(string Key);
    struct Error { public string ErrorMessage; }

    [Fact]
    public void Case_all_when_result()
    {
        // Given
        Either<Result, Pending, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When
        var msg = either.Case(
            res => res.Strings.First(),
            pend => pend.Key,
            err => err.ErrorMessage
        );

        // Then
        Assert.Equal("Hey", msg);
    }

    [Fact]
    public void Case_all_when_pending()
    {
        // Given
        Either<Result, Pending, Error> either = new Pending("DH18");

        // When
        var msg = either.Case(
            res => res.Strings.First(),
            pend => pend.Key,
            err => err.ErrorMessage
        );

        // Then
        Assert.Equal("DH18", msg);
    }

    [Fact]
    public void Case_all_when_error()
    {
        // Given
        Either<Result, Pending, Error> either = new Error { ErrorMessage = "Failed" };

        // When
        var msg = either.Case(
            res => res.Strings.First(),
            pend => pend.Key,
            err => err.ErrorMessage
        );

        // Then
        Assert.Equal("Failed", msg);
    }

    [Fact]
    public void Case_result()
    {
        // Given
        Either<Result, Pending, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };

        // When
        var all = either.Case(res => res.Strings.First(), res => res.Key, res => res.ErrorMessage);
        var resultMsg = either.Case(res => res.Strings.First());
        var pendingKey = either.Case(res => res.Key);
        var errorMessage = either.Case(res => res.ErrorMessage);

        // Then
        Assert.Equal("Hey", all);
        Assert.Equal("Hey", resultMsg);
        Assert.Null(pendingKey);
        Assert.Null(errorMessage);
    }

    [Fact]
    public void Case_pending()
    {
        // Given
        Either<Result, Pending, Error> either = new Pending("DH18");

        // When
        var all = either.Case(res => res.Strings.First(), res => res.Key, res => res.ErrorMessage);
        var resultMsg = either.Case(res => res.Strings.First());
        var pendingKey = either.Case(res => res.Key);
        var errorMessage = either.Case(res => res.ErrorMessage);

        // Then
        Assert.Equal("DH18", all);
        Assert.Null(resultMsg);
        Assert.Equal("DH18", pendingKey);
        Assert.Null(errorMessage);
    }

    [Fact]
    public void Case_error()
    {
        // Given
        Either<Result, Pending, Error> either = new Error { ErrorMessage = "Failed" };

        // When
        var all = either.Case(res => res.Strings.First(), res => res.Key, res => res.ErrorMessage);
        var resultMsg = either.Case(res => res.Strings.First());
        var pendingKey = either.Case(res => res.Key);
        var errorMessage = either.Case(res => res.ErrorMessage);

        // Then
        Assert.Equal("Failed", all);
        Assert.Null(resultMsg);
        Assert.Null(pendingKey);
        Assert.Equal("Failed", errorMessage);
    }

    [Fact]
    public void Void_case_result()
    {
        // Given
        Either<Result, Pending, Error> either = new Result { Count = 1, Strings = new List<string> { "Hey" } };
        string? all = null, resultMsg = null, pendingKey = null, errorMessage = null;

        // When
        either.Case(res => { all = res.Strings.First(); }, res => { all = res.Key; }, res => { all = res.ErrorMessage; });
        either.Case(res => { resultMsg = res.Strings.First(); });
        either.Case(res => { pendingKey = res.Key; });
        either.Case(res => { errorMessage = res.ErrorMessage; });

        // Then
        Assert.Equal("Hey", all);
        Assert.Equal("Hey", resultMsg);
        Assert.Null(pendingKey);
        Assert.Null(errorMessage);
    }

    [Fact]
    public void Void_case_pending()
    {
        // Given
        Either<Result, Pending, Error> either = new Pending("DH18");
        string? all = null, resultMsg = null, pendingKey = null, errorMessage = null;

        // When
        either.Case(res => { all = res.Strings.First(); }, res => { all = res.Key; }, res => { all = res.ErrorMessage; });
        either.Case(res => { resultMsg = res.Strings.First(); });
        either.Case(res => { pendingKey = res.Key; });
        either.Case(res => { errorMessage = res.ErrorMessage; });

        // Then
        Assert.Equal("DH18", all);
        Assert.Null(resultMsg);
        Assert.Equal("DH18", pendingKey);
        Assert.Null(errorMessage);
    }

    [Fact]
    public void Void_case_error()
    {
        // Given
        Either<Result, Pending, Error> either = new Error { ErrorMessage = "Failed" };
        string? all = null, resultMsg = null, pendingKey = null, errorMessage = null;

        // When
        either.Case(res => { all = res.Strings.First(); }, res => { all = res.Key; }, res => { all = res.ErrorMessage; });
        either.Case(res => { resultMsg = res.Strings.First(); });
        either.Case(res => { pendingKey = res.Key; });
        either.Case(res => { errorMessage = res.ErrorMessage; });

        // Then
        Assert.Equal("Failed", all);
        Assert.Null(resultMsg);
        Assert.Null(pendingKey);
        Assert.Equal("Failed", errorMessage);
    }

}
