using Microsoft.AspNetCore.Identity;
using EventsSystem.Models;

namespace EventsSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if already seeded
            if (context.Users.Any())
            {
                return;
            }

            // Create test users
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "john@test.com",
                    Email = "john@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "jane@test.com",
                    Email = "jane@test.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    UserName = "bob@test.com",
                    Email = "bob@test.com",
                    FirstName = "Bob",
                    LastName = "Johnson",
                    EmailConfirmed = true
                }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Password123!");
            }

            await context.SaveChangesAsync();

            // Create categories
            var categories = new List<Category>
            {
                new Category { Name = "Technology", Color = "#3B82F6" },
                new Category { Name = "Business", Color = "#EF4444" },
                new Category { Name = "Health & Wellness", Color = "#10B981" },
                new Category { Name = "Education", Color = "#F59E0B" },
                new Category { Name = "Entertainment", Color = "#8B5CF6" },
                new Category { Name = "Sports", Color = "#F97316" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Get created users
            var johnUser = await userManager.FindByEmailAsync("john@test.com");
            var janeUser = await userManager.FindByEmailAsync("jane@test.com");
            var bobUser = await userManager.FindByEmailAsync("bob@test.com");

            // Create many events
            var events = new List<Event>();
            var random = new Random();

            // Create 150 events
            for (int i = 1; i <= 150; i++)
            {
                var creator = i % 3 == 0 ? johnUser : (i % 3 == 1 ? janeUser : bobUser);
                var isPrivate = i % 5 == 0; // Every 5th event is private
                
                events.Add(new Event
                {
                    Title = $"Sample Event {i}",
                    Description = $"This is a description for event number {i}. It's a great event that you should attend!",
                    EventDate = DateTime.UtcNow.AddDays(random.Next(1, 60)),
                    Capacity = random.Next(10, 100),
                    IsPrivate = isPrivate,
                    CreatedById = creator!.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                });
            }

            context.Events.AddRange(events);
            await context.SaveChangesAsync();

            // Assign random categories to events
            var eventCategories = new List<EventCategory>();
            foreach (var eventEntity in events)
            {
                var categoryCount = random.Next(1, 4); // 1-3 categories per event
                var selectedCategories = categories.OrderBy(x => random.Next()).Take(categoryCount);
                
                foreach (var category in selectedCategories)
                {
                    eventCategories.Add(new EventCategory
                    {
                        EventId = eventEntity.Id,
                        CategoryId = category.Id
                    });
                }
            }

            context.EventCategories.AddRange(eventCategories);
            await context.SaveChangesAsync();

            // Create some registrations
            var registrations = new List<Registration>();
            var allUsers = new[] { johnUser!, janeUser!, bobUser! };
            
            foreach (var eventEntity in events.Take(50)) // Register users for first 50 events
            {
                var userCount = random.Next(1, Math.Min(4, eventEntity.Capacity));
                var selectedUsers = allUsers.OrderBy(x => random.Next()).Take(userCount);
                
                foreach (var user in selectedUsers)
                {
                    registrations.Add(new Registration
                    {
                        EventId = eventEntity.Id,
                        UserId = user.Id,
                        RegistrationDate = DateTime.UtcNow.AddDays(-random.Next(1, 20))
                    });
                }
            }

            context.Registrations.AddRange(registrations);
            await context.SaveChangesAsync();
        }
    }
}