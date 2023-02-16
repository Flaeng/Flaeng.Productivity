namespace Flaeng.Productivity;

class ClassBuilder : BaseBuilder
{
    public ClassBuilder(SourceBuilder builder) : base(builder) { }

    public void EndConstructor() => builder.AppendEndTag();

    public void EndClass() => builder.AppendEndTag();

    public void StartConstructor(ConstructorOptions options)
        => StartConstructorOrMethod(options, isAbstract: false, isStub: false);

    public void AddMethodStub(MethodOptions options)
        => StartConstructorOrMethod(options, isAbstract: true, isStub: true);

    public void StartMethod(MethodOptions options)
        => StartConstructorOrMethod(options, isAbstract: false, isStub: false);

}
