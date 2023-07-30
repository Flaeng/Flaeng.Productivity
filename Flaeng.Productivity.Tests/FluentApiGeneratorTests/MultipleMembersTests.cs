namespace Flaeng.Productivity.Tests.FluentApiGeneratorTests;

public class MultipleMembersTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleMembersTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact]
    public void Can_handle_multiple_members()
    {
        // Given
        string source = """
        using Flaeng;

        namespace Test
        {
            [Flaeng.MakeFluent]
            public class Blah 
            { 
                [MakeFluent] public string FirstName;
                [MakeFluent] public string LastName;
                [MakeFluent] public int Age;
            }
        }
        
        """;

        // When
        var output = compiler.GetGeneratedOutput<FluentApiGenerator>(
            new SourceFile("source.cs", source)
        );

        // Then
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace Test
        {
            public static partial class BlahExtensions
            {
                {{Constants.GeneratedCodeAttribute}}
                public static Blah FirstName(
                    this Blah _this,
                    global::System.String FirstName
                )
                {
                    _this.FirstName = FirstName;
                    return _this;
                }
                {{Constants.GeneratedCodeAttribute}}
                public static Blah LastName(
                    this Blah _this,
                    global::System.String LastName
                )
                {
                    _this.LastName = LastName;
                    return _this;
                }
                {{Constants.GeneratedCodeAttribute}}
                public static Blah Age(
                    this Blah _this,
                    global::System.Int32 Age
                )
                {
                    _this.Age = Age;
                    return _this;
                }
            }
        }
        
        """;
        var file = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(file);
        Assert.Equal(Constants.FluentApiGeneratorGeneratedContentPathPrefix + "Test.Blah.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }

    [Fact]
    public void Can_handle_multiple_members_in_multiple_files()
    {
        // Given
        string source1 = """
        namespace Test
        {
            public partial class Blah 
            { 
                public string FirstName;
                public string LastName;
            }
        }
        
        """;
        string source2 = """
        namespace Test;
    
        [Flaeng.MakeFluent]
        public partial class Blah 
        { 
            public int Age;
        }
        """;

        // When
        var output = compiler.GetGeneratedOutput<FluentApiGenerator>(
            new SourceFile("source1.cs", source1),
            new SourceFile("source2.cs", source2)
        );

        // Then
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace Test
        {
            public static partial class BlahExtensions
            {
                {{Constants.GeneratedCodeAttribute}}
                public static Blah FirstName(
                    this Blah _this,
                    global::System.String FirstName
                )
                {
                    _this.FirstName = FirstName;
                    return _this;
                }
                {{Constants.GeneratedCodeAttribute}}
                public static Blah LastName(
                    this Blah _this,
                    global::System.String LastName
                )
                {
                    _this.LastName = LastName;
                    return _this;
                }
                {{Constants.GeneratedCodeAttribute}}
                public static Blah Age(
                    this Blah _this,
                    global::System.Int32 Age
                )
                {
                    _this.Age = Age;
                    return _this;
                }
            }
        }
        
        """;
        var file = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(file);
        Assert.Equal(Constants.FluentApiGeneratorGeneratedContentPathPrefix + "Test.Blah.g.cs", file.Filename);
        Assert.Equal(expected_output, file.Content);
        Assert.Empty(output.Diagnostic);
    }
}
