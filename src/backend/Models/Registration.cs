namespace EventsSystem.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}