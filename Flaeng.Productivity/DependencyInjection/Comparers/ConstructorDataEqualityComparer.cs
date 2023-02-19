namespace Flaeng.Productivity;

internal class ConstructorDataEqualityComparer : IEqualityComparer<ConstructorData>
{
    public static ConstructorDataEqualityComparer Instance = new();

    public bool Equals(ConstructorData x, ConstructorData y)
    {
        return x.Class == y.Class
            && x.Members.SequenceEqual(y.Members)
            && x.WrapperClasses.SequenceEqual(y.WrapperClasses);
    }

    public int GetHashCode(ConstructorData obj)
    {
        throw new NotImplementedException();
    }
}
