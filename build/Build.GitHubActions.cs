using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Compile) },
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = true
)]
[GitHubActions(
    "deploy",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Publish) },
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = true
)]
partial class Build
{
}
