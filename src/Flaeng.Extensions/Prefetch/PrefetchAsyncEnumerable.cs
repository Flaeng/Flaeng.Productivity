namespace Flaeng.Productivity;

public class PrefetchAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly IAsyncEnumerable<T> collection;

    public PrefetchAsyncEnumerable(IAsyncEnumerable<T> collection)
    {
        this.collection = collection;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new PrefetchAsyncEnumerator<T>(collection, cancellationToken);
    }
}
