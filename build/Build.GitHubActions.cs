[GitHubActions(
    "Housekeeping",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Format), nameof(Test) },
    OnPullRequestBranches = new[] { "main" }
)]
[GitHubActions(
    "Run mutation tests",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Stryker) },
    OnPushBranches = new[] { "main" }
)]
[GitHubActions(
    "Deploy new version",
    GitHubActionsImage.UbuntuLatest,
    // OnPushBranches = new [] { "main" },
    OnPushTags = new[] { @"v*.*.*" },
    InvokedTargets = new[] { nameof(Publish) },
    PublishArtifacts = true
)]
partial class Build
{
}
