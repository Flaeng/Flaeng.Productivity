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
        return (obj.Diagnostics == default ? 0 : obj.Diagnostics.Length)
            ^ GetHashCode(obj.Injectables, StartupInjectDataEqualityComparer.Instance)
            ^ (obj.Namespace?.GetHashCode() ?? 0);
    }
}

internal class StartupInjectDataEqualityComparer : EqualityComparerBase<StartupGenerator.InjectData, StartupInjectDataEqualityComparer>
{
    public override bool Equals(StartupGenerator.InjectData x, StartupGenerator.InjectData y)
    {
        return x.InjectType == y.InjectType
            && SequenceEqual(x.Interfaces, y.Interfaces, StringComparer.InvariantCulture)
            && x.TypeName == y.TypeName;
    }

    public override int GetHashCode(StartupGenerator.InjectData obj)
    {
        return obj.InjectType.GetHashCode()
            ^ GetHashCode(obj.Interfaces, StringComparer.InvariantCulture)
            ^ obj.TypeName.GetHashCode();
    }
}
