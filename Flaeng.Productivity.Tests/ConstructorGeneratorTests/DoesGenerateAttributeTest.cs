namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

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
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>();

        // Assert
        string expected_output = $$"""
        {{Constants.TriggerAttributeContentPrefix}}
        
        namespace Flaeng
        {
            [global::System.AttributeUsageAttribute(
                global::System.AttributeTargets.Property | global::System.AttributeTargets.Field, 
                AllowMultiple = false, 
                Inherited = false)]
            {{Constants.GeneratedCodeAttribute}}
            internal sealed class InjectAttribute : global::System.Attribute
            {
            }
        }
        """;
        var source = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("InjectAttribute.g.cs"));
        Assert.Equal(expected_output, source?.Content);
        Assert.Empty(output.Diagnostic);
    }
}
