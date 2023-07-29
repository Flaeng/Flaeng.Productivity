namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

public class MembersWithoutTypeParameterTests : IClassFixture<CSharpCompiler>
{
    readonly CSharpCompiler compiler;
    public MembersWithoutTypeParameterTests(CSharpCompiler compiler)
    {
        this.compiler = compiler;
    }

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_single_method()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public void Simple() { }
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
                void Simple();
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

    [Fact(Timeout = 1000)]
    public void writes_types_with_full_qualified_name()
    {
        // Arrange
        string source = """
        using System;

        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public string string1() { return default; }
                public String string2() { return default; }
                public System.String string3() { return default; }
                public global::System.String string4() { return default; }
                
                public bool bool1() { return default; }
                public Boolean bool2() { return default; }
                public System.Boolean bool3() { return default; }
                public global::System.Boolean bool4() { return default; }
                
                public int int1() { return default; }
                public Int32 int2() { return default; }
                public System.Int32 int3() { return default; }
                public global::System.Int32 int4() { return default; }
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
                global::System.String string1();
                global::System.String string2();
                global::System.String string3();
                global::System.String string4();
                global::System.Boolean bool1();
                global::System.Boolean bool2();
                global::System.Boolean bool3();
                global::System.Boolean bool4();
                global::System.Int32 int1();
                global::System.Int32 int2();
                global::System.Int32 int3();
                global::System.Int32 int4();
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

    [Fact(Timeout = 1000)]
    public void can_make_interface_when_class_has_multiple_attributes()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            [System.ComponentModel.DefaultEvent("")]
            public partial class Dummy
            {
                public string Simple { get; set; }
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
                global::System.String Simple { get; set; }
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

    [Fact(Timeout = 1000)]
    public void can_make_interface_when_class_has_multiple_attributes_alternate()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface, System.ComponentModel.DefaultEvent("")]
            public partial class Dummy
            {
                public string Simple { get; set; }
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
                global::System.String Simple { get; set; }
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

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_single_property()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public string Simple { get; set; }
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
                global::System.String Simple { get; set; }
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

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_single_field()
    {
        // Arrange
        string source = """
        namespace TestNamespace.Providers
        {
            [Flaeng.GenerateInterface]
            public partial class Dummy
            {
                public string Simple;
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

    [Fact(Timeout = 1000)]
    public void can_make_interface_with_multiple_methods()
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
        var dummyGenerated = output.GeneratedFiles.ExcludeTriggerAttribute().SingleOrDefault();
        Assert.NotNull(dummyGenerated);
        Assert.Equal(expected_output, dummyGenerated.Content);
        Assert.Empty(output.Diagnostic);
    }

}
