namespace Flaeng.Productivity.Tests;

public class SourceBuilderTests
{

    [Fact]
    public void Can_make_empty_source()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable
                     """, result);
    }

    [Fact]
    public void Can_change_tab_size()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.TabLength = 2;
        sourceBuilder.StartNamespace("Test");
        sourceBuilder.StartClass(TypeVisiblity.Public, "TestClass");
        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace Test
                     {
                       public class TestClass
                       {
                       }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_use_tabs()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.TabLength = 1;
        sourceBuilder.TabStyle = TabStyle.Tabs;
        sourceBuilder.StartNamespace("Test");
        sourceBuilder.StartClass(TypeVisiblity.Public, "TestClass");
        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace Test
                     {
                     
                     """ +
                     '\t' + "public class TestClass\r\n" +
                     '\t' + "{\r\n" +
                     '\t' + "}\r\n" +
                     """
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_disable_nullable()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.NullableEnable = false;
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable disable
                     """, result);
    }

    [Fact]
    public void Can_make_empty_namespace()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_class()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.StartClass(TypeVisiblity.Public, "Publicy");
        sourceBuilder.StartMethod(MemberVisiblity.Private, "string", "Parse", new string[0], @static: true);
        sourceBuilder.AddLineOfCode("return \"\";");
        sourceBuilder.EndMethod();
        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public class Publicy
                         {
                             private static string Parse()
                             {
                                 return "";
                             }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_static_class()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.StartClass(TypeVisiblity.Public, "Publicy", @static: true, interfaces: new[] { "BaseClass", "Interface1", "Interface2" });
        sourceBuilder.StartMethod(MemberVisiblity.Private, "string", "Parse", new string[0], @static: true);
        sourceBuilder.AddLineOfCode("return \"\";");
        sourceBuilder.EndMethod();
        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public static class Publicy : BaseClass, Interface1, Interface2
                         {
                             private static string Parse()
                             {
                                 return "";
                             }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_interface()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.StartInterface(TypeVisiblity.Internal, "Interconnected", partial: true, interfaces: new[] { "BaseInterface" });
        sourceBuilder.DeclareMethod("void", "Invoke", new[] { "object obj", "string str" });
        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         internal partial interface Interconnected : BaseInterface
                         {
                             void Invoke(
                                 object obj,
                                 string str
                                 );
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_struct()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.StartStruct(TypeVisiblity.Public, "DataContainer", partial: true);
        sourceBuilder.AddProperty(MemberVisiblity.Protected, "string", "Name");
        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public partial struct DataContainer
                         {
                             protected string Name { get; set; }
                         }
                     }
                     
                     """, result);
    }

    [Fact]
    public void Can_make_properties()
    {
        // Arrange
        var sourceBuilder = new SourceBuilder();

        // Act
        sourceBuilder.StartNamespace("SpacyName");
        sourceBuilder.StartClass(TypeVisiblity.Public, "Publicy");

        sourceBuilder.AddProperty(MemberVisiblity.Public, "string", "Text",
            getter: GetterSetterVisiblity.Protected,
            setter: GetterSetterVisiblity.Private,
            "\"\"");

        sourceBuilder.AddProperty(MemberVisiblity.Public, "string", "Text2",
            getter: GetterSetterVisiblity.Private,
            setter: GetterSetterVisiblity.Protected);

        sourceBuilder.EndClass();
        sourceBuilder.EndNamespace();
        var result = sourceBuilder.ToString();

        // Assert
        Assert.Equal("""
                     // <auto-generated/>

                     #nullable enable

                     namespace SpacyName
                     {
                         public class Publicy
                         {
                             public string Text { protected get; private set; } = "";
                             public string Text2 { private get; protected set; }
                         }
                     }
                     
                     """, result);
    }

}