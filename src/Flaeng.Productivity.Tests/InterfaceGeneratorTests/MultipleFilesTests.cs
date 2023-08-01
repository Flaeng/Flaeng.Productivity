namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class MultipleFilesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleFilesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void wont_generate_methods_for_inherited_methods()
    {
        // Arrange
        string source1 = """
        namespace TestNamespace.Providers
        {
            public partial class Dummy
            {
                public bool Simple(string text, out int number) { number = 0; return true; }
            }
        }
        """;
        string source2 = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class DumDum : Dummy
            {
                public bool Simple(out int number) { number = 0; return true; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source1),
            new SourceFile("dumdum.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}

        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDumDum
            {
                global::System.Boolean Simple(
                    out global::System.Int32 number
                );
                global::System.Boolean Simple(
                    global::System.String text,
                    out global::System.Int32 number
                );
            }
            public partial class DumDum : IDumDum
            {
            }
        }
        
        """;
        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDumDum.g.cs"));
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void will_generate_methods_for_methods_in_different_file_same_class()
    {
        // Arrange
        string source1 = """
        namespace TestNamespace.Providers
        {
            public partial class DumDum
            {
                public bool Simple(string text, out int number) { number = 0; return true; }
            }
        }
        """;
        string source2 = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class DumDum
            {
                public bool Simple(out int number) { number = 0; return true; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source1),
            new SourceFile("dumdum.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}

        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDumDum
            {
                global::System.Boolean Simple(
                    global::System.String text,
                    out global::System.Int32 number
                );
                global::System.Boolean Simple(
                    out global::System.Int32 number
                );
            }
            public partial class DumDum : IDumDum
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
