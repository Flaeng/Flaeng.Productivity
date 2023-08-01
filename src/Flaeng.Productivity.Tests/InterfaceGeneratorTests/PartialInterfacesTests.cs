namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class PartialInterfacesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public PartialInterfacesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_generate_partial_interface_with_another_name()
    {
        // Arrange
        string source1 = """
        namespace TestNamespace.Providers
        {
            public interface IDummyTest 
            {
                bool Simple(string text, out int number);
            }
            [Flaeng.GenerateInterface]
            public partial class Dummy : IDummyTest, IDummyTester
            {
                public string Header { get; }
                public string Paragraph { get; }
                public bool Simple(string text, out int number) { number = 0; return true; }
                public bool Advanced(string text) { return true; }
                public int Expert(string text, ref int number) { number = 0; return 1; }
            }
        }
        """;
        string source2 = """
        namespace TestNamespace.Providers;
        
        public interface IDummyTester 
        {
            string Header { get; }
            int Expert(string text, ref int number);
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy1.cs", source1),
            new SourceFile("dummy2.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                global::System.String Header { get; }
                global::System.String Paragraph { get; }
                global::System.Boolean Simple(
                    global::System.String text,
                    out global::System.Int32 number
                );
                global::System.Boolean Advanced(
                    global::System.String text
                );
                global::System.Int32 Expert(
                    global::System.String text,
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
    public void can_generate_partial_interface()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            public interface IDummy
            {
                bool Simple(string text, out int number);
            }
            [Flaeng.GenerateInterface]
            public partial class Dummy : IDummy
            {
                public bool Simple(string text, out int number) { number = 0; return true; }
                public bool Advanced(string text) { return true; }
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
            public interface IDummy2
            {
                global::System.Boolean Simple(
                    global::System.String text,
                    out global::System.Int32 number
                );
                global::System.Boolean Advanced(
                    global::System.String text
                );
            }
            public partial class Dummy : IDummy2
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
    public void can_generate_partial_interface_with_user_generated_code()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            public partial interface IDummy
            {
                bool Simple(string text, out int number);
            }
            [Flaeng.GenerateInterface]
            public partial class Dummy : IDummy
            {
                public bool Simple(string text, out int number) { number = 0; return true; }
                public bool Advanced(string text) { return true; }
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
            public partial interface IDummy
            {
                global::System.Boolean Advanced(
                    global::System.String text
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

}
