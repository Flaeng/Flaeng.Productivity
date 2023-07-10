[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Compile) },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = true
)]
[GitHubActions(
    "deploy",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Publish) },
    OnPushTags = new[] { "main" },
    PublishArtifacts = true
)]
[GitHubActions(
    "housekeeping",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Format), nameof(Test), nameof(Stryker) },
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" }
)]
partial class Build
{
}
