namespace Flaeng.Extensions;

public interface IAttempt
{
    bool DidSucceed { get; }
}
public interface IAttempt<TThis> : IAttempt where TThis : IAttempt<TThis>
{
    TThis Then(Func<TThis> doThis);
}
public class Attempt : IAttempt<Attempt>
{
    public static Attempt Failed => new(didSucceed: false);
    public static Attempt Success => new(didSucceed: true);
    public bool DidSucceed { get; init; }

    protected Attempt(bool didSucceed)
    {
        this.DidSucceed = didSucceed;
    }

    public Attempt Then(Func<Attempt> doThis)
    {
        if (DidSucceed == false)
            return Failed;

        return doThis();
    }
    
    public Attempt Then(Action doThis)
    {
        if (DidSucceed == false)
            return this;

        doThis();
        return this;
    }

}
