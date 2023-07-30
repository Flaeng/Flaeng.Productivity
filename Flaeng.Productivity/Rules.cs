namespace Flaeng.Productivity;

#pragma warning disable RS2008 // TODO

public static class Rules
{
    public static DiagnosticDescriptor ConstructorGenerator_ClassIsNotPartial = new DiagnosticDescriptor(
        id: "FJ1001",
        title: "Classes with Inject attribute on members should be partial",
        messageFormat: "Class {0} should be partial for the source generator to extend it",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    public static DiagnosticDescriptor ConstructorGenerator_ClassIsStatic = new DiagnosticDescriptor(
        id: "FJ1002",
        title: "Classes with Inject attribute on members cannot be static",
        messageFormat: "Class {0} cannot be static for the source generator to extend it",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    public static DiagnosticDescriptor ConstructorGenerator_MemberIsStatic = new DiagnosticDescriptor(
        id: "FJ1003",
        title: "Members with Inject attribute cannot be static",
        messageFormat: "Member {0} cannot be static for the source generator to set it in the constructor",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor InterfaceGenerator_ClassIsNotPartial = new DiagnosticDescriptor(
        id: "FJ2001",
        title: "Classes with GenerateInterface attribute should be partial",
        messageFormat: "Class {0} should be partial for the source generator to extend it",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    public static DiagnosticDescriptor InterfaceGenerator_ClassIsStatic = new DiagnosticDescriptor(
        id: "FJ2002",
        title: "Classes with GenerateInterface attribute cannot be static",
        messageFormat: "Class {0} cannot be static for the source generator to extend it",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
