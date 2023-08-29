namespace Flaeng.Productivity.EitherOfT1OrT2Tests;

public class SimpleEitherTests
{
    class Child { public string Name = ""; public int? Age; }
    record Parent(string Name);

    [Fact]
    public void Can_use_class_record_and_struct_as_T()
    {
        // Given

        // When
        var person1 = new Either<Child, Parent>(new Child { Name = "Jack", Age = 10 });
        var person2 = new Either<Parent, Child>(new Child { Name = "John", Age = 13 });

        // Then
        Assert.NotNull(person1);
        Assert.NotNull(person2);
    }

    [Fact]
    public void Can_use_as_method_when_T1()
    {
        // Given
        var person = new Either<Child, Parent>(new Child());

        // When
        var barn = person.AsT1();
        var foraelder = person.AsT2();

        // Then
        Assert.NotNull(barn);
        Assert.Null(foraelder);
    }

    [Fact]
    public void Can_use_as_method_when_T2()
    {
        // Given
        var person = new Either<Child, Parent>(new Parent(""));

        // When
        var barn = person.AsT1();
        var foraelder = person.AsT2();

        // Then
        Assert.Null(barn);
        Assert.NotNull(foraelder);
    }

    [Fact]
    public void Can_use_is_method_when_T1()
    {
        // Given
        var person = new Either<Child, Parent>(new Child());

        // When
        var isBarn = person.Is(out Child? barn);
        var isForaelder = person.Is(out Parent? foraelder);

        // Then
        Assert.True(isBarn);
        Assert.NotNull(barn);
        Assert.False(isForaelder);
        Assert.Null(foraelder);
    }

    [Fact]
    public void Can_use_is_method_when_T2()
    {
        // Given
        var person = new Either<Child, Parent>(new Parent(""));

        // When
        var isBarn = person.Is(out Child? barn);
        var isForaelder = person.Is(out Parent? foraelder);

        // Then
        Assert.False(isBarn);
        Assert.Null(barn);
        Assert.True(isForaelder);
        Assert.NotNull(foraelder);
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

}
