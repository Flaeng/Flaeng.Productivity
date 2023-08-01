// Todo
// Should make methods for each property and field
// that work like fluent apis

// Input
// [MakeFluent]
// public class Test
// {
//     public string Name = "";
//     public int MyProperty { get; set; }
// }

// Input (alternative)
// public class Test
// {
//     [MakeFluent]
//     public string Name = "";
//     [MakeFluent]
//     public int MyProperty { get; set; }
// }

// Output
// public static class TestExtensions
// {
//     public static Test MyProperty(this Test test, int num)
//     {
//         test.MyProperty = num;
//         return test;
//     }
//     public static Test Name(this Test test, string str)
//     {
//         test.Name = str;
//         return test;
//     }
// }

namespace Flaeng.Productivity.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class FluentApiGenerator : GeneratorBase
{
    internal record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string? Namespace,
        ImmutableArray<ClassDefinition> ParentClasses,
        ClassDefinition ClassDefinition,
        ImmutableArray<IMemberDefinition> Members
    );

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Initialize(context, GenerateTriggerAttribute, Predicate, Transform, FluentApiDataEqualityComparer.Instance, Execute);
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } cds && HasTriggerAttribute(cds)
            || node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 } pds && HasTriggerAttribute(pds)
            || node is FieldDeclarationSyntax { AttributeLists.Count: > 0 } fds && HasTriggerAttribute(fds);
    }

    private static bool HasTriggerAttribute(MemberDeclarationSyntax syntax) => HasAttribute(syntax, "MakeFluent");

    private static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        if (SyntaxNodeShouldRunTransform(context, ct, out var classSymbol) == false)
            return new Data();

        classSymbol = Unsafe.As<INamedTypeSymbol>(classSymbol);

        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace ? null : classSymbol.ContainingNamespace.ToDisplayString();

        var classDefinition = ClassDefinition.Parse(classSymbol, ct);
        var members = classSymbol
            .GetMembers()
            .Where(x =>
                (x is IFieldSymbol fs && fs.IsImplicitlyDeclared == false)
                || (x is IPropertySymbol)
            )
            .Select(x => MemberDefinitions.Parse(x, ct))
            .OfType<IMemberDefinition>()
            .ToImmutableArray();

        var parentClasses = GetContainingTypeRecursively(classSymbol, ct);

        return new Data(ImmutableArray<Diagnostic>.Empty, namespaceName, parentClasses.ToImmutableArray(), classDefinition, members);
    }

    private static bool SyntaxNodeShouldRunTransform(GeneratorSyntaxContext context, CancellationToken ct, out INamedTypeSymbol? classSymbol)
    {
        // If current node is class node
        if (context.Node is ClassDeclarationSyntax cls)
        {
            classSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node, ct) as INamedTypeSymbol;
            return true;
        }

        classSymbol = null;
        var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node, ct);
        if (fieldSymbol is null)
            return false;

        classSymbol = fieldSymbol.ContainingType;
        // Verify that current node is not a field or property on a class with trigger attribute
        // If so return false, execute has been called with class when it has trigger attribute
        if (classSymbol.GetAttributes().Any(IsTriggerAttribute))
            return false;

        // Verify that current node is the first field or property with the attribute in this class
        var allMembers = classSymbol.GetMembers();
        var membersWithTriggerAttribute = allMembers.Where(member => member.GetAttributes().Any(IsTriggerAttribute)).ToArray();
        return SymbolEqualityComparer.Default.Equals(membersWithTriggerAttribute.First(), fieldSymbol);
    }

    private static bool IsTriggerAttribute(AttributeData data)
    {
        return data.AttributeClass is not null
            && data.AttributeClass.ContainingNamespace.Name == "Flaeng"
            && data.AttributeClass.Name == "MakeFluent";
    }

    private static void Execute(SourceProductionContext context, Data source)
    {
        TryWriteDiagnostics(context, source.Diagnostics);

        var clsName = source.ClassDefinition.Name;
        if (clsName is null)
            return;

        CSharpBuilder builder = new(DefaultCSharpOptions);
        List<string> filenameParts = new();


        if (TryWriteNamespace(source.Namespace, builder))
            filenameParts.Add(source.Namespace!);

        // Write class and wrapper classes
        WriteWrapperClasses(source.ParentClasses, builder, filenameParts);

        builder.WriteClass(new ClassDefinition(
            visibility: source.ClassDefinition.Visibility,
            isStatic: true,
            isPartial: true,
            name: $"{source.ClassDefinition.Name}Extensions",
            typeArguments: ImmutableArray<string>.Empty,
            interfaces: ImmutableArray<InterfaceDefinition>.Empty,
            constructors: ImmutableArray<MethodDefinition>.Empty
        ));
        builder.StartScope();

        foreach (var member in source.Members)
        {
            if (member.Name is null)
                continue;

            if (member is PropertyDefinition pd && pd.SetterVisibility == null)
                continue;

            builder.WriteLine(Constants.GeneratedCodeAttribute);
            builder.WriteMethod(new MethodDefinition(
                visibility: Visibility.Public,
                isStatic: true,
                type: clsName,
                name: member.Name,
                parameters: new[]
                {
                    new MethodParameterDefinition(parameterKind: "this", type: clsName, name: "_this", defaultValue: null),
                    new MethodParameterDefinition(parameterKind: null, type: member.Type, name: member.Name, defaultValue: null)
                }.ToImmutableArray()
            ));
            builder.StartScope();
            builder.WriteLine($"_this.{member.Name} = {member.Name};");
            builder.WriteLine("return _this;");
            builder.EndScope();
        }

        string filename = filenameParts.Select(x => $"{x}.").Join() + $"{source.ClassDefinition.Name}.g.cs";
        var content = builder.Build();
        context.AddSource(filename, content);
    }

    private static void GenerateTriggerAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("MakeFluentAttribute.g.cs", SourceText.From($$"""
        // <auto-generated/>

        #nullable enable

        namespace Flaeng
        {
            [global::System.AttributeUsageAttribute(
                global::System.AttributeTargets.Class | global::System.AttributeTargets.Field | global::System.AttributeTargets.Property,
                AllowMultiple = false,
                Inherited = false)]
            {{Constants.GeneratedCodeAttribute}}
            internal sealed class MakeFluentAttribute : global::System.Attribute
            { }
        }
        """, Encoding.UTF8));
    }
}
