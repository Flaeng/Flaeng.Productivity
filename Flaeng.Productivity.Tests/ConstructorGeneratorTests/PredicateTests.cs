namespace Flaeng.Productivity.Tests.ConstructorGeneratorTests;

public class PredicateTests
{

    [Fact(Timeout = 1000)]
    public void PredicateTest1()
    {
        // Given
        var source = """
        public class Test
        {
        }
        """;

        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => ConstructorGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.False(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest2()
    {
        // Given
        var source = """
        public class Test
        {
            public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => ConstructorGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.False(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest3()
    {
        // Given
        var source = """
        public class Test
        {
            [Flaeng.Inject] public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => ConstructorGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest4()
    {
        // Given
        var source = """
        public class Test
        {
            [Flaeng.InjectAttribute] public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => ConstructorGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest5()
    {
        // Given
        var source = """
        using Flaeng;
        public class Test
        {
            [Inject] public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => ConstructorGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest6()
    {
        // Given
        var source = """
        using Flaeng;
        public class Test
        {
            [InjectAttribute] public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => ConstructorGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

}
