namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a live TV stream.
    /// </summary>
    public class LiveStream
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string StreamName { get; set; } = string.Empty;
        public string StreamUrl { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public LiveCategory? Category { get; set; }
        public EpgChannel? EpgChannel { get; set; }
    }
}

