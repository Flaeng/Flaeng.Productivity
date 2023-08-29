namespace Flaeng.Productivity.ExtensionsTests;

public class ObjectEqualsAnyTests
{
    record User(string Username);
    enum Status { Pending, Starting, Running, Completed, Failed }

    [Fact]
    public void Can_handle_enum_values_when_result_should_be_true()
    {
        // Given
        var status = Status.Starting;

        // When
        var result = status.EqualsAny(Status.Starting, Status.Running);

        // Then
        Assert.True(result);
    }

    [Fact]
    public void Can_handle_enum_values_when_result_should_be_false()
    {
        // Given
        var status = Status.Starting;

        // When
        var result = status.EqualsAny(Status.Completed, Status.Failed);

        // Then
        Assert.False(result);
    }

    [Fact]
    public void Can_handle_struct_values_when_result_should_be_true()
    {
        // Given
        var number = 10;

        // When
        var result = number.EqualsAny(10, 20, 30, 40, 50);

        // Then
        Assert.True(result);
    }

    [Fact]
    public void Can_handle_struct_values_when_result_should_be_false()
    {
        // Given
        var number = 10;

        // When
        var result = number.EqualsAny(15, 25, 35, 45, 55);

        // Then
        Assert.False(result);
    }

    [Fact]
    public void Can_handle_class_values_when_result_should_be_true()
    {
        // Given
        var john = new User("John");
        var jane = new User("Jane");
        var jimmy = new User("Jimmy");

        // When
        var result = john.EqualsAny(john, jane, jimmy);

        // Then
        Assert.True(result);
    }

    [Fact]
    public void Can_handle_class_values_when_result_should_be_false()
    {
        // Given
        var john = new User("John");
        var jane = new User("Jane");
        var jimmy = new User("Jimmy");

        // When
        var result = john.EqualsAny(jane, jimmy);

        // Then
        Assert.False(result);
    }

}
