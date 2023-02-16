namespace Flaeng.Productivity;

internal enum MemberVisiblity { Public, Internal, Protected, Private, None }
internal enum GetterSetterVisiblity { Inherited, Public, Internal, Protected, Private, None }
class PropertyOptions : MemberOptions
{
    public GetterSetterVisiblity Getter { get; set; } = GetterSetterVisiblity.Inherited;
    public GetterSetterVisiblity Setter { get; set; } = GetterSetterVisiblity.Inherited;
    public PropertyOptions(string type, string name) : base(type, name) { }
}
class FieldOptions : MemberOptions
{
    public FieldOptions(string type, string name) : base(type, name) { }
}

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
