partial class Build
{
    Target Housekeeping => _ => _
        .DependsOn(Format, Test, Stryker);

    Target Format => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetFormat();

            if (IsServerBuild && GitTasks.GitHasCleanWorkingCopy() == false)
                throw new Exception($"Branch needs formatting - Please run 'dotnet format' locally and push changes");
        });


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
            DotNetTasks.DotNet("stryker -l Advanced");
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Projects
                .Where(proj => proj.IsTestProject())
                .ForEach(proj =>
                    DotNetTasks.DotNetTest(opts => opts
                    .SetProjectFile(proj)));
        });

}
