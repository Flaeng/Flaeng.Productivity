partial class Build
{
    Target Housekeeping => _ => _
        .DependsOn(Format, Test);

    Target Format => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetFormat(opts => opts
                .SetProcessWorkingDirectory("src")
                .SetVerifyNoChanges(true));
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
