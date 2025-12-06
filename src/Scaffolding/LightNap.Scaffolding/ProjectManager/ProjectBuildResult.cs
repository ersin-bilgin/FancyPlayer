namespace LightNap.Scaffolding.ProjectManager
{
    /// <summary>
    /// Represents the result of a project build.
    /// </summary>
    public class ProjectBuildResult
    {
        /// <summary>
        /// The path to the output assembly.
        /// </summary>
        public string? OutputAssemblyPath { get; set; }

        /// <summary>
        /// True if successful.
        /// </summary>
        public bool Success { get; set; }
    }
}
