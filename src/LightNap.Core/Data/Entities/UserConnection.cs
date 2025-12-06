namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an active user connection for max_connections tracking.
    /// </summary>
    public class UserConnection
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastCheck { get; set; }

        // Navigation properties
        public XtreamUser User { get; set; } = null!;
    }
}

