namespace Flaeng.Productivity.AttemptTests;

public class SimpleAttemptOfTorTTests
{
    [Fact]
    public void Simple_success_attempt_of_T()
    {
        // Given

        // When
        var result = Attempt<int, string>.Success(101);

        // Then
        Assert.True(result.DidSucceed);
        Assert.Equal(101, result.Result);
        // Assert.Null(result.Error);
    }

    [Fact]
    public void Simple_failure_attempt_of_T()
    {
        // Given

        // When
        var result = Attempt<int, string>.Failed("101");

        // Then
        Assert.False(result.DidSucceed);
        // Assert.Null(result.Result); TODO
        Assert.Equal("101", result.Error);
    }

    [Fact]
    public void Simple_success_attempt_of_T_with_action()
    {
        // Given
        Func<Attempt<int, string>> action = () => { return 201; };

        // When
        var result = action();

        // Then
        Assert.True(result.DidSucceed);
        Assert.Equal(201, result.Result);
    }

    [Fact]
    public void Simple_failure_attempt_of_T_with_action()
    {
        // Given
        Func<Attempt<int, string>> action = () => { return "Failed"; };

        // When
        Attempt<int, string> result = action();

        // Then
        Assert.False(result.DidSucceed);
        // Assert.Null(result.Result); TODO
        Assert.Equal("Failed", result.Error);
    }
}
