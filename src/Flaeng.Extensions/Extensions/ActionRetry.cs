namespace Flaeng.Extensions;

public static class ActionRetry
{
    public static Attempt Retry(this Action function, int numberOfTimes)
        => function.Retry(numberOfTimes, TimeSpan.Zero);

    public static Attempt Retry(this Action function, int numberOfTimes, double delayInMs)
        => function.Retry(numberOfTimes, TimeSpan.FromMilliseconds(delayInMs));

    public static Attempt Retry(this Action function, int numberOfTimes, TimeSpan delay)
    {
        for (int i = 0; i < numberOfTimes; i++)
        {
            if (i != 0 && delay.Ticks > 0)
                Thread.Sleep(delay);

            try
            {
                function();
                return Attempt.Success;
            }
            catch { }
        }
        return Attempt.Failed;
    }
}
