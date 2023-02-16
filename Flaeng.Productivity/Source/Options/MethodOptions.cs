namespace Flaeng.Productivity;

class MethodOptions : FunctionOptions
{
    public string ReturnType { get; set; }
    public bool Static { get; set; } = false;
    public MethodOptions(string returnType, string name) : base(name)
    {
        this.ReturnType = returnType;
    }
}
