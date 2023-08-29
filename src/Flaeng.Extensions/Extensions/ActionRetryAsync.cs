namespace Flaeng.Extensions;

public static class ActionRetryAsync
{
    #region AsyncAction that doesnt CancellationToken
    public static Task<Attempt> RetryAsync(this Action function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt> RetryAsync(this Action function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static Task<Attempt> RetryAsync(this Action function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        Action<CancellationToken> newFunc = ct => function();
        return newFunc.RetryAsync(numberOfTimes, delay, token);
    }
    #endregion

    #region AsyncAction that accepts CancellationToken
    public static Task<Attempt> RetryAsync(this Action<CancellationToken> function, int numberOfTimes, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.Zero, token);

    public static Task<Attempt> RetryAsync(this Action<CancellationToken> function, int numberOfTimes, double delayInMs, CancellationToken token = default)
        => function.RetryAsync(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs), token);

    public static async Task<Attempt> RetryAsync(this Action<CancellationToken> function, int numberOfTimes, TimeSpan delay, CancellationToken token = default)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            token.ThrowIfCancellationRequested();
            if (i != 0 && delay.Ticks > 0)
                await Task.Delay(delay, token);

            try
            {
                await Task.Factory.StartNew(() => function(token), token);
                return Attempt.Success;
            }
            catch { }
        }
        return Attempt.Failed;
    }
    #endregion
}
