// Todo
// Should make class that implements IEqualityComparer
// for classes and structs where we check all properties and fields

// Input
// [Flaeng.GenerateEqualityComparer]
// public class Test
// {
//     [Flaeng.EqualityComparerIgnore] public int Id = 0;
//     public string Name = "";
//     public int MyProperty { get; set; }
// }

// Output
// public class TestEqualityComparer : IEqualityComparer<Test>
// {
//     public static TestEqualityComparer Instance = new();

//     public bool Equals(Test x, Test y)
//     {
//         return x.Name.Equals(y.Name)
//             && x.MyProperty.Equals(y.MyProperty);
//     }

//     public int GetHashCode(Test obj)
//     {
//         return obj.Name.GetHashCode()
//             ^ obj.MyProperty.GetHashCode();
//     }
// }
