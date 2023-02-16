using System.Collections.Generic;

namespace Flaeng.Productivity;

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
