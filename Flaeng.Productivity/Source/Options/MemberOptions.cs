namespace Flaeng.Productivity;

abstract class MemberOptions
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string? DefaultValue { get; set; } = null;
    public bool Static { get; set; } = false;
    public MemberVisibility Visibility { get; set; } = MemberVisibility.Default;
    public MemberOptions(string type, string name)
    {
        this.Type = type;
        this.Name = name;
    }
}
