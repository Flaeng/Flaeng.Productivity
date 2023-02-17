namespace Flaeng.Productivity;

class ClassOptions : TypeOptions
{
    public override string TypeName => "class";
    public bool Abstract { get; set; } = false;
    public ClassOptions(string name) : base(name) { }
}
