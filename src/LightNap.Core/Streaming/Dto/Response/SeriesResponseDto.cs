using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// Series response DTO.
    /// </summary>
    public class SeriesResponseDto
    {
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("series_id")]
        public int SeriesId { get; set; }

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

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;
    }
}

