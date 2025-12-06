using LightNap.Core.Data.Entities;

namespace LightNap.Core.Configuration
{
    /// <summary>
    /// Provides predefined application roles.
    /// </summary>
    public static class ApplicationRoles
    {
        /// <summary>
        /// Gets the administrator role with access to all administrative features.
        /// </summary>
        public static readonly ApplicationRole Administrator = new(Constants.Roles.Administrator, "Administrator", "Access to all administrative features");
        
        /// <summary>
        /// Gets the member role with access to all member features.
        /// </summary>
        public static readonly ApplicationRole Member = new(Constants.Roles.Member, "Member", "Access to all member features");

        /// <summary>
        /// Gets a read-only list of all predefined application roles.
        /// </summary>
        public static IReadOnlyList<ApplicationRole> All =>
        [
            Administrator,
            Member
        ];
    }
}
