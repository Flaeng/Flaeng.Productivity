namespace Flaeng.Productivity;
#pragma warning disable CS8601

public static class EnumeratorExtensions
{
    public static IAsyncEnumerator<T> Prefetch<T>(this IEnumerable<Task<T>> collection)
    {
        return new PrefetchEnumerator<T>(collection);
    }

    public static IAsyncEnumerable<T> Prefetch<T>(this IAsyncEnumerable<T> collection)
    {
        return new PrefetchAsyncEnumerable<T>(collection);
    }

    public static IEnumerable<T> WaitAndEnumerate<T>(this IEnumerable<Task<T>> collection, CancellationToken token = default)
    {
        var taskArray = collection.ToArray();
        while (taskArray.Length != 0)
        {
            int index = Task.WaitAny(taskArray, token);
            yield return taskArray[index].Result;
            Task<T>[] tempArray = new Task<T>[taskArray.Length - 1];
            Array.Copy(taskArray, 0, tempArray, 0, index); // Up to i
            Array.Copy(taskArray, index + 1, tempArray, index, taskArray.Length - index - 1); // From i + 1 to end of array
            taskArray = tempArray;
        }
    }
}
