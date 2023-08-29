namespace Flaeng.Extensions;

public static class FuncRetry
{
    #region Functions that does not return Attempt<T>
    public static Attempt<T> Retry<T>(this Func<T> function, int numberOfTimes)
        => function.Retry(numberOfTimes, TimeSpan.Zero);

    public static Attempt<T> Retry<T>(this Func<T> function, int numberOfTimes, double delayInMs)
        => function.Retry(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs));

    public static Attempt<T> Retry<T>(this Func<T> function, int numberOfTimes, TimeSpan delay)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            if (i != 0 && delay.Ticks > 0)
                Thread.Sleep(delay);

            try
            {
                return function();
            }
            catch { }
        }
        return Attempt<T>.Failed;
    }
    #endregion

    #region Functions that returns attempt
    public static Attempt<T> Retry<T>(this Func<Attempt<T>> function, int numberOfTimes)
        => function.Retry(numberOfTimes, TimeSpan.Zero);

    public static Attempt<T> Retry<T>(this Func<Attempt<T>> function, int numberOfTimes, double delayInMs)
        => function.Retry(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs));

    public static Attempt<T> Retry<T>(this Func<Attempt<T>> function, int numberOfTimes, TimeSpan delay)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            if (i != 0 && delay.Ticks > 0)
                Thread.Sleep(delay);

            var result = function();
            if (result.DidSucceed)
                return result;
        }
        return Attempt<T>.Failed;
    }
    #endregion
}
