using System;
using System.Collections.Generic;
using System.Linq;

using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;

partial class Build
{
    [Parameter("Commit message")] readonly string CommitMessage;

    Target Commit => _ => _
        .Before(Compile)
        .Requires(() => CommitMessage)
        .Executes(() =>
        {
            DotNetTasks.DotNetFormat(opts => opts
                .SetIncludeGenerated(false)
                );

            var isClean = GitTasks.GitHasCleanWorkingCopy();
            Serilog.Log.Information($"Formatting caused changes: {(isClean == false)}");
            if (isClean == false)
            {
                GitTasks.Git("add .");
                GitTasks.Git($"commit -m \"{CommitMessage}\"");
            }
        });
}
