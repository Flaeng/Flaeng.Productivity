namespace Flaeng.Extensions;

public static class ObjectTryConvert
{
    public static Attempt<object> TryConvert(this object value, Type targetType)
    {
        if (targetType is null)
            return Attempt<object>.Failed;

        try
        {
            var result = Convert.ChangeType(value, targetType);
            return Attempt<object>.Success(result);
        }
        catch
        {
            return Attempt<object>.Failed;
        }
    }

    public static Attempt<T> TryConvert<T>(this object value)
    {
        try
        {
            var result = (T)Convert.ChangeType(value, typeof(T));
            return Attempt<T>.Success(result);
        }
        catch
        {
            return Attempt<T>.Failed;
        }
    }

    public static bool TryConvert(this object value, Type targetType, [NotNullWhen(true)] out object? result)
    {
        result = default;
        if (targetType is null)
            return false;

        try
        {
            result = Convert.ChangeType(value, targetType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryConvert<T>(this object value, [NotNullWhen(true)] out T? result)
    {
        try
        {
            result = (T)Convert.ChangeType(value, typeof(T));
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
