namespace Flaeng.Productivity;

class InterfaceBuilder : BaseBuilder
{
    public InterfaceBuilder(SourceBuilder builder) : base(builder) { }

    public void EndInterface() => builder.AppendEndTag();

    public void AddMethodStub(MethodOptions options)
        => StartConstructorOrMethod(options, isAbstract: false, isStub: true);

}
