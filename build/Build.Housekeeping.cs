partial class Build
{
    Target Housekeeping => _ => _
        .DependsOn(Format, Test);

    Target Format => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetFormat(opts => opts
                .SetVerifyNoChanges(IsServerBuild)
                .SetVerbosity(IsServerBuild ? "normal" : "diagnostic")
                .CombineWith(Solution.Projects
                    .Select<Project, Configure<DotNetFormatSettings>>(proj => opts => opts
                        .SetProject(proj.Path)
                    )
                    .ToArray()
                ),
                degreeOfParallelism: Solution.Projects.Count
            );
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(opts => opts
                .SetProjectFile(Solution.Flaeng_Productivity_Tests)
            );
        });

}
