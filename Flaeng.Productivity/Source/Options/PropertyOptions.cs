namespace Flaeng.Productivity;

class PropertyOptions : MemberOptions
{
    public GetterSetterVisiblity Getter { get; set; } = GetterSetterVisiblity.Default;
    public GetterSetterVisiblity Setter { get; set; } = GetterSetterVisiblity.Default;
    public PropertyOptions(string type, string name) : base(type, name) { }
}
