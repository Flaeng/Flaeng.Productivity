namespace Flaeng.Productivity;

class FunctionOptions
{
    public string Name { get; set; }
    public MemberVisibility Visibility { get; set; } = MemberVisibility.Default;
    public List<string> Parameters { get; set; } = new List<string>();
    public FunctionOptions(string name)
    {
        this.Name = name;
    }
}
