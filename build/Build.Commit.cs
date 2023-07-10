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
    Target Housekeeping => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetFormat(opts => opts
                .SetIncludeGenerated(false)
                );

            if (GitTasks.GitHasCleanWorkingCopy())
                return;

            GitTasks.Git("config --global user.name '@Flaeng'");
            GitTasks.Git("config --global user.email 'flaeng@users.noreply.github.com'");
            GitTasks.Git($"commit -am \"{nameof(Housekeeping)}\"");
            GitTasks.Git("push");
        });
}
