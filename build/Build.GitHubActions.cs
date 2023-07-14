[GitHubActions(
    "Deploy new RC",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(PublishRC) },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = true
)]
[GitHubActions(
    "Deploy new version",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
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
