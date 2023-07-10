partial class Build
{
    Target Housekeeping => _ => _
        .DependsOn(Format, Test, Stryker);

    Target Format => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetFormat(opts => opts
                .SetIncludeGenerated(false)
                );

            if (GitTasks.GitHasCleanWorkingCopy())
                return;

            if (IsServerBuild == false)
                return;

            GitTasks.Git("config --global user.name '@Flaeng'");
            GitTasks.Git("config --global user.email 'flaeng@users.noreply.github.com'");
            GitTasks.Git($"commit -am \"{nameof(Format)}\"");
            GitTasks.Git($"push origin HEAD:{GitRepository.Branch}");
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
