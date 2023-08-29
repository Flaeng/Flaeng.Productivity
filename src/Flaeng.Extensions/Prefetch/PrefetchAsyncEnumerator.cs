namespace Flaeng.Productivity;

#pragma warning disable CS8601

public class PrefetchAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly CancellationToken token;
    private readonly IAsyncEnumerator<T> enumerator;
    public PrefetchAsyncEnumerator(IAsyncEnumerable<T> funcs, CancellationToken token)
    {
        this.token = token;
        this.enumerator = funcs.GetAsyncEnumerator(token);
    }

    private readonly ValueTask<bool> hasMoreItems = new ValueTask<bool>(true);
    private Task<Wrapper>? nextItem = null;
    public T Current { get; private set; } = default;

    record Wrapper(bool MoveNext, T Current);

    public async ValueTask<bool> MoveNextAsync()
    {
        // On first run, set nextItem so that we can await it later
        bool bValue = true;
        if (nextItem == null)
        {
            // If enumerator is empty, return false
            var hasItems = await enumerator.MoveNextAsync();
            if (hasItems == false)
                return false;
            Current = enumerator.Current;
        }
        else
        {
            Current = nextItem.Result.Current;
            bValue = nextItem.Result.MoveNext;
        }

        nextItem = enumerator.MoveNextAsync()
            .AsTask()
            .ContinueWith(x => new Wrapper(x.Result, enumerator.Current));
        return bValue;
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
