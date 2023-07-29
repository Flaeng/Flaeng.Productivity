namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class NonClassTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;

    public NonClassTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void Wont_run_for_structs()
    {
        // Given
        string source = """
        namespace Test
        {
            public struct TestStruct
            {
                [Flaeng.Inject] public int MyProperty { get; set; }
            }
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("TestStruct.cs", source)
        );

        // Then
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());
        Assert.Empty(output.Diagnostic);
    }
    
    [Fact(Timeout = 1000)]
    public void Wont_run_for_interfaces()
    {
        // Given
        string source = """
        namespace Test
        {
            public interface TestInterface
            {
                [Flaeng.Inject] public int MyProperty { get; set; }
            }
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("TestStruct.cs", source)
        );

        // Then
        Assert.Empty(output.GeneratedFiles.ExcludeTriggerAttribute());
        Assert.Empty(output.Diagnostic);
    }

}
