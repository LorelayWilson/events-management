using EventsSystem.Data;
using EventsSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsSystem.Tests.Services
{
    public class EventServiceTest
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private ILogger<EventService> GetLogger()
        {
            return new Mock<ILogger<EventService>>().Object;
        }

        // This test checks if the service can retrieve all public events correctly if no user is logged in.
        [Fact]
        public async Task GetAllEventsAsync_WithoutUser_ReturnsOnlyPublicEvents()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });

            db.Events.Add(new Event
            {
                Id = 1,
                Title = "Public",
                IsPrivate = false,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedById = "user1"
            });
            db.Events.Add(new Event
            {
                Id = 2,
                Title = "Private",
                IsPrivate = true,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedById = "user1"
            });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetAllEventsAsync();

            // Assert
            Assert.Single(result.Items); 
            Assert.Equal("Public", result.Items.First().Title);
        }


        // This test checks if the service can retrieve all events correctly for a logged-in user, including their own private events.
        [Fact]
        public async Task GetAllEventsAsync_WithUser_ReturnsPublicAndOwnPrivateEvents()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });
            db.Users.Add(new ApplicationUser
            {
                Id = "user2",
                UserName = "jane@test.com",
                Email = "jane@test.com",
                FirstName = "Jane",
                LastName = "Smith"
            });
            db.SaveChanges();

            db.Events.Add(new Event { Id = 1, Title = "Public", IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedById = "user1" });
            db.Events.Add(new Event { Id = 2, Title = "Private by me", IsPrivate = true, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedById = "user1" });
            db.Events.Add(new Event { Id = 3, Title = "Private by other", IsPrivate = true, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedById = "user2" });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetAllEventsAsync("user1");

            // Assert
            Assert.Equal(2, result.Items.Count); // Public + Private by me
            Assert.Contains(result.Items, e => e.Title == "Public");
            Assert.Contains(result.Items, e => e.Title == "Private by me");
        }


        // This test checks if the service can retrieve only events created by the user.
        [Fact]
        public async Task GetEventsByUserAsync_ReturnsUserEvents()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });
            db.Users.Add(new ApplicationUser
            {
                Id = "user2",
                UserName = "jane@test.com",
                Email = "jane@test.com",
                FirstName = "Jane",
                LastName = "Smith"
            });
            db.SaveChanges();

            db.Events.Add(new Event
            {
                Id = 1,
                Title = "A",
                CreatedById = "user1",
                IsPrivate = false,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
            db.Events.Add(new Event
            {
                Id = 2,
                Title = "B",
                CreatedById = "user2",
                IsPrivate = false,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetEventsByUserAsync("user1");

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("A", result.Items.First().Title);
        }


        // This test checks if the service returns Ok when acceessed by a user who is the owner of the event.
        [Fact]
        public async Task GetEventByIdAsync_PrivateEvent_OwnerUser_CanSee()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });
            db.SaveChanges();

            db.Events.Add(new Event
            {
                Id = 10,
                Title = "Privado",
                CreatedById = "user1",
                IsPrivate = true,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetEventByIdAsync(10, "user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Privado", result!.Title);
        }


        // This test checks if the service returns null for a private event when accessed by a user who is not the owner.
        [Fact]
        public async Task GetEventByIdAsync_PrivateEvent_OtherUser_CannotSee()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Events.Add(new Event { Id = 10, Title = "Privado", CreatedById = "user1", IsPrivate = true, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetEventByIdAsync(10, "user2");

            // Assert
            Assert.Null(result);
        }

        // This test checks if the service returns Ok when creating an event with a valid DTO.
        [Fact]
        public async Task CreateEventAsync_CreatesEventAndReturnsDto()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });

            db.Categories.Add(new Category { Id = 1, Name = "Tech" });
            db.SaveChanges();

            var service = new EventService(db, logger);
            var dto = new CreateEventDto
            {
                Title = "Test event",
                Description = "desc",
                EventDate = DateTime.UtcNow,
                Capacity = 10,
                IsPrivate = false,
                Address = "Calle Falsa 123",
                CategoryIds = new List<int> { 1 }
            };

            // Act
            var result = await service.CreateEventAsync(dto, "user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test event", result.Title);
            Assert.Contains(result.Categories, c => c.Name == "Tech");
            Assert.Equal("John Doe", result.CreatedByName); 
        }



        // This test checks if the service returns OK registering for an event.
        [Fact]
        public async Task RegisterForEventAsync_SuccessfulRegistration_ReturnsTrue()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();
            db.Events.Add(new Event { Id = 1, Capacity = 2, IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var dto = new RegisterEventDto { EventId = 1, UserId = "user1" };
            var result = await service.RegisterForEventAsync(dto);

            // Assert
            Assert.True(result);
            Assert.Single(db.Registrations.ToList());
        }

        // This test checks if the service returns false when trying to register for an event where the user is already registered.
        [Fact]
        public async Task RegisterForEventAsync_DuplicateRegistration_ReturnsFalse()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();
            db.Events.Add(new Event { Id = 1, Capacity = 2, IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.Registrations.Add(new Registration { EventId = 1, UserId = "user1", RegistrationDate = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var dto = new RegisterEventDto { EventId = 1, UserId = "user1" };
            var result = await service.RegisterForEventAsync(dto);

            // Assert
            Assert.False(result);
        }

        // This test checks if the service can unregister a user from an event successfully.
        [Fact]
        public async Task UnregisterFromEventAsync_ExistingRegistration_RemovesRegistration()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();
            db.Events.Add(new Event { Id = 1, Capacity = 2, IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.Registrations.Add(new Registration { EventId = 1, UserId = "user1", RegistrationDate = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.UnregisterFromEventAsync(1, "user1");

            // Assert
            Assert.True(result);
            Assert.Empty(db.Registrations.ToList());
        }

        // This test checks if the service returns false when trying to unregister from an event where the user is not registered.
        [Fact]
        public async Task UnregisterFromEventAsync_NonExistentRegistration_ReturnsFalse()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();
            db.Events.Add(new Event { Id = 1, Capacity = 2, IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.UnregisterFromEventAsync(1, "user1");

            // Assert
            Assert.False(result);
        }

        // This test checks if the service deletes an event when the creator user tries to delete it.
        [Fact]
        public async Task DeleteEventAsync_CreatorUser_DeletesEvent()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();
            db.Events.Add(new Event { Id = 100, CreatedById = "user1", EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.DeleteEventAsync(100, "user1");

            // Assert
            Assert.True(result);
            Assert.Null(db.Events.Find(100));
        }

        // This test checks if the service does not delete an event when a user who is not the creator tries to delete it.
        [Fact]
        public async Task DeleteEventAsync_NonCreatorUser_DoesNotDeleteEvent()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();
            db.Events.Add(new Event { Id = 100, CreatedById = "user1", EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.DeleteEventAsync(100, "user2");

            // Assert
            Assert.False(result);
            Assert.NotNull(db.Events.Find(100));
        }

        // This test checks if the service retrieves all categories correctly.
        [Fact]
        public async Task GetCategoriesAsync_ReturnsAllCategories()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Categories.AddRange(
                new Category { Id = 1, Name = "Tech", Color = "#fff", Icon = "💻" },
                new Category { Id = 2, Name = "Business", Color = "#000", Icon = "💼" }
            );
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetCategoriesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Tech");
            Assert.Contains(result, c => c.Icon == "💼");
        }

        // This test checks if the service retrieves events by category correctly.
        [Fact]
        public async Task GetEventsByCategoryAsync_ReturnsOnlyMatchingCategory()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });
            db.SaveChanges();

            db.Categories.AddRange(
                new Category { Id = 1, Name = "Tech" },
                new Category { Id = 2, Name = "Business" }
            );
            db.SaveChanges();

            db.Events.AddRange(
                new Event { Id = 1, Title = "Evento Tech", IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedById = "user1" },
                new Event { Id = 2, Title = "Evento Biz", IsPrivate = false, EventDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedById = "user1" }
            );
            db.SaveChanges();

            db.EventCategories.AddRange(
                new EventCategory { EventId = 1, CategoryId = 1 },
                new EventCategory { EventId = 2, CategoryId = 2 }
            );
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetEventsByCategoryAsync(1);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("Evento Tech", result.Items.First().Title);
        }


        // This test checks if the service returns correct pagination when retrieving events by category.
        [Fact]
        public async Task GetEventsByCategoryAsync_Pagination_Works()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });
            db.SaveChanges();

            db.Categories.Add(new Category { Id = 1, Name = "Tech" });
            db.SaveChanges();

            for (int i = 1; i <= 15; i++)
            {
                db.Events.Add(new Event
                {
                    Id = i,
                    Title = $"Evento {i}",
                    IsPrivate = false,
                    EventDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = "user1"
                });
            }
            db.SaveChanges();

            for (int i = 1; i <= 15; i++)
            {
                db.EventCategories.Add(new EventCategory
                {
                    EventId = i,
                    CategoryId = 1
                });
            }
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var page1 = await service.GetEventsByCategoryAsync(1, null, page: 1, pageSize: 10);
            var page2 = await service.GetEventsByCategoryAsync(1, null, page: 2, pageSize: 10);

            // Assert
            Assert.Equal(10, page1.Items.Count);
            Assert.Equal(5, page2.Items.Count);
            Assert.Equal(15, page1.TotalCount); // TotalCount
        }


        // This test checks if the service retrieves all events with pagination.
        [Fact]
        public async Task GetAllEventsAsync_Pagination_Works()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });

            for (int i = 1; i <= 25; i++)
            {
                db.Events.Add(new Event
                {
                    Id = i,
                    Title = $"Evento {i}",
                    IsPrivate = false,
                    EventDate = DateTime.UtcNow.AddDays(i),
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    CreatedById = "user1" 
                });
            }
            db.SaveChanges(); 

            var service = new EventService(db, logger);

            // Act
            var page1 = await service.GetAllEventsAsync(null, page: 1, pageSize: 10);
            var page3 = await service.GetAllEventsAsync(null, page: 3, pageSize: 10);

            // Assert
            Assert.Equal(10, page1.Items.Count);
            Assert.Equal(5, page3.Items.Count);
            Assert.Equal(25, page1.TotalCount);
        }


        // This test checks if the service retrieves an event by ID when the event is public.
        [Fact]
        public async Task GetEventByIdAsync_PublicEvent_ReturnsEvent()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "user1",
                UserName = "john@test.com",
                Email = "john@test.com",
                FirstName = "John",
                LastName = "Doe"
            });
            db.SaveChanges();

            db.Events.Add(new Event
            {
                Id = 1,
                Title = "Público",
                IsPrivate = false,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedById = "user1"
            });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var resultAnon = await service.GetEventByIdAsync(1, null);
            var resultUser = await service.GetEventByIdAsync(1, "otroUser");

            // Assert
            Assert.NotNull(resultAnon);
            Assert.NotNull(resultUser);
            Assert.Equal("Público", resultAnon!.Title);
        }


        // This test checks if the service retrieves a private event when the user is registered for it.
        [Fact]
        public async Task GetEventByIdAsync_PrivateEvent_RegisteredUser_CanSee()
        {
            // Arrange
            var db = GetDbContext(Guid.NewGuid().ToString());
            var logger = GetLogger();

            db.Users.Add(new ApplicationUser
            {
                Id = "owner",
                UserName = "owner@test.com",
                Email = "owner@test.com",
                FirstName = "Owner",
                LastName = "User"
            });
            db.Users.Add(new ApplicationUser
            {
                Id = "invited",
                UserName = "invited@test.com",
                Email = "invited@test.com",
                FirstName = "Invited",
                LastName = "User"
            });
            db.SaveChanges();

            db.Events.Add(new Event
            {
                Id = 2,
                Title = "Privado",
                IsPrivate = true,
                EventDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedById = "owner"
            });
            db.SaveChanges();

            db.Registrations.Add(new Registration
            {
                EventId = 2,
                UserId = "invited",
                RegistrationDate = DateTime.UtcNow
            });
            db.SaveChanges();

            var service = new EventService(db, logger);

            // Act
            var result = await service.GetEventByIdAsync(2, "invited");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Privado", result!.Title);
        }


    }

}
