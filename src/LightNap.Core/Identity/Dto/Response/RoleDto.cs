namespace LightNap.Core.Identity.Dto.Response
{
    /// <summary>
    /// Represents a role for the Application Role.
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the role display name.
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        public required string Description { get; set; }
    }
}