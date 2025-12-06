using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// Short EPG response DTO.
    /// </summary>
    public class ShortEpgResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("epg_listings")]
        public List<EpgResponseDto> EpgList { get; set; } = new();
    }
}

