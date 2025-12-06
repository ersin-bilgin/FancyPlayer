namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a VOD movie.
    /// </summary>
    public class VodMovie
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverUrl { get; set; }
        public string StreamUrl { get; set; } = string.Empty;
        public int? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal? Rating { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public VodCategory? Category { get; set; }
    }
}

