namespace Flaeng.Productivity;

internal class InterfaceDataEqualityComparer : IEqualityComparer<InterfaceData>
{
    public static InterfaceDataEqualityComparer Instance = new();

    public bool Equals(InterfaceData x, InterfaceData y)
    {
        return x.Class == y.Class
            && x.Members.SequenceEqual(y.Members)
            && x.Methods.SequenceEqual(y.Methods)
            && x.InterfaceNames.SequenceEqual(y.InterfaceNames);
    }

    public int GetHashCode(InterfaceData obj)
    {
        throw new NotImplementedException();
    }
}
