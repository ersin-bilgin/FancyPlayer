using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// EPG response DTO.
    /// </summary>
    public class EpgResponseDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("lang")]
        public string Lang { get; set; } = string.Empty;

        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; } = string.Empty;

        [JsonPropertyName("start_timestamp")]
        public long StartTimestamp { get; set; }

        [JsonPropertyName("stop_timestamp")]
        public long StopTimestamp { get; set; }
    }
}

