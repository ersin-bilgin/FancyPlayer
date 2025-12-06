namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// Represents the relationship between users and packages.
    /// </summary>
    public class UserPackage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public XtreamUser User { get; set; } = null!;
        public Package Package { get; set; } = null!;
    }
}

