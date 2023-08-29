namespace Flaeng.Extensions;

public class Attempt<T> : IAttempt //where T : notnull
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

    public static implicit operator Attempt<T>(T result)
        => Success(result);
}
