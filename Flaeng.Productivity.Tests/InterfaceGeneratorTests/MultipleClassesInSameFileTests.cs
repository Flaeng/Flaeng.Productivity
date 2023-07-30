namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class MultipleClassesInSameFileTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleClassesInSameFileTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_make_multiple_interfaces_when_defined_in_one_file()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public void Simple() { }
                public bool Simple(int number) { return true; }
            }
            [Flaeng.GenerateInterface]
            public partial class DumDum
            {
                public void Second() { }
                public bool Second(string text) { return true; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output1 = $$"""
        {{Constants.GeneratedContentPrefix}}

        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                void Simple();
                global::System.Boolean Simple(
                    global::System.Int32 number
                );
            }
            public partial class Dummy : IDummy
            {
            }
        }

        """;
        string expected_output2 = $$"""
        {{Constants.GeneratedContentPrefix}}

        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDumDum
            {
                void Second();
                global::System.Boolean Second(
                    global::System.String text
                );
            }
            public partial class DumDum : IDumDum
            {
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output1, dummyGenerated.Content);

        var dumdumGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDumDum.g.cs"));
        Assert.NotNull(dumdumGenerated);
        Assert.Equal(expected_output2, dumdumGenerated.Content);

        Assert.Empty(output.Diagnostic);
    }

}
