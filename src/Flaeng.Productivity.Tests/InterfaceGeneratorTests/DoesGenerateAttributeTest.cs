namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class DoesGenerateAttributeTest : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public DoesGenerateAttributeTest(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void Does_generate_attribute()
    {
        // Arrange

        // Act
        var output = compiler.GetGeneratedOutput<InterfaceGenerator>();

        // Assert
        string expected_output = $$"""
        {{Constants.TriggerAttributeContentPrefix}}
        
        namespace Flaeng
        {
            {{Constants.GeneratedCodeAttribute}}
            internal enum Visibility { Default, Public, Internal }

            [global::System.AttributeUsageAttribute(
                global::System.AttributeTargets.Class,
                AllowMultiple = false,
                Inherited = false)]
            {{Constants.GeneratedCodeAttribute}}
            internal sealed class GenerateInterfaceAttribute : global::System.Attribute
            {
                public string? Name = null;
                public Visibility Visibility = Visibility.Default;
            }
        }
        """;
        Assert.Single(output.GeneratedFiles);
        var file = output.GeneratedFiles.SingleOrDefault();
        Assert.NotNull(file);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }

}
