namespace Flaeng.Productivity.Tests.ExtensionsTests;

public class ObjectTryDisposeTests
{
    class CanDispose : IDisposable
    {
        public bool WasDisposed { get; private set; } = false;
        public void Dispose() => WasDisposed = true;
    }
    class CannotDispose
    {
        public bool WasDisposed { get; private set; } = false;
        public void Dispose() => WasDisposed = true;
    }

    [Fact]
    public void Can_dispose_idisposable()
    {
        // Given
        var obj = new CanDispose();

        // When
        var didDispose = obj.TryDispose();

        // Then
        Assert.True(didDispose);
        Assert.True(obj.WasDisposed);
    }

    [Fact]
    public void Wont_call_dispose_on_non_disposable()
    {
        // Given
        var obj = new CannotDispose();

        // When
        var didDispose = obj.TryDispose();

        // Then
        Assert.False(didDispose);
        Assert.False(obj.WasDisposed);
    }
}
