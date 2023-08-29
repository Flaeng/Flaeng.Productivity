namespace Flaeng.Productivity.EitherOfT1OrT2OrT3Tests;

public class SimpleEitherTests
{
    class Child { public string Name = ""; public int? Age; }
    record Parent(string Name);
    struct GrandParent { public string Name; public bool IsRetired; }

    [Fact]
    public void Can_use_class_record_and_struct_as_T()
    {
        // Given

        // When
        var person1 = new Either<Child, Parent, GrandParent>(new Child { Name = "John", Age = 14 });
        var person2 = new Either<Parent, GrandParent, Child>(new Child { Name = "Jack", Age = 12 });
        var person3 = new Either<GrandParent, Child, Parent>(new Child { Name = "Jane", Age = 10 });

        // Then
        Assert.NotNull(person1);
        Assert.NotNull(person2);
        Assert.NotNull(person3);
    }

    [Fact]
    public void Can_use_as_method_when_T1()
    {
        // Given
        var person = new Either<Child, Parent, GrandParent>(new Child());

        // When
        var barn = person.AsT1();
        var foraelder = person.AsT2();
        var grandparent = person.AsT3();

        // Then
        Assert.NotNull(barn);
        Assert.Null(foraelder);
        Assert.Equal(default, grandparent);
    }

    [Fact]
    public void Can_use_as_method_when_T2()
    {
        // Given
        var person = new Either<Child, Parent, GrandParent>(new Parent("John"));

        // When
        var barn = person.AsT1();
        var foraelder = person.AsT2();
        var grandparent = person.AsT3();

        // Then
        Assert.Null(barn);
        Assert.NotNull(foraelder);
        Assert.Equal("John", foraelder.Name);
        Assert.Equal(default, grandparent);
    }

    [Fact]
    public void Can_use_as_method_when_T3()
    {
        // Given
        var person = new Either<Child, Parent, GrandParent>(new GrandParent { Name = "Jane", IsRetired = true });

        // When
        var child = person.AsT1();
        var parent = person.AsT2();
        var grandparent = person.AsT3();

        // Then
        Assert.Null(child);
        Assert.Null(parent);
        //Assert.NotNull(grandparent);
        Assert.Equal("Jane", grandparent.Name);
        Assert.True(grandparent.IsRetired);
    }

    [Fact]
    public void Can_use_is_method_when_T1()
    {
        // Given
        var person = new Either<Child, Parent, GrandParent>(new Child());

        // When
        var isBarn = person.Is(out Child? barn);
        var isForaelder = person.Is(out Parent? foraelder);
        var isGrandParent = person.Is(out GrandParent grandParent);

        // Then
        Assert.True(isBarn);
        Assert.NotNull(barn);
        Assert.False(isForaelder);
        Assert.Null(foraelder);
        Assert.False(isGrandParent);
        Assert.Equal(default, grandParent);
    }

    [Fact]
    public void Can_use_is_method_when_T2()
    {
        // Given
        var person = new Either<Child, Parent, GrandParent>(new Parent(""));

        // When
        var isBarn = person.Is(out Child? barn);
        var isForaelder = person.Is(out Parent? foraelder);
        var isGrandParent = person.Is(out GrandParent grandParent);

        // Then
        Assert.False(isBarn);
        Assert.Null(barn);
        Assert.True(isForaelder);
        Assert.NotNull(foraelder);
        Assert.False(isGrandParent);
        Assert.Equal(default, grandParent);
    }

    [Fact]
    public void Can_use_is_method_when_T3()
    {
        // Given
        var person = new Either<Child, Parent, GrandParent>(new GrandParent { Name = "Jane", IsRetired = true });

        // When
        var isBarn = person.Is(out Child? barn);
        var isForaelder = person.Is(out Parent? foraelder);
        var isGrandParent = person.Is(out GrandParent grandParent);

        // Then
        Assert.False(isBarn);
        Assert.Null(barn);
        Assert.False(isForaelder);
        Assert.Null(foraelder);
        Assert.True(isGrandParent);
        //Assert.NotNull(grandParent);
        Assert.Equal("Jane", grandParent.Name);
        Assert.True(grandParent.IsRetired);
    }

    [Fact]
    public void Can_get_value_when_T1()
    {
        // Given
        var child = new Child();

        // When
        var person = new Either<Child, Parent>(child);

        // Then
        Assert.IsType<Child>(person.Value);
        Assert.Equal(child, person.Value);
    }

    [Fact]
    public void Can_get_value_when_T2()
    {
        // Given
        var parent = new Parent("");

        // When
        var person = new Either<Child, Parent>(parent);

        // Then
        Assert.IsType<Parent>(person.Value);
        Assert.Equal(parent, person.Value);
    }

    [Fact]
    public void Can_get_value_when_T3()
    {
        // Given
        var parent = new GrandParent { Name = "Jane", IsRetired = true };

        // When
        var person = new Either<Child, Parent, GrandParent>(parent);

        // Then
        Assert.IsType<GrandParent>(person.Value);
        Assert.Equal(parent, person.Value);
    }

}
