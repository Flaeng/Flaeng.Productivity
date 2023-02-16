namespace Flaeng.Productivity;

class StructBuilder : BaseBuilder
{
    public StructBuilder(SourceBuilder builder) : base(builder) { }

    public void EndStruct() => builder.AppendEndTag();
}
