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

    Target TestCoverage => _ => _
        .DependsOn(Test)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            if (IsServerBuild)
            {
                DotNetTasks.DotNetToolRestore(opts => opts
                    .SetProcessWorkingDirectory(RootDirectory)
                );
            }
            (Solution.Directory / "dotcover").CreateOrCleanDirectory();
            DotNetTasks.DotNet("dotcover test --dcFilters=+:Flaeng.Productivity --dcOutput=./dotcover/coverage-report.dcvr", workingDirectory: Solution.Directory);
            DotNetTasks.DotNet("dotcover report --source=./dotcover/coverage-report.dcvr --output=./dotcover/coverage.html --reportType=HTML", workingDirectory: Solution.Directory);
            if (IsLocalBuild)
            {
                var htmlfile = Solution.Directory / "dotcover" / "coverage.html";
                new Process
                {
                    StartInfo = new ProcessStartInfo(htmlfile)
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
            else
            {
                var output = Solution.Directory / "dotcover";
                ArtifactsDirectory.CreateDirectory();
                output.MoveToDirectory(ArtifactsDirectory);
            }
        });
}
