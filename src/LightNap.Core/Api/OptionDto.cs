using System.Diagnostics.CodeAnalysis;

namespace LightNap.Core.Api
{
    public class OptionDto
    {
        public required string DisplayName { get; set; }
        public required string Key { get; set; }
        public required string Description { get; set; }

        [SetsRequiredMembers]
        public OptionDto(string key, string displayName, string? description = null)
        {
            this.Description = description ?? string.Empty;
            this.DisplayName = displayName;
            this.Key = key;
        }
    }
}
