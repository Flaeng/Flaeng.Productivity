[GitHubActions(
    "PR",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = new[] { nameof(Format), nameof(Test)/*, nameof(Stryker)*/ },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = false
)]
[GitHubActions(
    "Build",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = new[] { nameof(Stryker) },
    OnPushBranches = new[] { "main" },
    PublishArtifacts = true
)]
partial class Build
{
}
