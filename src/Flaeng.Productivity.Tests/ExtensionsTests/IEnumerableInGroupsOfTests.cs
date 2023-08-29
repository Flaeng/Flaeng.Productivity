namespace Flaeng.Productivity.ExtensionsTests;

public class IEnumerableInGroupsOfTests
{
    record User(string Username, int Age);

    [Fact]
    public void Can_distinct_by()
    {
        // Given
        var list = new List<User>
        {
            new User("John", 20),
            new User("Jimmy", 10),
            new User("Jane", 10),
            new User("Jennie", 20),
            new User("Jay", 20),
        };

        // When
        var ages = list.InGroupsOf(2);

        // Then
        Assert.Equal(3, ages.Count());
        Assert.Equal("John", ages.ElementAt(0).ElementAt(0).Username);
        Assert.Equal("Jimmy", ages.ElementAt(0).ElementAt(1).Username);
        Assert.Equal("Jane", ages.ElementAt(1).ElementAt(0).Username);
        Assert.Equal("Jennie", ages.ElementAt(1).ElementAt(1).Username);
        Assert.Equal("Jay", ages.ElementAt(2).ElementAt(0).Username);
        Assert.Null(ages.ElementAt(2).ElementAtOrDefault(1));
    }

}
