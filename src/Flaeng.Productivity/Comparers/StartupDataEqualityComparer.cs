namespace Flaeng.Productivity.Comparers;

internal class StartupDataEqualityComparer : EqualityComparerBase<StartupGenerator.Data, StartupDataEqualityComparer>
{
    public override bool Equals(StartupGenerator.Data x, StartupGenerator.Data y)
    {
        return SameLength(x.Diagnostics, y.Diagnostics)
            && SequenceEqual(x.Injectables, y.Injectables, StartupInjectDataEqualityComparer.Instance)
            && x.Namespace == y.Namespace;
    }

    public override int GetHashCode(StartupGenerator.Data obj)
    {
        return CalculateHashCode(
            obj.Diagnostics,
            GetHashCode(obj.Injectables, StartupInjectDataEqualityComparer.Instance),
            obj.Namespace
        );
    }
}
