using System.Collections.Generic;

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
class ConstructorOptions : FunctionOptions
{
    public ConstructorOptions(string name) : base(name) { }
}
class FunctionOptions
{
    public string Name { get; set; }
    public MemberVisiblity Visibility { get; set; } = MemberVisiblity.None;
    public List<string> Parameters { get; set; } = new List<string>();
    public FunctionOptions(string name)
    {
        this.Name = name;
    }
}
