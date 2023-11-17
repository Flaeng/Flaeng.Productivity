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
            var testProjects = Solution.Projects
                .Where(proj => proj.IsTestProject())
                .ToArray();

            DotNetTasks.DotNetTest(opts => opts
                .CombineWith(testProjects
                    .Select<Project, Configure<DotNetTestSettings>>(proj => opts => opts
                        .SetProjectFile(proj.Path)
                    )
                    .ToArray()
                ),
                degreeOfParallelism: testProjects.Length
            );
        });

}
