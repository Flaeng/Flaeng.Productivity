namespace Flaeng.Extensions;

public interface IAttempt
{
    bool DidSucceed { get; }
}
public class Attempt : IAttempt
{
    public static Attempt Failed => new(didSucceed: false);
    public static Attempt Success => new(didSucceed: true);
    public bool DidSucceed { get; init; }

    protected Attempt(bool didSucceed)
    {
        this.DidSucceed = didSucceed;
    }
}
