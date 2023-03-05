namespace Flaeng.Productivity;

class ConstructorOptions : FunctionOptions
{
    public bool CallBase { get; set; }
    public List<string> BaseParameters { get; set; } = new();
    public ConstructorOptions(string name, bool callBase, List<string> baseParameters) : base(name)
    {
        this.CallBase = callBase;
        this.BaseParameters = baseParameters;
    }
}
