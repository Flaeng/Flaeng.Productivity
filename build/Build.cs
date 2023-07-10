using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Projects
                .ForEach(proj =>
                    DotNetTasks.DotNetClean(opts => opts
                    .SetProject(proj)));
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Projects
                .ForEach(proj =>
                    DotNetTasks.DotNetRestore(opts => opts
                    .SetProjectFile(proj)));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Projects
                .ForEach(proj =>
                    DotNetTasks.DotNetBuild(opts => opts
                    .SetProjectFile(proj)
                    .SetTreatWarningsAsErrors(true)
                    ));
        });

}
