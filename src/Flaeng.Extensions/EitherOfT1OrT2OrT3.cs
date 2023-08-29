namespace Flaeng.Extensions;

public class Either<T1, T2, T3>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull
{
#pragma warning disable CS8603
    public object Value
        => ValueIsOfTIndex switch
        {
            0 => Value1,
            1 => Value2,
            2 => Value3,
            _ => throw new Exception()
        };
#pragma warning restore CS8603

    private int ValueIsOfTIndex { get; set; }

    protected T1? Value1 { get; }

    protected T2? Value2 { get; }

    protected T3? Value3 { get; }

    public Either(T1 value)
    {
        this.Value1 = value;
        this.ValueIsOfTIndex = 0;
    }

    public Either(T2 value)
    {
        this.Value2 = value;
        this.ValueIsOfTIndex = 1;
    }

    public Either(T3 value)
    {
        this.Value3 = value;
        this.ValueIsOfTIndex = 2;
    }

    public T1? AsT1() => Value1;

    public T2? AsT2() => Value2;

    public T3? AsT3() => Value3;

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

    public bool Is([NotNullWhen(true)] out T3? value)
    {
        value = Value3;
        return ValueIsOfTIndex == 2;
    }

    public void Case(Action<T1> action1, Action<T2> action2, Action<T3> action3)
    {
        Case<object?>(v => { action1(v); return null; }, v => { action2(v); return null; }, v => { action3(v); return null; });
    }

    public void Case(Action<T1> action)
    {
        Case<object?>(v => { action(v); return null; }, _ => null, _ => null);
    }

    public void Case(Action<T2> action)
    {
        Case<object?>(_ => null, v => { action(v); return null; }, _ => null);
    }

    public void Case(Action<T3> action)
    {
        Case<object?>(_ => null, _ => null, v => { action(v); return null; });
    }

#pragma warning disable CS8604
    public TResult Case<TResult>(Func<T1, TResult> action1, Func<T2, TResult> action2, Func<T3, TResult> action3)
    {
        return ValueIsOfTIndex == 0 ? action1(Value1)
            : ValueIsOfTIndex == 1 ? action2(Value2)
            : ValueIsOfTIndex == 2 ? action3(Value3)
            : throw new Exception();
    }
#pragma warning restore CS8604

    public TResult? Case<TResult>(Func<T1, TResult> action)
    {
        return Case<TResult?>(action, _ => default, _ => default);
    }

    public TResult? Case<TResult>(Func<T2, TResult> action)
    {
        return Case<TResult?>(_ => default, action, _ => default);
    }

    public TResult? Case<TResult>(Func<T3, TResult> action)
    {
        return Case<TResult?>(_ => default, _ => default, action);
    }

#pragma warning disable CS8604
    public Either<TResult, T2, T3> Map<TResult>(Func<T1, TResult> mapper)
        where TResult : notnull
    {
        return ValueIsOfTIndex == 0 ? mapper(Value1)
            : ValueIsOfTIndex == 1 ? Value2
            : ValueIsOfTIndex == 2 ? Value3
            : throw new Exception();
    }

    public Either<T1, TResult, T3> Map<TResult>(Func<T2, TResult> mapper)
        where TResult : notnull
    {
        return ValueIsOfTIndex == 0 ? Value1
            : ValueIsOfTIndex == 1 ? mapper(Value2)
            : ValueIsOfTIndex == 2 ? Value3
            : throw new Exception();
    }

    public Either<T1, T2, TResult> Map<TResult>(Func<T3, TResult> mapper)
        where TResult : notnull
    {
        return ValueIsOfTIndex == 0 ? Value1
            : ValueIsOfTIndex == 1 ? Value2
            : ValueIsOfTIndex == 2 ? mapper(Value3)
            : throw new Exception();
    }
#pragma warning restore CS8604

    public static implicit operator Either<T1, T2, T3>(T1 value) => new(value);
    public static implicit operator Either<T1, T2, T3>(T2 value) => new(value);
    public static implicit operator Either<T1, T2, T3>(T3 value) => new(value);
}
