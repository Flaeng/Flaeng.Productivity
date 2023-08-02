namespace Flaeng.Productivity.Definitions;

internal interface IMemberDefinition
{
    string? Type { get; }
    bool IsStatic { get; }
    string? Name { get; }
}
