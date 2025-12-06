namespace LightNap.Scaffolding.AssemblyManager
{
    /// <summary>
    /// Interface for managing assembly operations.
    /// </summary>
    public interface IAssemblyManager
    {
        /// <summary>
        /// Gets the path of the currently executing assembly.
        /// </summary>
        /// <returns>The executing assembly path as a string.</returns>
        string GetExecutingPath();

        /// <summary>
        /// Loads a type from a specified assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file.</param>
        /// <param name="className">The name of the class to load.</param>
        /// <returns>The loaded type, or null if the type cannot be found.</returns>
        Type? LoadType(string assemblyPath, string className);
    }
}
