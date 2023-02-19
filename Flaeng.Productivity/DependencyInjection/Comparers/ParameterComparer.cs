namespace Flaeng.Productivity;

class ParameterComparer : IEqualityComparer<IParameterSymbol>
{
    public static ParameterComparer Instance = new();

    public bool Equals(IParameterSymbol x, IParameterSymbol y)
    {
        return SymbolEqualityComparer.Default.Equals(x.Type, y.Type)
            && x.Name == y.Name
            && x.RefKind == y.RefKind;
    }

    public int GetHashCode(IParameterSymbol obj)
    {
        throw new NotImplementedException();
    }
}
