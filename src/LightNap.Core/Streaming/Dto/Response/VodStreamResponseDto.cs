using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// VOD stream response DTO.
    /// </summary>
    public class VodStreamResponseDto
    {
        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; } = string.Empty;

        [JsonPropertyName("stream_id")]
        public int StreamId { get; set; }

        [JsonPropertyName("stream_icon")]
        public string StreamIcon { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("rating_5based")]
        public double Rating5Based { get; set; }

        [JsonPropertyName("added")]
        public string Added { get; set; } = string.Empty;

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonPropertyName("container_extension")]
        public string ContainerExtension { get; set; } = string.Empty;

        [JsonPropertyName("custom_sid")]
        public string CustomSid { get; set; } = string.Empty;

        [JsonPropertyName("direct_source")]
        public string DirectSource { get; set; } = string.Empty;
    }
}

