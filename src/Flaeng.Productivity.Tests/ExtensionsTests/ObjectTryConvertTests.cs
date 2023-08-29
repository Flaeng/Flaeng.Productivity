namespace Flaeng.Productivity.ExtensionsTests;

#pragma warning disable CS8625

public class ObjectTryConvertTests
{
    [Fact]
    public void Can_convert_string_to_int()
    {
        // Given
        string number = "159";

        // When
        var result1 = number.TryConvert<int>();
        var result2 = number.TryConvert(typeof(int));

        // Then
        Assert.True(result1.DidSucceed);
        Assert.Equal(159, result1.Result);
        Assert.True(result2.DidSucceed);
        Assert.Equal(159, result2.Result);
    }

    [Fact]
    public void Can_convert_int_to_string()
    {
        // Given
        int number = 159;

        // When
        var result1 = number.TryConvert<string>();
        var result2 = number.TryConvert(typeof(string));

        // Then
        Assert.True(result1.DidSucceed);
        Assert.Equal("159", result1.Result);
        Assert.True(result2.DidSucceed);
        Assert.Equal("159", result2.Result);
    }

    [Fact]
    public void Cannot_convert_invalid_string_to_int()
    {
        // Given
        string number = "blah";

        // When
        var result1 = number.TryConvert<int>();
        var result2 = number.TryConvert(typeof(int));

        // Then
        Assert.False(result1.DidSucceed);
        Assert.Equal(default, result1.Result);
        Assert.False(result2.DidSucceed);
        Assert.Equal(default, result2.Result);
    }

    [Fact]
    public void Cannot_convert_when_given_null_as_type()
    {
        // Given
        string number = "456";

        // When
        var result = number.TryConvert(null);

        // Then
        Assert.False(result.DidSucceed);
        Assert.Equal(default, result.Result);
    }

    [Fact]
    public void Can_convert_string_to_int_with_out()
    {
        // Given
        string number = "159";

        // When
        var bResult1 = number.TryConvert<int>(out var iResult1);
        var bResult2 = number.TryConvert(typeof(int), out var iResult2);

        // Then
        Assert.True(bResult1);
        Assert.Equal(159, iResult1);
        Assert.True(bResult2);
        Assert.Equal(159, iResult2);
    }

    [Fact]
    public void Can_convert_int_to_string_with_out()
    {
        // Given
        int number = 159;

        // When
        var bResult1 = number.TryConvert<string>(out var sResult1);
        var bResult2 = number.TryConvert(typeof(string), out var sResult2);

        // Then
        Assert.True(bResult1);
        Assert.Equal("159", sResult1);
        Assert.True(bResult2);
        Assert.Equal("159", sResult2);
    }

    [Fact]
    public void Cannot_convert_invalid_string_to_int_with_out()
    {
        // Given
        string number = "blah";

        // When
        var bResult1 = number.TryConvert<int>(out var iResult1);
        var bResult2 = number.TryConvert(typeof(int), out var iResult2);

        // Then
        Assert.False(bResult1);
        Assert.Equal(default, iResult1);
        Assert.False(bResult2);
        Assert.Equal(default, iResult2);
    }

    [Fact]
    public void Cannot_convert_when_given_null_as_type_with_out()
    {
        // Given
        string number = "456";

        // When
        var boolResult = number.TryConvert(null, out var result);

        // Then
        Assert.False(boolResult);
        Assert.Equal(default, result);
    }

}
