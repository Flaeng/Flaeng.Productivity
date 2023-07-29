namespace Flaeng.Productivity.Comparers;

internal class IMemberDefinitionEqualityComparer : IEqualityComparer<IMemberDefinition>
{
    public static readonly IMemberDefinitionEqualityComparer Instance = new();

    public bool Equals(IMemberDefinition x, IMemberDefinition y)
    {
        return x switch
        {
            FieldDefinition fdx
                => y is FieldDefinition fdy
                && FieldDefinitionEqualityComparer.Instance.Equals(fdx, fdy),

            PropertyDefinition pdx
                => y is PropertyDefinition pdy
                && PropertyDefinitionEqualityComparer.Instance.Equals(pdx, pdy),

            MethodDefinition mdx
                => y is MethodDefinition mdy
                && MethodDefinitionEqualityComparer.Instance.Equals(mdx, mdy),

            _ => false
        };
    }

    public int GetHashCode(IMemberDefinition obj)
    {
        return obj.GetHashCode();
    }
}
