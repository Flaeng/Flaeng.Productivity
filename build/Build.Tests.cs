
using System.IO;
using System.Linq;

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

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
