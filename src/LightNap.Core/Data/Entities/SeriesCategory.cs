namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a series category.
    /// </summary>
    public class SeriesCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public ICollection<Series> Series { get; set; } = new List<Series>();
    }
}

