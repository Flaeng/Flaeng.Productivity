using System.Reflection;

using Flaeng.Productivity.Comparers;
using Flaeng.Productivity.Definitions;

namespace Flaeng.Productivity.Tests.Comparers;

public class IMemberDefinitionEqualityComparerTests
{
    [Fact]
    public void No_IMemberDefinitions_are_equal()
    {
        // Given
        var types = TypesThatImplementIMemberDefinition();

        // When
        for (int x = 0; x < types.Length; x++)
        {
            for (int y = 0; y < types.Length; y++)
            {
                if (x == y)
                    continue;

                var type1 = types[x];
                var type2 = types[y];

                var inst1 = Activator.CreateInstance(type1) as IMemberDefinition;
                var inst2 = Activator.CreateInstance(type2) as IMemberDefinition;

                Assert.NotNull(inst1);
                Assert.NotNull(inst2);

                var equals = IMemberDefinitionEqualityComparer.Instance.Equals(inst1, inst2);
                var hash1 = IMemberDefinitionEqualityComparer.Instance.GetHashCode(inst1);
                var hash2 = IMemberDefinitionEqualityComparer.Instance.GetHashCode(inst2);

                // Then
                Assert.False(equals);
                Assert.NotEqual(hash1, hash2);
            }
        }
    }

    [Fact]
    public void IMemberDefinitions_calls_type_comparer()
    {
        // Given
        var types = TypesThatImplementIMemberDefinition();

        // When
        for (int x = 0; x < types.Length; x++)
        {
            var type = types[x];

            var inst1 = Activator.CreateInstance(type) as IMemberDefinition;
            var inst2 = Activator.CreateInstance(type) as IMemberDefinition;

            Assert.NotNull(inst1);
            Assert.NotNull(inst2);

            var equals = IMemberDefinitionEqualityComparer.Instance.Equals(inst1, inst2);
            var hash1 = IMemberDefinitionEqualityComparer.Instance.GetHashCode(inst1);
            var hash2 = IMemberDefinitionEqualityComparer.Instance.GetHashCode(inst2);

            // Then
            Assert.True(equals);
            Assert.Equal(hash1, hash2);
        }
    }

    private static Type[] TypesThatImplementIMemberDefinition()
    {
        return typeof(IMemberDefinition)
                    .Assembly
                    .GetTypes()
                    .Where(x => x != typeof(IMemberDefinition))
                    .Where(x => typeof(IMemberDefinition).IsAssignableFrom(x))
                    .ToArray();
    }
}
