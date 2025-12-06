namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an Xtream API user.
    /// </summary>
    public class XtreamUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public bool Status { get; set; } = true;
        public int MaxConnections { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpireDate { get; set; }
        public string? LastIp { get; set; }
        public string? AllowedIps { get; set; }
        public string? AllowedUserAgents { get; set; }

        // Navigation properties
        public ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
        public ICollection<UserConnection> Connections { get; set; } = new List<UserConnection>();
        public ICollection<StreamLog> StreamLogs { get; set; } = new List<StreamLog>();
    }
}

