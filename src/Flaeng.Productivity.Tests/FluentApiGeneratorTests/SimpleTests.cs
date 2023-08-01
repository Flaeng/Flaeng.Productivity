namespace Flaeng.Productivity.Tests.FluentApiGeneratorTests;

public class SimpleTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public SimpleTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void Can_make_startup_extension_with_one_class()
    {
        // Given
        string source = """
        [Flaeng.MakeFluent]
        public class Blah 
        { 
            public string Name;
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

