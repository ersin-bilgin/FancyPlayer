namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a live TV category.
    /// </summary>
    public class LiveCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public ICollection<LiveStream> Streams { get; set; } = new List<LiveStream>();
    }
}

