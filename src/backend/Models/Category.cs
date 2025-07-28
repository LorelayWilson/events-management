namespace EventsSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#3B82F6"; // Default blue color
        public string Icon { get; set; } = string.Empty; // Font Awesome icon class

        // Navigation properties
        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
    }
}