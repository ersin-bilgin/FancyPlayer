using System.Text.Json.Serialization;

namespace LightNap.Core.Streaming.Dto.Response
{
    /// <summary>
    /// Category response DTO.
    /// </summary>
    public class CategoryResponseDto
    {
        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = string.Empty;

        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }
    }
}

