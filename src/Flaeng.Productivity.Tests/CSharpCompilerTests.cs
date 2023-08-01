namespace Flaeng.Productivity.Tests;

// You wont believe have much time i spend trying to get the correct
// behavior out of this class
// Missing compilation errors...
// Compilation errors I didn't see when running dotnet build...
// That is why i test a class i only use for testing... To make sure it behaves as it should
public class CSharpCompilerTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public CSharpCompilerTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Theory]
    [InlineData("public class Test { private string blah; }")]
    [InlineData("public class Test { public async System.Threading.Tasks.Task<int> BlahAsync() { return -1; } }")]
    public void Gives_compilation_warning(string sourceText)
    {
        // Given
        var source = new SourceFile("DoesntMatter.cs", sourceText);

        // When
        var compilation = compiler.GetGeneratedOutput(Array.Empty<IIncrementalGenerator>(), source);

        // Then
        Assert.Empty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Hidden));
        Assert.Empty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Info));
        Assert.NotEmpty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Warning));
    }

    [Theory]
    [InlineData("public class Test { private string; }")]
    [InlineData("public class Test private string blah }")]
    [InlineData("publc class Test { }")]
    [InlineData("nameSpacy Lalala { }")]
    // [InlineData("public class Test { public async System.Wrong.Namespace.Task<int> BlahAsync() { return -1; } }")] HOW IS THIS NOT AN ERROR, BUT SIMPLY A WARNING ???
    public void Gives_compilation_error(string sourceText)
    {
        // Given
        var source = new SourceFile("DoesntMatter.cs", sourceText);

        // When
        var compilation = compiler.GetGeneratedOutput(Array.Empty<IIncrementalGenerator>(), source);

        // Then
        Assert.NotEmpty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Error));
        Assert.Empty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Hidden));
        Assert.Empty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Info));
        Assert.Empty(compilation.Diagnostic.Where(x => x.Severity == DiagnosticSeverity.Warning));
    }

    [Theory]
    [InlineData("""
    using System.Threading.Tasks;
    namespace MicroSofty
    {
        public class Hey
        {
            public async Task<string> ManyMains(int number) 
            {
                await Task.Yield();
                return number.ToString();
            }
        }
    }
    """)]
    [InlineData("""
    namespace MicroSofty
    {
        public class Hey
        {
            public async System.Threading.Tasks.Task<string> ManyMains(int number) 
            {
                await System.Threading.Tasks.Task.Yield();
                return number.ToString();
            }
        }
    }
    """)]
    public void Compiles_without_errors(string sourceText)
    {
        // Given
        var source = new SourceFile("DoesntMatter.cs", sourceText);

        // When
        var compilation = compiler.GetGeneratedOutput(Array.Empty<IIncrementalGenerator>(), source);

        // Then
        Assert.Empty(compilation.Diagnostic);
    }

}
