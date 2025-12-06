using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// Simple data table response DTO.
    /// </summary>
    public class SimpleDataTableResponseDto
    {
        [JsonPropertyName("epg_listings")]
        public Dictionary<string, List<EpgResponseDto>> EpgListings { get; set; } = new();
    }
}

