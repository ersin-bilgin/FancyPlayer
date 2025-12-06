using System.Reflection;

namespace LightNap.Scaffolding.AssemblyManager
{

    /// <summary>
    /// Manages assembly operations such as retrieving the executing path and loading types from assemblies.
    /// </summary>
    public class AssemblyManager : IAssemblyManager
    {
        /// <summary>
        /// Gets the path of the executing assembly.
        /// </summary>
        /// <returns>The directory path of the executing assembly.</returns>
        public string GetExecutingPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
        }

        /// <summary>
        /// Loads a type from a specified assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file.</param>
        /// <param name="className">The full name of the type to load.</param>
        /// <returns>The type if found; otherwise, null.</returns>
        public Type? LoadType(string assemblyPath, string className)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly.GetType(className);
        }
    }
}
