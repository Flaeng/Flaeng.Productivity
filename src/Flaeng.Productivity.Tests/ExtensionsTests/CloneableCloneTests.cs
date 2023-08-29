// namespace Flaeng.Productivity.Tests.ExtensionsTests;

// public class CloneableCloneTests
// {
//     class Person : ICloneable<Person>
//     {
//         public string Name = "";
//         public Person? Parent { get; set; }
//     }

//     [Fact]
//     public void Can_clone_simple_class()
//     {
//         // Given
//         var john = new Person { Name = "John" };

//         // When
//         var clone = john.Clone();

//         // Then
//         Assert.Equal("John", clone.Name);
//     }

//     [Fact]
//     public void Wont_clone_property_if_recursive_is_false()
//     {
//         // Given
//         var john = new Person { Name = "John" };
//         var jim = new Person { Name = "Jim", Parent = john };

//         // When
//         var clone = jim.DeepClone(recursive: false);

//         // Then
//         Assert.Equal(jim.Parent, clone.Parent);
//     }

//     [Fact]
//     public void Will_clone_property_if_recursive_is_true()
//     {
//         // Given
//         var john = new Person { Name = "John" };
//         var jim = new Person { Name = "Jim", Parent = john };

//         // When
//         var clone = jim.DeepClone(recursive: false);

//         // Then
//         Assert.Equal(jim.Parent, clone.Parent);
//         Assert.Equal("John", clone.Parent?.Name);
//     }

//     [Fact]
//     public void Will_clone_in_depth_of_1()
//     {
//         // Given
//         var jack = new Person { Name = "Jack" };
//         var john = new Person { Name = "John", Parent = jack };
//         var jim = new Person { Name = "Jim", Parent = john };

//         // When
//         var clone = jim.DeepClone(depth: 1);

//         // Then
//         Assert.NotEqual(jim.Parent, clone.Parent);
//         Assert.Equal("John", clone.Parent?.Name);
//         Assert.Equal(jim.Parent.Parent, clone.Parent?.Parent);
//         Assert.Equal("Jack", clone.Parent?.Parent?.Name);
//     }

// }
