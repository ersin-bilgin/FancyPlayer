namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents an EPG channel mapping.
    /// </summary>
    public class EpgChannel
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string EpgId { get; set; } = string.Empty;
        public string? DisplayName { get; set; }

        // Navigation properties
        public LiveStream Channel { get; set; } = null!;
        public ICollection<EpgProgramme> Programmes { get; set; } = new List<EpgProgramme>();
    }
}

