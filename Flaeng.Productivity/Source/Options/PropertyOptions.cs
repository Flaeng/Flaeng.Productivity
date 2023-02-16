namespace Flaeng.Productivity;

class PropertyOptions : MemberOptions
{
    public GetterSetterVisiblity Getter { get; set; } = GetterSetterVisiblity.Inherited;
    public GetterSetterVisiblity Setter { get; set; } = GetterSetterVisiblity.Inherited;
    public PropertyOptions(string type, string name) : base(type, name) { }
}
