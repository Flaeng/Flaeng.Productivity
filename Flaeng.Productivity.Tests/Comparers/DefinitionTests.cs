using System.Reflection;

using Flaeng.Productivity.Comparers;

namespace Flaeng.Productivity.Tests.Comparers;

public class EqualityComparerTests
{
    [Fact]
    public void ClassDefinition()
        => TestComparer(ClassDefinitionEqualityComparer.Instance);

    [Fact]
    public void ConstructorData()
        => TestComparer(ConstructorDataEqualityComparer.Instance);

    [Fact]
    public void FieldDefinition()
        => TestComparer(FieldDefinitionEqualityComparer.Instance);

    [Fact]
    public void InterfaceData()
        => TestComparer(InterfaceDataEqualityComparer.Instance);

    [Fact]
    public void MethodDefinition()
        => TestComparer(MethodDefinitionEqualityComparer.Instance);

    [Fact]
    public void MethodParameterDefinition()
        => TestComparer(MethodParameterDefinitionEqualityComparer.Instance);

    [Fact]
    public void PropertyDefinition()
        => TestComparer(PropertyDefinitionEqualityComparer.Instance);

    private void TestComparer<T>(IEqualityComparer<T> comparer)
        where T : struct
    {
        var members = typeof(T).GetProperties();
        var ctor = typeof(T).GetConstructors()
            .Where(x => x.GetParameters().Any())
            .Single();

        foreach (var mem in members)
        {
            var memberType = mem.PropertyType;
            object?[] newValue = GetValuesForType(memberType);

            for (int x = 0; x < newValue.Length; x++)
            {
                T item1 = CreateT<T>(ctor, mem, newValue[x]);

                for (int y = 0; y < newValue.Length; y++)
                {
                    T item2 = CreateT<T>(ctor, mem, newValue[y]);

                    var result = comparer.Equals(item1, item2);
                    var hash1 = comparer.GetHashCode(item1);
                    var hash2 = comparer.GetHashCode(item2);

                    if (x == y)
                    {
                        Assert.True(result);
                        Assert.Equal(hash1, hash2);
                    }
                    else
                    {
                        Assert.False(result, $"EqualityComparer {comparer.GetType().Name} does not handle property named {mem.Name}");
                        Assert.NotEqual(hash1, hash2);
                    }
                }
            }
        }
    }

    private static object?[] GetValuesForType(Type memberType)
    {
        if (memberType is null)
            throw new NullReferenceException();

        if (memberType == typeof(string))
        {
            return new string?[] { null, "", "Lorem Ipsum" };
        }
        else if (memberType == typeof(bool))
        {
            return new bool[] { true, false }.Cast<object?>().ToArray();
        }
        else if (memberType == typeof(int))
        {
            return new int?[] { -100, 0, null, 100 }.Cast<object?>().ToArray();
        }
        else if (memberType.IsEnum)
        {
            return Enum.GetValues(memberType).Cast<object?>().ToArray();
        }
        else if (memberType.Name == "ImmutableArray`1")
        {
            return new object?[0];
            // var itemType = memberType.GenericTypeArguments[0];
            // var itemCtor = itemType.GetConstructors().First();
            // return new[]
            // {
            //     new object[0],
            //     new object[]
            //     {
            //         itemCtor.Invoke(itemCtor
            //             .GetParameters()
            //             .Select(x => (object?)null)
            //             .ToArray()
            //         )
            //     }
            // };

            // var arr = Array.CreateInstance(memberType,);
        }
        else if (memberType.UnderlyingSystemType != null)
        {
            return new object?[0];
            // return GetValuesForType(memberType.UnderlyingSystemType);
        }
        else
        {
            throw new Exception($"Type {memberType.Name} not supported");
        }
    }

    private static T CreateT<T>(
        ConstructorInfo ctor,
        PropertyInfo member,
        object? memberValue
    ) where T : struct
    {
        var ctorParams = ctor
            .GetParameters()
            .ToList();

        var indexOfProperty = ctorParams
            .FindIndex(x => x.Name!.Equals(member.Name, StringComparison.InvariantCultureIgnoreCase));

        if (indexOfProperty < 0)
            throw new IndexOutOfRangeException();

        var parameters = Enumerable.Range(0, ctorParams.Count)
            .Select((x, i) => i == indexOfProperty ? memberValue : default)
            .ToArray();

        var item1 = (T)ctor.Invoke(parameters);
        return item1;
    }

    private T Clone<T>(T item) where T : struct
    {
        var result = new T();
        foreach (var field in typeof(T).GetFields())
        {
            var value = field.GetValue(item);
            field.SetValue(result, value);
        }
        return result;
    }
}
