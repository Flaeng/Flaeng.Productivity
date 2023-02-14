using Flaeng.Productivity.DependencyInjection;

namespace Flaeng.Productivity.Tests.DependencyInjection;

public class InterfaceGeneratorTests : TestBase
{

    [Fact]
    public void Does_generate_attribute()
    {
        // Arrange

        // Act
        var output = GetGeneratedOutput<InterfaceGenerator>();

        // Assert
        Assert.Empty(output.Diagnostic);

        var source = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("GenerateInterfaceAttribute.g.cs"));
        Assert.Equal(@"// <auto-generated/>
#nullable enable

namespace Flaeng.Productivity.DependencyInjection
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Flaeng.Productivity"", ""1.0.0.0"")]
    [global::System.AttributeUsageAttribute(
        global::System.AttributeTargets.Class, 
        AllowMultiple = false,
        Inherited = false)]
    internal sealed class GenerateInterfaceAttribute : global::System.Attribute
    {
        public GenerateInterfaceAttribute()
        { }
    }
}", source?.Content);
    }

    [Fact]
    public void wont_generate_source_when_no_attribute_is_provided()
    {
        // Arrange
        string source = @"namespace TestNamespace.Providers
{
    public partial class Dummy
    {
        public void Simple() { }
    }
}";

        // Act
        var output = GetGeneratedOutput<ConstructorGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        Assert.Empty(output.Diagnostic);
        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.Null(dummyGenerated);
    }

    [Fact]
    public void can_make_interface_with_no_methods()
    {
        // Arrange
        string source = @"using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace.Providers
{
    [GenerateInterface]
    public partial class Dummy
    { }
}";

        // Act
        var output = GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = @"// <auto-generated/>

#nullable enable

namespace TestNamespace.Providers
{
    public partial interface IDummy
    {
    }
    public partial class Dummy : IDummy
    {
    }
}
";

        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_make_interface_with_single_method()
    {
        // Arrange
        string source = @"using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace.Providers
{
    [GenerateInterface]
    public partial class Dummy
    {
        public void Simple() { }
    }
}";

        // Act
        var output = GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = @"// <auto-generated/>

#nullable enable

namespace TestNamespace.Providers
{
    public partial interface IDummy
    {
        void Simple();
    }
    public partial class Dummy : IDummy
    {
    }
}
";

        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_make_interface_with_multiple_methods()
    {
        // Arrange
        string source = @"using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace.Providers
{
    [GenerateInterface]
    public partial class Dummy
    {
        public void Simple() { }
        public bool Simple(int number) { return true; }
    }
}";

        // Act
        var output = GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = @"// <auto-generated/>

#nullable enable

namespace TestNamespace.Providers
{
    public partial interface IDummy
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
";
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_make_interface_with_a_method_with_an_out_parameter()
    {
        // Arrange
        string source = @"using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace.Providers
{
    [GenerateInterface]
    public partial class Dummy
    {
        public bool Simple(ref int number) { number = 0; return true; }
    }
}";

        // Act
        var output = GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = @"// <auto-generated/>

#nullable enable

namespace TestNamespace.Providers
{
    public partial interface IDummy
    {
        global::System.Boolean Simple(
            ref global::System.Int32 number
            );
    }
    public partial class Dummy : IDummy
    {
    }
}
";
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

    [Fact]
    public void can_make_interface_with_a_method_with_a_normal_and_an_out_parameter()
    {
        // Arrange
        string source = @"using Flaeng.Productivity.DependencyInjection;

namespace TestNamespace.Providers
{
    [GenerateInterface]
    public partial class Dummy
    {
        public bool Simple(string text, out int number) { number = 0; return true; }
    }
}";

        // Act
        var output = GetGeneratedOutput<InterfaceGenerator>(
            new SourceFile("dummy.cs", source)
        );

        // Assert
        string expected_output = @"// <auto-generated/>

#nullable enable

namespace TestNamespace.Providers
{
    public partial interface IDummy
    {
        global::System.Boolean Simple(
            global::System.String text,
            out global::System.Int32 number
            );
    }
    public partial class Dummy : IDummy
    {
    }
}
";
        Assert.Empty(output.Diagnostic);

        var dummyGenerated = output.GeneratedFiles
            .SingleOrDefault(x => x.Filename.EndsWith("TestNamespace.Providers.IDummy.g.cs"));
        Assert.Equal(expected_output, dummyGenerated?.Content);
    }

}