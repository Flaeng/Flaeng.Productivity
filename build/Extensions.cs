using Nuke.Common.ProjectModel;

public static class ProjectExtensions
{
    public static bool IsTestProject(this Project project)
        => project.Name.EndsWith(".Tests");
}
