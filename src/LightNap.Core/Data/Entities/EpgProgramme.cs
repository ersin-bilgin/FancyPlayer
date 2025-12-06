namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an EPG programme entry.
    /// </summary>
    public class EpgProgramme
    {
        public int Id { get; set; }
        public int EpgChannelId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation properties
        public EpgChannel EpgChannel { get; set; } = null!;
    }
}

