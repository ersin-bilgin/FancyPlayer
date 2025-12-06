using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// VOD info response DTO.
    /// </summary>
    public class VodInfoResponseDto
    {
        [JsonPropertyName("info")]
        public VodInfoDetailDto Info { get; set; } = new();
    }

    /// <summary>
    /// VOD info detail DTO.
    /// </summary>
    public class VodInfoDetailDto
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

        [JsonPropertyName("stream_id")]
        public int StreamId { get; set; }

        [JsonPropertyName("container_extension")]
        public string ContainerExtension { get; set; } = string.Empty;
    }
}

