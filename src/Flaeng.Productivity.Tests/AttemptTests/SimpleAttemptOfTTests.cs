namespace Flaeng.Productivity.AttemptTests;

public class SimpleAttemptOfTTests
{
    [Fact]
    public void Simple_success_attempt_of_T()
    {
        // Given

        // When
        var result = Attempt<int>.Success(101);

        // Then
        Assert.True(result.DidSucceed);
        Assert.Equal(101, result.Result);
    }

    [Fact]
    public void Simple_failure_attempt_of_T()
    {
        // Given

        // When
        var result = Attempt<int>.Failed;

        // Then
        Assert.False(result.DidSucceed);
        // Assert.Null(result.Result); TODO
    }

    [Fact]
    public void Simple_success_attempt_of_T_with_action()
    {
        // Given
        int i = 201;
        Func<Attempt<int>> action = () => { return i; };

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
        Func<Attempt<int>> action = () => { return Attempt<int>.Failed; };

        // When
        Attempt<int> result = action();

        // Then
        Assert.False(result.DidSucceed);
        // Assert.Null(result.Result); TODO
    }
}
