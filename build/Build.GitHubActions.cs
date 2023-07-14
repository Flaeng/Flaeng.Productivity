[GitHubActions(
    "Deploy new RC",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "main" },
    InvokedTargets = new[] { nameof(PublishRC) },
    PublishArtifacts = true
)]
[GitHubActions(
    "Deploy new version",
    GitHubActionsImage.UbuntuLatest,
    OnPushTags = new[] { @"^v[0-9]+\.[0-9]+\.[0-9]+" },
    InvokedTargets = new[] { nameof(Publish) },
    PublishArtifacts = true
)]
[GitHubActions(
    "Run mutation tests",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Stryker) },
    OnPushBranches = new[] { "main" }
)]
[GitHubActions(
    "Housekeeping",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Format), nameof(Test) },
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" }
)]
partial class Build
{
}
