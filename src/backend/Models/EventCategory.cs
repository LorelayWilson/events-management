namespace EventsSystem.Models
{
    public class EventCategory
    {
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
        
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}