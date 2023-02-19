namespace Flaeng.Productivity;

class PropertyComparer : IEqualityComparer<IPropertySymbol>
{
    public static PropertyComparer Instance = new();

    public bool Equals(IPropertySymbol x, IPropertySymbol y)
    {
        return x.IsStatic == y.IsStatic
            && x.Name == y.Name
            && SymbolEqualityComparer.Default.Equals(x.Type, y.Type)
            && CompareMethods(x.GetMethod, y.GetMethod)
            && CompareMethods(x.SetMethod, y.SetMethod);
    }

    private static bool CompareMethods(IMethodSymbol? x, IMethodSymbol? y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return MethodComparer.Instance.Equals(x, y);
    }

    public int GetHashCode(IPropertySymbol obj)
    {
        throw new NotImplementedException();
    }
}
