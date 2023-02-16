namespace Flaeng.Productivity;

abstract class TypeOptions
{
    public TypeVisiblity Visibility { get; set; } = TypeVisiblity.Public;
    public bool Static { get; set; } = false;
    public bool Partial { get; set; } = false;
    public abstract string TypeName { get; }
    public string Name { get; set; }
    public string[] Interfaces { get; set; } = new string[0];

    public TypeOptions(string name)
    {
        this.Name = name;
    }
}
