namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an episode of a TV series.
    /// </summary>
    public class Episode
    {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string StreamUrl { get; set; } = string.Empty;
        public int? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }

        // Navigation properties
        public Series Series { get; set; } = null!;
    }
}

