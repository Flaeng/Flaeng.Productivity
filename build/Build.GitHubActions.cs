[GitHubActions(
    "PR",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Format), nameof(Test), nameof(Stryker) },
    OnPullRequestBranches = new[] { "main" }
)]
[GitHubActions(
    "Deploy",
    GitHubActionsImage.UbuntuLatest,
    OnPushTags = new[] { @"v*.*.*" },
    InvokedTargets = new[] { nameof(Publish) },
    PublishArtifacts = true
)]
partial class Build
{
}
