partial class Build
{
    readonly AbsolutePath StrykerOutput = RootDirectory / "StrykerOutput" / "**" / "mutation-report.html";

    Target Stryker => _ => _
        .DependsOn(Compile)
        .Produces(StrykerOutput)
        .Executes(() =>
        {
            DotNetTasks.DotNetToolInstall(opts => opts
                .SetPackageName("dotnet-stryker")
                .SetGlobal(true)
                .SetProcessExitHandler(process => { })
                );
            DotNetTasks.DotNet("stryker -l Advanced", "src");
        });

}
