namespace Flaeng.Productivity.AttemptTests;

public class SimpleAttemptTests
{
    [Fact]
    public void Simple_success_attempt()
    {
        // Given

        // When
        var result = Attempt.Success;

        // Then
        Assert.True(result.DidSucceed);
    }

    [Fact]
    public void Simple_failure_attempt()
    {
        // Given

        // When
        var result = Attempt.Failed;

        // Then
        Assert.False(result.DidSucceed);
    }
}
