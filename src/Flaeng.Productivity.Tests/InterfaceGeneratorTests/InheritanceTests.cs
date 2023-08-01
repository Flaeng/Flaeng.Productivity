namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class InheritanceTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public InheritanceTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void will_add_inherited_methods_to_interface()
    {
        // Arrange
        string source1 = """
        namespace TestNamespace.Providers
        {
            public abstract class BaseBase
            {
                public int Math(string text) { return 1; }
            }
        }
        """;
        string source2 = """
        namespace TestNamespace.Providers
        {
            public abstract class DummyBase : BaseBase
            {
                public bool Simple1(string text) { return true; }
                public virtual bool Simple2(string text, out int number) { number = 0; return true; }
            }
        }
        """;
        string source3 = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy : DummyBase
            {
                public override bool Simple2(string text, out int number) { number = 0; return true; }
                public bool Simple3(string text1, string text2, out int number) { number = 0; return true; }
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("BaseBase.cs", source1),
            new SourceFile("DummyBase.cs", source2),
            new SourceFile("Dummy.cs", source3)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Providers
        {
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                global::System.Boolean Simple3(
                    global::System.String text1,
                    global::System.String text2,
                    out global::System.Int32 number
                );
                global::System.Boolean Simple1(
                    global::System.String text
                );
                global::System.Boolean Simple2(
                    global::System.String text,
                    out global::System.Int32 number
                );
                global::System.Int32 Math(
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
