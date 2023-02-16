namespace Flaeng.Productivity;

abstract class MemberOptions
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string? DefaultValue { get; set; } = null;
    public MemberVisiblity Visibility { get; set; } = MemberVisiblity.None;
    public MemberOptions(string type, string name)
    {
        this.Type = type;
        this.Name = name;
    }
}
