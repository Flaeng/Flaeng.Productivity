namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class NestedClassesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public NestedClassesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_nested_classes()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    [Flaeng.GenerateInterface]
                    public partial class Dummy
                    {
                        public bool Simple(string text, out int number) { number = 0; return true; }
                    }
                }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            public partial class Wrapper1
            {
                public partial class Wrapper2
                {
                    {{Constants.GeneratedCodeAttribute}}
                    public interface IDummy
                    {
                        global::System.Boolean Simple(
                            global::System.String text,
                            out global::System.Int32 number
                        );
                    }
                    public partial class Dummy : IDummy
                    {
                    }
                }
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.EndsWith("TestNamespace.Providers.Wrapper1.Wrapper2.IDummy.g.cs", dummyGenerated.Filename);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

}
