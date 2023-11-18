[GitHubActions(
    "Test",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = new string[] { /*nameof(Format),*/ nameof(Test), nameof(TestCoverage) },
    OnPullRequestBranches = new string[] { "main", "dev" },
    OnPushExcludePaths = new string [] { "docs/**" },
    PublishArtifacts = true
)]
[GitHubActions(
    "Publish",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = new string[] { nameof(Publish) },
    OnPushBranches = new string[] { "main", "dev" },
    OnPushExcludePaths = new string [] { "docs/**" },
    PublishArtifacts = true
)]
[GitHubActions(
    "Stryker test",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = new string[] { nameof(Stryker) },
    OnPushBranches = new string[] { "dev" },
    OnPushExcludePaths = new string [] { "docs/**" },
    PublishArtifacts = true
)]
partial class Build
{
}
