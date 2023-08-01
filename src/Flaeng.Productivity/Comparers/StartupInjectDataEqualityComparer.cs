namespace Flaeng.Productivity.Comparers;

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
        return CalculateHashCode(
            obj.InjectType,
            GetHashCode(obj.Interfaces, StringComparer.InvariantCulture),
            obj.TypeName.GetHashCode()
        );
    }
}
