namespace Flaeng.Productivity.Tests.InterfaceGeneratorTests;

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
        var anyMatchesPredicate = root.ChildNodes().Any(node => InterfaceGenerator.Predicate(node, CancellationToken.None));

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
        var anyMatchesPredicate = root.ChildNodes().Any(node => InterfaceGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.False(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest3()
    {
        // Given
        var source = """
        [Flaeng.GenerateInterface]
        public class Test
        {
            public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => InterfaceGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest4()
    {
        // Given
        var source = """
        [Flaeng.GenerateInterfaceAttribute]
        public class Test
        {
            public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => InterfaceGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest5()
    {
        // Given
        var source = """
        using Flaeng;
        
        [GenerateInterface]
        public class Test
        {
            public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => InterfaceGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

    [Fact(Timeout = 1000)]
    public void PredicateTest6()
    {
        // Given
        var source = """
        using Flaeng;
        
        [GenerateInterfaceAttribute]
        public class Test
        {
            public int MyProperty { get; set; }
        }
        """;
        // When
        var syntax = CSharpSyntaxTree.ParseText(source);
        var root = syntax.GetRoot();
        var anyMatchesPredicate = root.ChildNodes().Any(node => InterfaceGenerator.Predicate(node, CancellationToken.None));

        // Then
        Assert.True(anyMatchesPredicate);
    }

}
