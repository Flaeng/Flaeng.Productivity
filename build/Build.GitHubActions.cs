[GitHubActions(
    "PR",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Format), nameof(Test), nameof(Stryker) },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = false
)]
[GitHubActions(
    "Build",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Stryker) },
    OnPushBranches = new [] { "main" },
    PublishArtifacts = false
)]
partial class Build
{
}
