using Microsoft.EntityFrameworkCore;
using EventsSystem.Data;
using EventsSystem.Models;
using EventsSystem.DTOs;

namespace EventsSystem.Services
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllEventsAsync(string? currentUserId = null);
        Task<List<EventDto>> GetEventsByCategoryAsync(int categoryId, string? currentUserId = null);
        Task<EventDto?> GetEventByIdAsync(int id, string? currentUserId = null);
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, string? currentUserId = null);
        Task<bool> RegisterForEventAsync(RegisterEventDto registerDto, string? currentUserId = null);
    }

    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventService> _logger;

        public EventService(ApplicationDbContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<EventDto>> GetAllEventsAsync(string? currentUserId = null)
        {
            var events = await _context.Events
                .Include(e => e.CreatedBy)
                .Include(e => e.Registrations)
                .Include(e => e.EventCategories)
                    .ThenInclude(ec => ec.Category)
                .ToListAsync();

            return events.Select(e => MapToEventDto(e, currentUserId)).ToList();
        }

        public async Task<List<EventDto>> GetEventsByCategoryAsync(int categoryId, string? currentUserId = null)
        {
            var eventEntities = await _context.Events
                .Include(e => e.CreatedBy)
                .Where(e => e.EventCategories.Any(ec => ec.CategoryId == categoryId))
                .ToListAsync();

            var events = new List<EventDto>();

            foreach (var eventEntity in eventEntities)
            {
                var eventDto = await MapToEventDtoWithCategoryIncluded(eventEntity, currentUserId);
                events.Add(eventDto);
            }

            return events;
        }

        private async Task<EventDto> MapToEventDtoWithCategoryIncluded(Event eventEntity, string? currentUserId)
        {
            var registrations = await _context.Registrations
                .Where(r => r.EventId == eventEntity.Id)
                .ToListAsync();

            var isRegistered = false;
            if (currentUserId != null)
            {
                isRegistered = await _context.Registrations
                    .AnyAsync(r => r.EventId == eventEntity.Id && r.UserId == currentUserId);
            }

            var eventCategories = await _context.EventCategories
                .Include(ec => ec.Category)
                .Where(ec => ec.EventId == eventEntity.Id)
                .ToListAsync();

            return new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                EventDate = eventEntity.EventDate,
                Capacity = eventEntity.Capacity,
                IsPrivate = eventEntity.IsPrivate,
                CreatedAt = eventEntity.CreatedAt,
                CreatedById = eventEntity.CreatedById,
                CreatedByName = $"{eventEntity.CreatedBy?.FirstName} {eventEntity.CreatedBy?.LastName}".Trim(),
                RegistrationsCount = registrations.Count,
                Categories = eventCategories.Select(ec => new CategoryDto
                {
                    Id = ec.Category.Id,
                    Name = ec.Category.Name,
                    Color = ec.Category.Color
                }).ToList(),
                IsRegistered = isRegistered
            };
        }

        public async Task<EventDto?> GetEventByIdAsync(int id, string? currentUserId = null)
        {
            var eventEntity = await _context.Events
                .Include(e => e.CreatedBy)
                .Include(e => e.Registrations)
                .Include(e => e.EventCategories)
                    .ThenInclude(ec => ec.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            return eventEntity != null ? MapToEventDto(eventEntity, currentUserId) : null;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            }).ToList();
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, string? currentUserId = null)
        {
            var eventEntity = new Event
            {
                Title = createEventDto.Title,
                Description = createEventDto.Description,
                EventDate = createEventDto.EventDate,
                Capacity = createEventDto.Capacity,
                IsPrivate = createEventDto.IsPrivate,
                CreatedById = createEventDto.CreatedById ?? currentUserId ?? "anonymous"
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            // Add categories
            foreach (var categoryId in createEventDto.CategoryIds)
            {
                _context.EventCategories.Add(new EventCategory
                {
                    EventId = eventEntity.Id,
                    CategoryId = categoryId
                });
            }

            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(eventEntity.Id, currentUserId) ?? throw new Exception("Failed to create event");
        }

        public async Task<bool> RegisterForEventAsync(RegisterEventDto registerDto, string? currentUserId = null)
        {
            var userId = registerDto.UserId ?? currentUserId;
            
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var registration = new Registration
            {
                EventId = registerDto.EventId,
                UserId = userId,
                RegistrationDate = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return true;
        }

        private EventDto MapToEventDto(Event eventEntity, string? currentUserId)
        {
            return new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                EventDate = eventEntity.EventDate,
                Capacity = eventEntity.Capacity,
                IsPrivate = eventEntity.IsPrivate,
                CreatedAt = eventEntity.CreatedAt,
                CreatedById = eventEntity.CreatedById,
                CreatedByName = $"{eventEntity.CreatedBy?.FirstName} {eventEntity.CreatedBy?.LastName}".Trim(),
                RegistrationsCount = eventEntity.Registrations.Count,
                Categories = eventEntity.EventCategories.Select(ec => new CategoryDto
                {
                    Id = ec.Category.Id,
                    Name = ec.Category.Name,
                    Color = ec.Category.Color
                }).ToList(),
                IsRegistered = !string.IsNullOrEmpty(currentUserId) && 
                              eventEntity.Registrations.Any(r => r.UserId == currentUserId)
            };
        }
    }
}