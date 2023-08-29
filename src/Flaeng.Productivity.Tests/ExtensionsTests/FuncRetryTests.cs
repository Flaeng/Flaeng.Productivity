namespace Flaeng.Productivity.Tests.ExtensionsTests;

public class FuncRetryTests
{
    [Fact]
    public void Will_call_func_again_if_method_throws_exceptions()
    {
        // Given
        int counter = 0;
        Func<int> func = () =>
        {
            counter++;
            throw new Exception();
        };

        // When
        var result = func.Retry(3);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(3, counter);
    }

    [Fact]
    public void Will_not_call_func_again_if_method_returns()
    {
        // Given
        int counter = 0;
        Func<int> action = () => counter++;

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
        Func<int> action = () => throw new Exception();

        // When
        var watch = Stopwatch.StartNew();
        var result = action.Retry(3, delayInMs: 1000);
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 1700, 2700);
    }

    [Fact]
    public void Will_handle_negative_delay()
    {
        // Given
        Func<int> action = () => throw new Exception();

        // When
        var watch = Stopwatch.StartNew();
        var result = action.Retry(3, -1);
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 0, 700);
    }

    [Fact]
    public void Will_handle_attempt()
    {
        // Given
        int counter = 0;
        Func<Attempt<int>> action = () => { counter++; return Attempt<int>.Failed; };

        // When
        var result = action.Retry(3);

        // Then
        Assert.Equal(3, counter);
    }

    [Fact]
    public void Will_handle_attempt_with_delay()
    {
        // Given
        int counter = 0;
        Func<Attempt<int>> action = () => { counter++; return Attempt<int>.Failed; };

        // When
        var watch = Stopwatch.StartNew();
        var result = action.Retry(3, delayInMs: 1000);
        watch.Stop();

        // Then
        Assert.Equal(3, counter);
        Assert.InRange(watch.ElapsedMilliseconds, 1500, long.MaxValue);
    }

}
