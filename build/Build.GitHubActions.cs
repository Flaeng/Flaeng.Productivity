[GitHubActions(
    "PR",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { /*nameof(Format),*/ nameof(Test) },
    OnPullRequestBranches = new[] { "main", "dev" },
    OnPushIncludePaths = new[] { "src/**" },
    PublishArtifacts = false
)]
[GitHubActions(
    "Publish",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Publish) },
    OnPushBranches = new[] { "main", "dev" },
    OnPushIncludePaths = new[] { "src/**" },
    PublishArtifacts = true
)]
partial class Build
{
}
