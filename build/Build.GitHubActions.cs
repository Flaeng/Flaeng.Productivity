[GitHubActions(
    "PR",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = [/*nameof(Format),*/ nameof(Test), nameof(TestCoverage)],
    OnPullRequestBranches = ["main", "dev"],
    OnPushIncludePaths = ["src/**"],
    PublishArtifacts = true
)]
[GitHubActions(
    "Publish",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = [nameof(Publish)],
    OnPushBranches = ["main", "dev"],
    OnPushIncludePaths = ["src/**"],
    PublishArtifacts = true
)]
partial class Build
{
}
