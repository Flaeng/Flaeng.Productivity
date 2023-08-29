namespace Flaeng.Productivity.ExtensionsTests;

public class FuncRetryAsyncTests
{
    [Fact]
    public async Task Will_run_async_when_given_sync_method()
    {
        // Given
        int counter = 0;
        Func<int> action = () => { counter++; throw new Exception(); };

        // When
        var result = await action.RetryAsync(3);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task Will_run_async_untill_cancellation_token_is_triggered()
    {
        // Given
        var tokenSource = new CancellationTokenSource();
        int counter = 0;
        Func<int> action = () =>
        {
            if (counter++ == 1)
                tokenSource.Cancel();
            throw new Exception();
        };

        // When
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await action.RetryAsync(3, tokenSource.Token);
        });

        // Then
        Assert.Equal(2, counter);
    }

    [Fact]
    public async Task Will_run_async_with_attempt_when_given_sync_method()
    {
        // Given
        int counter = 0;
        Func<Attempt<int>> action = () => { counter++; return Attempt<int>.Failed; };

        // When
        var result = await action.RetryAsync(3);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task Will_run_async_with_attempt_untill_cancellation_token_is_triggered()
    {
        // Given
        var tokenSource = new CancellationTokenSource();
        int counter = 0;
        Func<Attempt<int>> action = () =>
        {
            if (counter++ == 1)
                tokenSource.Cancel();
            return Attempt<int>.Failed;
        };

        // When
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await action.RetryAsync(3, tokenSource.Token);
        });

        // Then
        Assert.Equal(2, counter);
    }

    [Fact]
    public async Task Will_run_async_with_task_of_attempt_when_given_sync_method()
    {
        // Given
        int counter = 0;
        Func<Task<Attempt<int>>> action = async () =>
        {
            counter++;
            await Task.Yield();
            return Attempt<int>.Failed;
        };

        // When
        var result = await action.RetryAsync(3);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task Will_run_async_with_task_of_attempt_untill_cancellation_token_is_triggered()
    {
        // Given
        var tokenSource = new CancellationTokenSource();
        int counter = 0;
        Func<Task<Attempt<int>>> action = async () =>
        {
            if (counter++ == 1)
                tokenSource.Cancel();
            await Task.Yield();
            return Attempt<int>.Failed;
        };

        // When
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await action.RetryAsync(3, tokenSource.Token);
        });

        // Then
        Assert.Equal(2, counter);
    }

    [Fact]
    public async Task Will_handle_retries_when_running_async_method_async()
    {
        // Given
        int counter = 0;
        Func<Task<int>> action = async () =>
        {
            counter++;
            await Task.Yield();
            throw new Exception();
        };

        // When
        Attempt<int> result = await action.RetryAsync(3);

        // Then
        Assert.Equal(3, counter);
    }

    [Fact]
    public async Task Will_handle_delay_when_running_with_task()
    {
        // Given
        Func<Task<int>> action = async () =>
        {
            await Task.Yield();
            throw new Exception();
        };

        // When
        var watch = Stopwatch.StartNew();
        Attempt<int> result = await action.RetryAsync(3, delayInMs: 1000);
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 1700, 2700);
    }

    [Fact]
    public async Task Will_handle_delay_when_running_with_task_of_attempt()
    {
        // Given
        Func<Task<Attempt<int>>> action = () => Task.FromResult(Attempt<int>.Failed);

        // When
        var watch = Stopwatch.StartNew();
        var result = await action.RetryAsync(3, delayInMs: 1000);
        watch.Stop();

        // Then
        Assert.InRange(watch.ElapsedMilliseconds, 1700, 2700);
    }

}
