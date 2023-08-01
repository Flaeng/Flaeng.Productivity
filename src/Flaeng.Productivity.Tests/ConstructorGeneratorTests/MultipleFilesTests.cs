namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class MultipleFilesTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MultipleFilesTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Skip = "Calling base constructors not implemented")]
    public void can_handle_inheritance_with_inject_field_and_no_constructor()
    {
        // Arrange
        string source1 = """
        using System; 

        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                [Flaeng.Inject] 
                global::System.Collections.Generic.List<System.String> _logger1;
            }
        }
        """;
        string source2 = """
        using System.Collections.Generic;
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum : Dummy
            {
                [Flaeng.Inject] 
                List<System.Int32> _logger2;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source1),
            new SourceFile("DumDum.cs", source2)
        );

        // Assert
        string expected_output1 = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    global::System.Collections.Generic.List<global::System.String> logger1
                )
                {
                    this._logger1 = logger1;
                }
            }
        }

        """;
        string expected_output2 = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum
            {
                {{Constants.GeneratedCodeAttribute}}
                public DumDum(
                    global::System.Collections.Generic.List<global::System.Int32> logger2,
                    global::System.Collections.Generic.List<global::System.String> logger1
                ) : base(logger1)
                {
                    this._logger2 = _logger2;
                }
            }
        }

        """;
        var generatedFiles = output.GeneratedFiles.ExcludeTriggerAttribute();
        var dummyGenerated = generatedFiles.SingleOrDefault(x => x.Filename.EndsWith("Dummy.g.cs"));
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output1, dummyGenerated.Content);

        var dumdumGenerated = generatedFiles.SingleOrDefault(x => x.Filename.EndsWith("DumDum.g.cs"));
        Assert.NotNull(dumdumGenerated);
        Assert.Equal(expected_output2, dumdumGenerated.Content);

        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void can_handle_inheritance_with_no_constructor()
    {
        // Arrange
        string source1 = """
        #pragma warning disable CS0169 // ignore unused variable (_logger1)

        namespace TestNamespace.Controllers
        {
            public class Dummy
            {
                global::System.Collections.Generic.List<System.String> _logger1;
            }
        }
        """;
        string source2 = """
        using System.Collections.Generic;
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum : Dummy
            {
                [Flaeng.Inject] 
                List<System.Int32> _logger2;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source1),
            new SourceFile("DumDum.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum
            {
                {{Constants.GeneratedCodeAttribute}}
                public DumDum(
                    List<System.Int32> logger2
                )
                {
                    this._logger2 = logger2;
                }
            }
        }

        """;
        var dumdumGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dumdumGenerated);
        Assert.Equal(expected_output, dumdumGenerated.Content);

        Assert.Empty(output.Diagnostic);
    }

    [Fact(Skip = "Calling base constructors not implemented")]
    public void can_handle_inheritance_with_constructor()
    {
        // Arrange
        string source1 = """
        using System; 
        using System.Collections.Generic; 

        namespace TestNamespace.Controllers
        {
            public class Dummy
            {
                List<String> _logger1;

                public Dummy(List<String> logger1) { this._logger1 = logger1; }
            }
        }
        """;
        string source2 = """
        using System.Collections.Generic;
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum : Dummy
            {
                [Flaeng.Inject] 
                List<System.Int32> _logger2;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source1),
            new SourceFile("DumDum.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum
            {
                {{Constants.GeneratedCodeAttribute}}
                public DumDum(
                    global::System.Collections.Generic.List<global::System.Int32> logger2,
                    global::System.Collections.Generic.List<global::System.String> logger1
                ) : base(logger1)
                {
                    this._logger2 = _logger2;
                }
            }
        }

        """;
        var dumdumGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dumdumGenerated);
        Assert.Equal(expected_output, dumdumGenerated.Content);

        Assert.Empty(output.Diagnostic);
    }

    [Fact(Skip = "Calling base constructors not implemented")]
    public void can_handle_inheritance_with_constructor_and_no_setter()
    {
        // Arrange
        string source1 = """
        using System; 
        using System.Collections.Generic; 

        namespace TestNamespace.Controllers
        {
            public class Dummy
            {
                List<String> _logger1;

                public Dummy() { }
            }
        }
        """;
        string source2 = """
        using System.Collections.Generic;
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum : Dummy
            {
                [Flaeng.Inject] 
                List<System.Int32> _logger2;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("Dummy.cs", source1),
            new SourceFile("DumDum.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        namespace TestNamespace.Controllers
        {
            public partial class DumDum
            {
                {{Constants.GeneratedCodeAttribute}}
                public DumDum(
                    global::System.Collections.Generic.List<global::System.Int32> logger2
                ) : base(logger1)
                {
                    this._logger2 = _logger2;
                }
            }
        }

        """;
        var dumdumGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dumdumGenerated);
        Assert.Equal(expected_output, dumdumGenerated.Content);

        Assert.Empty(output.Diagnostic);
    }

    [Fact(Timeout = 1000)]
    public void Can_handle_partial_classes_in_multiple_files()
    {
        // Arrange
        string source1 = """
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Flaeng.Inject] IDictionary<string, object> _logger1;
            }
        }
        """;
        string source2 = """
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                [Flaeng.Inject] IDictionary<string, object> _logger2;
            }
        }
        """;

        // Act
        var output = compiler.GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy1.cs", source1),
            new SourceFile("dummy2.cs", source2)
        );

        // Assert
        string expected_output = $$"""
        {{Constants.GeneratedContentPrefix}}
        
        using System.Collections.Generic;

        namespace TestNamespace
        {
            public partial class Dummy
            {
                {{Constants.GeneratedCodeAttribute}}
                public Dummy(
                    IDictionary<string, object> logger1,
                    IDictionary<string, object> logger2
                )
                {
                    this._logger1 = logger1;
                    this._logger2 = logger2;
                }
            }
        }

        """;
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);

        Assert.Empty(output.Diagnostic);
    }

}
