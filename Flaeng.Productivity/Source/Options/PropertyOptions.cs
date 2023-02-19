namespace Flaeng.Productivity;

class PropertyOptions : MemberOptions
{
    public GetterSetterVisibility Getter { get; set; } = GetterSetterVisibility.Default;
    public GetterSetterVisibility Setter { get; set; } = GetterSetterVisibility.Default;
    public PropertyOptions(string type, string name) : base(type, name) { }
}
