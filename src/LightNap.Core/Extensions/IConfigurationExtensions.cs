using Microsoft.Extensions.Configuration;

namespace LightNap.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IConfiguration"/>.
    /// </summary>
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Gets the value of a required setting from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The value of the setting.</returns>
        /// <exception cref="ArgumentException">Thrown when the required setting is missing.</exception>
        public static string GetRequiredSetting(this IConfiguration configuration, string key)
        {
            return configuration[key] ?? throw new ArgumentException($"Required setting '{key}' is missing");
        }
    }
}