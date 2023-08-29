namespace Flaeng.Extensions;

public class Attempt<TResult, TError> : IAttempt
{
    public static Attempt<TResult, TError> Failed(TError error)
        => new(didSucceed: false, result: default, error);

    public static Attempt<TResult, TError> Success(TResult result)
        => new(didSucceed: true, result, error: default);

    public bool DidSucceed { get; init; }

    [MemberNotNullWhen(true, nameof(DidSucceed))]
    public TResult? Result { get; init; }

    [MemberNotNullWhen(false, nameof(DidSucceed))]
    public TError? Error { get; init; }

    protected Attempt(bool didSucceed, TResult? result, TError? error)
    {
        this.DidSucceed = didSucceed;
        this.Result = result;
        this.Error = error;
    }

    public static implicit operator Attempt<TResult, TError>(TResult result)
        => Success(result);

    public static implicit operator Attempt<TResult, TError>(TError error)
        => Failed(error);
}
