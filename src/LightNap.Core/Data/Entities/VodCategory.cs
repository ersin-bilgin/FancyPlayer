namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a VOD (Video on Demand) category.
    /// </summary>
    public class VodCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;

        // Navigation properties
        public ICollection<VodMovie> Movies { get; set; } = new List<VodMovie>();
    }
}

