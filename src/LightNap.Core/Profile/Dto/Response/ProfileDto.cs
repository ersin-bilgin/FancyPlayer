namespace LightNap.Core.Profile.Dto.Response
{
    /// <summary>
    /// Data transfer object representing the logged-in user's profile.
    /// </summary>
    public class ProfileDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user profile.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public required string UserName { get; set; }
    }
}