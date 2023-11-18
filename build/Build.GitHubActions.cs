[GitHubActions(
    "PR",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = [nameof(Format), nameof(Test), nameof(TestCoverage)],
    OnPullRequestBranches = ["main", "dev"],
    OnPushExcludePaths = ["docs/**"],
    PublishArtifacts = true
)]
[GitHubActions(
    "Publish",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = [nameof(Publish)],
    OnPushBranches = ["main", "dev"],
    OnPushExcludePaths = ["docs/**"],
    PublishArtifacts = true
)]
[GitHubActions(
    "Stryker",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = [nameof(Stryker)],
    OnPushBranches = ["dev"],
    OnPushExcludePaths = ["docs/**"],
    PublishArtifacts = true
)]
partial class Build
{
}
