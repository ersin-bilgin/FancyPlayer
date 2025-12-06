namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a stream access log entry.
    /// </summary>
    public class StreamLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string StreamType { get; set; } = string.Empty; // live, vod, series
        public int StreamId { get; set; }
        public string? Ip { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        // Navigation properties
        public XtreamUser? User { get; set; }
    }
}

