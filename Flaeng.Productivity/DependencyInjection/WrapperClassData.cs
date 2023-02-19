namespace Flaeng.Productivity;

internal struct WrapperClassData
{
    public string Name;
    public TypeVisibility Visibility;

    public WrapperClassData(string Name, TypeVisibility Visibility)
    {
        this.Name = Name;
        this.Visibility = Visibility;
    }

    public static Stack<WrapperClassData> From(ClassDeclarationSyntax cds)
    {
        return cds
            .GetParents()
            .OfType<ClassDeclarationSyntax>()
            .Select(x => new WrapperClassData(x.GetClassName(), x.GetTypeVisiblity()))
            .ToStack();
    }
}
