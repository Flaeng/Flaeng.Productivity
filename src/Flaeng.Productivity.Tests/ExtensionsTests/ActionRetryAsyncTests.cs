using System.IO.Compression;

namespace Flaeng.Productivity.Tests.ExtensionsTests;

public class ActionRetryAsyncTests
{
    [Fact]
    public async Task Will_call_action_again_if_method_throws_exceptions()
    {
        // Given
        int counter = 0;
        Action action = () =>
        {
            counter++;
            throw new Exception();
        };

        // When
        var result = await action.RetryAsync(numberOfTimes: 3);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task Will_not_call_action_again_if_method_returns()
    {
        // Given
        int counter = 0;
        Action action = () => counter++;

        // When
        var result = await action.RetryAsync(numberOfTimes: 3);

        // Then
        Assert.True(result.DidSucceed);
        Assert.Equal(1, counter);
    }

    [Fact]
    public async Task Will_handle_delay()
    {
        // Given
        Action action = () => throw new Exception();

        // When
        var watch = Stopwatch.StartNew();
        var result = await action.RetryAsync(numberOfTimes: 3, delayInMs: 1000);
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 1700, long.MaxValue);
    }

    [Fact]
    public async Task Will_stop_if_cancellation_token_is_fulfilled()
    {
        // Given
        Action action = () => { Thread.Sleep(1000); throw new Exception(); };
        var tokenSource = new CancellationTokenSource(1000);

        // When
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await action.RetryAsync(3, tokenSource.Token)
        );

        // Then
    }

    [Fact]
    public async Task Will_handle_negative_delay()
    {
        // Given
        var tokenSource = new CancellationTokenSource();
        int counter = 0;
        Action<CancellationToken> action = token =>
        {
            if (counter++ == 1)
                tokenSource.Cancel();
            throw new Exception();
        };

        // When
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await action.RetryAsync(3, tokenSource.Token)
        );

        // Then
        Assert.Equal(2, counter);
    }

}
