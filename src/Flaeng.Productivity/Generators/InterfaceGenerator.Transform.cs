namespace Flaeng.Productivity.Generators;

public sealed partial class InterfaceGenerator
{
    internal record struct Data(
        ImmutableArray<Diagnostic> Diagnostics,
        string? Namespace,
        ImmutableArray<ClassDefinition> ParentClasses,
        ClassDefinition ClassDefinition,
        ImmutableArray<IMemberDefinition> Members,
        string? InterfaceName,
        Visibility Visibility
    );

    internal static Data Transform(GeneratorSyntaxContext context, CancellationToken ct)
    {
        var cds = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        var symbol = context.SemanticModel.GetDeclaredSymbol(cds, ct);
        if (symbol is null)
            return default;

        var syntaxes = symbol.DeclaringSyntaxReferences.Length == 1
            ? new[] { cds }.ToImmutableArray()
            : GetAllDeclarations(symbol);

        // Make sure we only generate one new source file for partial classes in multiple files
        var triggerSyntax = syntaxes.Where(HasTriggerAttribute).First();
        if (triggerSyntax != context.Node)
            return default;

        GetAttributeParameters(triggerSyntax, out string? interfaceName, out Visibility visibility);

        List<string> usings = GetUsings(syntaxes);
        GetClassModifiers(context, out bool isPartial, out bool isStatic);

        var classDef = new SyntaxSerializer().Deserialize(cds);

        if (HasError(classDef, out var diagnostics) && diagnostics is not null)
            return DataWithDiagnostic(context, symbol, diagnostics);

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();

        var baseTypes = GetBaseTypeRecursively(symbol);
        var members = baseTypes.SelectMany(x => x.GetMembers())
            .Where(x =>
                (x is IFieldSymbol && x.IsImplicitlyDeclared == false) // IsImplicitlyDeclared == Property's backingfield
                || x is IPropertySymbol
                || (
                    x is IMethodSymbol method
                    && method.IsOverride == false
                    && METHOD_KINDS_TO_IGNORE.Contains(method.MethodKind) == false
                )
            )
            .Select(x => MemberDefinitions.Parse(x, ct))
            .OfType<IMemberDefinition>()
            .ToImmutableArray();

        List<ClassDefinition> parentClasses = GetContainingTypeRecursively(symbol, ct);

        var data = new Data(
            Diagnostics: new ImmutableArray<Diagnostic>(),
            Namespace: namespaceName,
            ParentClasses: parentClasses.ToImmutableArray(),
            ClassDefinition: ClassDefinition.Parse(symbol, ct),
            Members: members.ToImmutableArray(),
            interfaceName,
            visibility
        );
        return data;
    }

    private static bool HasError(ClassDefinition classDef, out DiagnosticDescriptor? diagnostics)
    {
        if (classDef.IsStatic)
        {
            diagnostics = Rules.InterfaceGenerator_ClassIsStatic;
            return true;
        }

        if (classDef.IsPartial == false)
        {
            diagnostics = Rules.InterfaceGenerator_ClassIsNotPartial;
            return true;
        }

        diagnostics = default;
        return false;
    }

    private static void GetAttributeParameters(
        ClassDeclarationSyntax triggerSyntax,
        out string? interfaceName,
        out Visibility visibility
    )
    {
        Dictionary<string, string> dictionary = GetAttributeParameters(triggerSyntax, "GenerateInterface");
        if (dictionary.TryGetValue("Name", out interfaceName))
            interfaceName = interfaceName.Substring(1, interfaceName.Length - 2); // Remove quotes

        visibility = Visibility.Default;
        if (dictionary.TryGetValue("Visibility", out var visibilityString))
        {
            visibility = visibilityString.Split('.').Last() switch
            {
                "Public" => Visibility.Public,
                "Internal" => Visibility.Internal,
                _ => Visibility.Default
            };
        }
    }

    private static Data DataWithDiagnostic(
        GeneratorSyntaxContext context,
        INamedTypeSymbol symbol,
        DiagnosticDescriptor diagnosticDescriptor
    )
    {
        Data data = new();
        data.Diagnostics = new[]
        {
            Diagnostic.Create(
                descriptor: diagnosticDescriptor,
                location: context.Node.GetLocation(),
                messageArgs: new [] { symbol.Name }
            )
        }.ToImmutableArray();
        return data;
    }

    static readonly MethodKind[] METHOD_KINDS_TO_IGNORE = new[]
    {
        MethodKind.PropertyGet,
        MethodKind.PropertySet,
        MethodKind.Constructor
    };

    private static bool HasTriggerAttribute(ClassDeclarationSyntax syntax)
    {
        return HasAttribute(syntax, "GenerateInterface");
    }

}
