namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class AsyncMethodTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public AsyncMethodTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_handle_async_methods()
    {
        // Arrange
        string source = """
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;

        namespace TestNamespace.Providers
        {
            public class User
            {
                public string Username { get; }
            }
            public class Page<T>
            {
                public List<T> Items { get; }
            }
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public async Task<Page<User>> SimpleAsync(string text, CancellationToken token) 
                { 
                    await Task.Yield();
                    return new Page<User>(); 
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
            {{Constants.GeneratedCodeAttribute}}
            public interface IDummy
            {
                global::System.Threading.Tasks.Task<global::TestNamespace.Providers.Page<global::TestNamespace.Providers.User>> SimpleAsync(
                    global::System.String text,
                    global::System.Threading.CancellationToken token
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
