namespace Flaeng.Extensions;

public static class FuncRetryAsync
{
    #region Functions that does not return Attempt<T>
    public static Task<Attempt<T>> RetryAsync<T>(this Func<T> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync<T>(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<T> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync<T>(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<T> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        Func<CancellationToken, T> func = _ => function();
        return func.RetryAsync(numberOfTimes, delay, token);
    }

    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, T> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync<T>(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, T> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync<T>(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, T> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        Func<CancellationToken, Task<T>> func = ct => Task.Factory.StartNew(() => function(ct));
        return func.RetryAsync(numberOfTimes, delay, token);
    }

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Task<T>> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Task<T>> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Task<T>> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        Func<CancellationToken, Task<T>> func = ct => function();
        return func.RetryAsync(numberOfTimes, delay, token);
    }

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Task<Attempt<T>>> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Task<Attempt<T>>> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Task<Attempt<T>>> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        Func<CancellationToken, Task<Attempt<T>>> func = ct => function();
        return func.RetryAsync(numberOfTimes, delay, token);
    }

    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, Task<T>> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, Task<T>> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static async Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, Task<T>> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            token.ThrowIfCancellationRequested();
            if (i != 0 && delay.Ticks > 0)
                await Task.Delay(delay, token);

            try
            {
                return await function(token);
            }
            catch { }
        }
        return Attempt<T>.Failed;
    }
    #endregion

    #region Functions that returns attempt
    // Doesnt accept CancellationToken
    public static Task<Attempt<T>> RetryAsync<T>(this Func<Attempt<T>> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync<T>(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Attempt<T>> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync<T>(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<Attempt<T>> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        Func<CancellationToken, Task<Attempt<T>>> func = ct => Task.Factory.StartNew(function);
        return func.RetryAsync(numberOfTimes, delay, token);
    }

    // Accepts CancellationToken
    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, Task<Attempt<T>>> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, Task<Attempt<T>>> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static async Task<Attempt<T>> RetryAsync<T>(this Func<CancellationToken, Task<Attempt<T>>> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            token.ThrowIfCancellationRequested();
            if (i != 0 && delay.Ticks > 0)
                await Task.Delay(delay, token);

            var result = await function(token);
            if (result.DidSucceed)
                return result;
        }
        return Attempt<T>.Failed;
    }
    #endregion
}
