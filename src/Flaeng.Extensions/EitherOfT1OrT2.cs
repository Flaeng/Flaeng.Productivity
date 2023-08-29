namespace Flaeng.Extensions;

public class Either<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
#pragma warning disable CS8603
    public object Value
        => ValueIsOfTIndex switch
        {
            0 => Value1,
            1 => Value2,
            _ => throw new Exception()
        };
#pragma warning restore CS8603

    private int ValueIsOfTIndex { get; set; }

    protected T1? Value1 { get; }

    protected T2? Value2 { get; }

    public Either(T1 value)
    {
        Value1 = value;
        this.ValueIsOfTIndex = 0;
    }

    public Either(T2 value)
    {
        Value2 = value;
        this.ValueIsOfTIndex = 1;
    }

    public T1? AsT1() => Value1;

    public T2? AsT2() => Value2;

    public bool Is([NotNullWhen(true)] out T1? value)
    {
        value = Value1;
        return ValueIsOfTIndex == 0;
    }

    public bool Is([NotNullWhen(true)] out T2? value)
    {
        value = Value2;
        return ValueIsOfTIndex == 1;
    }

    public void Case(Action<T1> action1, Action<T2> action2)
    {
        Case<object?>(v => { action1(v); return null; }, v => { action2(v); return null; });
    }

    public void Case(Action<T1> action)
    {
        Case<object?>(v => { action(v); return null; }, _ => null);
    }

    public void Case(Action<T2> action)
    {
        Case<object?>(_ => null, v => { action(v); return null; });
    }

    public TResult Case<TResult>(Func<T1, TResult> action1, Func<T2, TResult> action2)
    {
        return Value1 is not null ? action1(Value1)
            : Value2 is not null ? action2(Value2)
            : throw new Exception();
    }

    public TResult? Case<TResult>(Func<T1, TResult> action)
    {
        return Case<TResult?>(action, _ => default);
    }

    public TResult? Case<TResult>(Func<T2, TResult> action)
    {
        return Case<TResult?>(_ => default, action);
    }

    public Either<TResult, T2> Map<TResult>(Func<T1, TResult> mapper)
        where TResult : notnull
    {
        return Value1 is not null ? new Either<TResult, T2>(mapper(Value1))
            : Value2 is not null ? new Either<TResult, T2>(Value2)
            : throw new Exception();
    }

    public Either<T1, TResult> Map<TResult>(Func<T2, TResult> mapper)
        where TResult : notnull
    {
        return Value1 is not null ? new Either<T1, TResult>(Value1)
            : Value2 is not null ? new Either<T1, TResult>(mapper(Value2))
            : throw new Exception();
    }

    public static implicit operator Either<T1, T2>(T1 value)
        => new Either<T1, T2>(value);

    public static implicit operator Either<T1, T2>(T2 value)
        => new Either<T1, T2>(value);
}
