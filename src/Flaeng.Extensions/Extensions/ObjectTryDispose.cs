namespace Flaeng.Extensions;

public static class ObjectTryDispose
{
    public static bool TryDispose<T>(this T obj)
    {
        if (obj is not IDisposable dis)
            return false;

        dis.Dispose();
        return true;
    }
}
