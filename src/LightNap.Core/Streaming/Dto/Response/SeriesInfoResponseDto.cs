using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// Series info response DTO.
    /// </summary>
    public class SeriesInfoResponseDto
    {
        [JsonPropertyName("info")]
        public SeriesInfoDetailDto Info { get; set; } = new();

        [JsonPropertyName("episodes")]
        public Dictionary<string, List<EpisodeDto>> Episodes { get; set; } = new();
    }

    /// <summary>
    /// Series info detail DTO.
    /// </summary>
    public class SeriesInfoDetailDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cover")]
        public string Cover { get; set; } = string.Empty;

        [JsonPropertyName("plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("cast")]
        public string Cast { get; set; } = string.Empty;

        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("rating_5based")]
        public double Rating5Based { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("duration_secs")]
        public int DurationSecs { get; set; }

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; } = string.Empty;

        [JsonPropertyName("series_id")]
        public int SeriesId { get; set; }
    }

    /// <summary>
    /// Episode DTO.
    /// </summary>
    public class EpisodeDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("container_extension")]
        public string ContainerExtension { get; set; } = string.Empty;

        [JsonPropertyName("info")]
        public EpisodeInfoDto Info { get; set; } = new();
    }

    /// <summary>
    /// Episode info DTO.
    /// </summary>
    public class EpisodeInfoDto
    {
        [JsonPropertyName("plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;

        [JsonPropertyName("duration_secs")]
        public int DurationSecs { get; set; }

        [JsonPropertyName("movie_image")]
        public string MovieImage { get; set; } = string.Empty;

        [JsonPropertyName("released")]
        public string Released { get; set; } = string.Empty;
    }
}

