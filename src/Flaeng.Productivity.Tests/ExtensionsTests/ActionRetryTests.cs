using System.Diagnostics;

namespace Flaeng.Productivity.Tests.ExtensionsTests;

public class ActionRetryTests
{
    [Fact]
    public void Will_call_action_again_if_method_throws_exceptions()
    {
        // Given
        int counter = 0;
        Action action = () =>
        {
            counter++;
            throw new Exception();
        };

        // When
        var result = action.Retry(3);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(3, counter);
    }

    [Fact]
    public void Will_not_call_action_again_if_method_returns()
    {
        // Given
        int counter = 0;
        Action action = () => counter++;

        // When
        var result = action.Retry(3);

        // Then
        Assert.True(result.DidSucceed);
        Assert.Equal(1, counter);
    }

    [Fact]
    public void Will_handle_delay()
    {
        // Given
        Action action = () => throw new Exception();

        // When
        var watch = Stopwatch.StartNew();
        var result = action.Retry(3, TimeSpan.FromSeconds(1));
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 1700, 2700);
    }

    [Fact]
    public void Will_handle_negative_delay()
    {
        // Given
        Action action = () => throw new Exception();

        // When
        var watch = Stopwatch.StartNew();
        var result = action.Retry(3, -1);
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 0, 700);
    }

}
