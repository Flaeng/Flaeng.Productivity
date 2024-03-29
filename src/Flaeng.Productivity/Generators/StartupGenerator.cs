namespace Flaeng.Productivity.Generators;

[Generator(LanguageNames.CSharp)]
public sealed partial class StartupGenerator : GeneratorBase
{
    private const string ServiceCollectionQualifiedName = "global::Microsoft.Extensions.DependencyInjection.IServiceCollection";

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Initialize(context, GenerateTriggerAttribute, Predicate, Transform, StartupDataEqualityComparer.Instance, Execute);
    }

    public static bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        if (node is not ClassDeclarationSyntax { AttributeLists.Count: > 0 } cds)
            return false;

        var result = HasTriggerAttribute(cds);
        return result;
    }

    private static bool HasTriggerAttribute(ClassDeclarationSyntax syntax)
    {
        return HasAttribute(syntax, "RegisterService");
    }

    private static void GenerateTriggerAttribute(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource(
            "StartupExtensionAttribute.g.cs",
            SourceText.From($$"""
            // <auto-generated/>

            #nullable enable

            namespace Flaeng
            {
                [global::System.AttributeUsage(global::System.AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
                public sealed class StartupExtensionAttribute : global::System.Attribute
                {
                    public string? MethodName = null;
                }
            }
            """, Encoding.UTF8)
        );
        context.AddSource(
            "RegisterAttribute.g.cs",
            SourceText.From($$"""
            // <auto-generated/>

            #nullable enable

            namespace Flaeng
            {
                {{Constants.GeneratedCodeAttribute}}
                internal enum ServiceType { Transient, Scoped, Singleton }

                [global::System.AttributeUsageAttribute(
                    global::System.AttributeTargets.Class,
                    AllowMultiple = false, 
                    Inherited = false)]
                {{Constants.GeneratedCodeAttribute}}
                internal sealed class RegisterServiceAttribute : global::System.Attribute
                {
                    public ServiceType ServiceType = ServiceType.Scoped;
                }
            }
            """, Encoding.UTF8)
        );
    }
}
