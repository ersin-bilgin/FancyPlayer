using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an application role with additional properties for description and display name.
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Gets or sets the description of the role.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name of the role.
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class with the specified name, display name, and description.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <param name="displayName">The display name of the role.</param>
        /// <param name="description">The description of the role.</param>
        [SetsRequiredMembers]
        public ApplicationRole(string name, string displayName, string description) : base(name)
        {
            Description = description;
            DisplayName = displayName;
        }
    }
}
