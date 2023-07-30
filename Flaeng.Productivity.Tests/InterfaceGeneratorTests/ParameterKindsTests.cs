namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class ParameterKindsTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public ParameterKindsTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_a_method_with_a_normal_and_an_out_parameter()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public bool Simple(string text, out int number) { number = 0; return true; }
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

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_a_method_with_an_out_parameter()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public bool Simple(ref int number) { number = 0; return true; }
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
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                global::System.Boolean Simple(
                    ref global::System.Int32 number
                );
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_handle_params()
    {
        // Arrange
        string source = """
        namespace TestNamespace;
        
        [Flaeng.GenerateInterface]
        public partial class Test
        {
            public void Main(params string[] args) {}
        } 
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface ITest
            {
                void Main(
                    params global::System.String[] args
                );
            }
            public partial class Test : ITest
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }
}
