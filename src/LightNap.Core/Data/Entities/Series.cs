namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a TV series.
    /// </summary>
    public class Series
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverUrl { get; set; }
        public decimal? Rating { get; set; }

        // Navigation properties
        public SeriesCategory? Category { get; set; }
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}

