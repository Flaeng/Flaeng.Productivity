namespace Flaeng.Productivity;

internal enum TypeVisiblity { Public, Internal, Private }
class ClassOptions : TypeOptions
{
    public override string TypeName => "class";
    public ClassOptions(string name) : base(name) { }
}
class InterfaceOptions : TypeOptions
{
    public override string TypeName => "interface";
    public InterfaceOptions(string name) : base(name) { }
}
class StructOptions : TypeOptions
{
    public override string TypeName => "struct";
    public StructOptions(string name) : base(name) { }
}
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
