namespace Flaeng.Extensions;

public interface ICloneable<T> where T : ICloneable<T> { }
public static class ICloneableClone
{
    public static T Clone<T>(this T source) where T : ICloneable<T>, new()
        => (T)Clone((object)source);

    public static object Clone(this object source)
    {
        var type = source.GetType();
        var result = Activator.CreateInstance(type);
        if (result is null)
            throw new CloneException($"Failed to create instance of type {type}");

        foreach (var prop in type.GetProperties().Where(prop => prop.CanRead && prop.CanWrite))
        {
            var value = prop.GetValue(source);
            prop.SetValue(result, value);
        }
        foreach (var field in type.GetFields())
        {
            var value = field.GetValue(source);
            field.SetValue(result, value);
        }
        return result;
    }

    public static T DeepClone<T>(this T source, bool recursive) where T : ICloneable<T>, new()
        => source.DeepClone(recursive ? int.MaxValue : 0);

    public static T DeepClone<T>(this T source, int depth) where T : ICloneable<T>, new()
        => (T)DeepClone(source, typeof(T), depth);

    private static object DeepClone(object source, Type type, int depth)
    {
        if (type.Namespace == "System" || depth < 0)
            return source;

        var result = Activator.CreateInstance(type);
        if (result is null)
            throw new CloneException($"Failed to create instance of type {type}");

        foreach (var prop in type.GetProperties().Where(prop => prop.CanRead && prop.CanWrite))
        {
            var value = prop.GetValue(source);
            if (value is null)
                continue;
            value = DeepClone(value, value.GetType(), depth - 1);
            prop.SetValue(result, value);
        }
        foreach (var field in type.GetFields())
        {
            var value = field.GetValue(source);
            if (value is null)
                continue;
            value = DeepClone(value, value.GetType(), depth - 1);
            field.SetValue(result, value);
        }
        return result;
    }
}

[System.Serializable]
public class CloneException : System.Exception
{
    public CloneException() { }
    public CloneException(string message) : base(message) { }
    public CloneException(string message, System.Exception inner) : base(message, inner) { }
    protected CloneException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
