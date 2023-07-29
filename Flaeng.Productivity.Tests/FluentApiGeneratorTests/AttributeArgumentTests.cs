namespace Flaeng.Productivity.Tests.FluentApiGeneratorTests;

public class AttributeArgumentTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public AttributeArgumentTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Theory]
    [InlineData("[Flaeng.MakeFluent]")]
    [InlineData("[Flaeng.MakeFluent()]")]
    [InlineData("[global::Flaeng.MakeFluent]")]
    [InlineData("[global::Flaeng.MakeFluent()]")]
    [InlineData("""
    using Flaeng;
    [MakeFluent()]
    """)]
    [InlineData("""
    using Flaeng;
    [MakeFluent]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.MakeFluent()]
    """)]
    [InlineData("""
    using Test = Flaeng;
    [Test.MakeFluent]
    """)]
    public void Can_read_attribute(string attributeLine)
    {
        // Given
        string source = $$"""
        {{attributeLine}}
        public class Blah 
        { 
            public string Name { get; set; }
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<FluentApiGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Then
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        public static partial class BlahExtensions
        {
            {{Constants.GeneratedCodeAttribute}}
            public static Blah Name(
                this Blah _this,
                global::System.String Name
            )
            {
                _this.Name = Name;
                return _this;
            }
        }
        
        """;
        var files = output.GeneratedFiles.ExcludeTriggerAttribute();
        Assert.Single(files);
        var file = files.Single();
        Assert.Equal(Constants.FluentApiGeneratorGeneratedContentPathPrefix + "Blah.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }
    
}
