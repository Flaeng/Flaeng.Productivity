namespace Flaeng.Extensions;

public class Attempt<T> : IAttempt
{
    public static Attempt<T> Failed => new(didSucceed: false, result: default);
    public static Attempt<T> Success(T result) => new(didSucceed: true, result);

    public bool DidSucceed { get; init; }

    [MemberNotNullWhen(true, nameof(DidSucceed))]
    public T? Result { get; init; }

    protected Attempt(bool didSucceed, T? result)
    {
        this.DidSucceed = didSucceed;
        this.Result = DidSucceed ? result : default;
    }

    public Attempt<TNewResult> Then<TNewResult>(Func<T, TNewResult> doThis)
    {
        if (DidSucceed == false)
            return new Attempt<TNewResult>(didSucceed: false, result: default);

        if (Result is not null)
            return doThis(Result);

        throw new InvalidOperationException();
    }

    public IAttempt Then<TNewResult>(Func<T, IAttempt> doThis)
    {
        if (DidSucceed == false)
            return this;

        if (Result is not null)
            return doThis(Result);

        throw new InvalidOperationException();
    }

    public static implicit operator Attempt<T>(T result)
        => Success(result);
}
