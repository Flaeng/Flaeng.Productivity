namespace Flaeng.Productivity;

#pragma warning disable CS8601

public class PrefetchEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<Task<T>> enumerator;
    public PrefetchEnumerator(IEnumerable<Task<T>> funcs)
    {
        this.enumerator = funcs.GetEnumerator();
    }

    private bool hasMoreItems = true;
    private Task<T>? nextItem = null;
    public T Current { get; private set; } = default;

    public async ValueTask<bool> MoveNextAsync()
    {
        // On first run, set nextItem so that we can await it later
        if (nextItem == null)
        {
            // If enumerator is empty, return false
            var hasItems = enumerator.MoveNext();
            if (hasItems == false)
                return false;
            nextItem = enumerator.Current;
        }

        await nextItem;
        Current = nextItem.Result;

        if (hasMoreItems == false)
            return false;

        hasMoreItems = enumerator.MoveNext();
        nextItem = hasMoreItems ? enumerator.Current : null;
        return true;
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
