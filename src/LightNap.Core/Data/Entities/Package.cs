namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents a package that can be assigned to users.
    /// </summary>
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<UserPackage> UserPackages { get; set; } = new List<UserPackage>();
    }
}

