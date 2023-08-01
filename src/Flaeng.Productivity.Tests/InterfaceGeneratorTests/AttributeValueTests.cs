namespace Flaeng.Productivity.Tests;

public class AttributeValueTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public AttributeValueTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void TestName()
    {
        // Given
        string source = """
        [Flaeng.GenerateInterface(MemberFinder = MemberFinder.CheckBaseClasses)]
        public partial class Blah
        {
        }
        """;

        // When
        var generator = new InterfaceGenerator();
        _ = compiler.GetGeneratedOutput(
            new[] { generator },
            new SourceFile("dummy.cs", source)
        );

        // Then
        // Assert.Equal(generator.LastData.Options.MemberFinder);
    }
}
