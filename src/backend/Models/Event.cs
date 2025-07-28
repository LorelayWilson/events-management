namespace EventsSystem.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int Capacity { get; set; }
        public bool IsPrivate { get; set; }
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser CreatedBy { get; set; } = null!;
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
    }
}