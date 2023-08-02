namespace Flaeng.Productivity.Generators;

public sealed partial class FluentApiGenerator
{
    internal record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string? Namespace,
        ImmutableArray<ClassDefinition> ParentClasses,
        ClassDefinition ClassDefinition,
        ImmutableArray<IMemberDefinition> Members
    );

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

}
