using EventsSystem.Data;
using EventsSystem.DTOs;
using EventsSystem.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EventsSystem.Services
{
    public interface IEventService
    {
        Task<PaginatedResult<EventDto>> GetAllEventsAsync(string? currentUserId = null, int page = 1, int pageSize = 20);
        Task<PaginatedResult<EventDto>> GetEventsByCategoryAsync(int categoryId, string? currentUserId = null, int page = 1, int pageSize = 20);
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

        public async Task<PaginatedResult<EventDto>> GetAllEventsAsync(string? currentUserId = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Events
                .AsNoTracking()
                .Include(e => e.CreatedBy)
                .Include(e => e.Registrations)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .AsQueryable();

            // Filter out private events unless the current user has access
            if (string.IsNullOrEmpty(currentUserId))
            {
                // If no user is provided, only include public events
                query = query.Where(e => !e.IsPrivate);
            }
            else
            {
                // Include public events or events where the user is the creator or already registered
                query = query.Where(e => !e.IsPrivate ||
                    e.CreatedById == currentUserId ||
                    e.Registrations.Any(r => r.UserId == currentUserId));
            }

            // Count BEFORE pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var events = await query
                .OrderByDescending(e => e.EventDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = events.Select(e => MapToEventDto(e, currentUserId)).ToList();

            return new PaginatedResult<EventDto>
            {
                Items = dtos,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResult<EventDto>> GetEventsByCategoryAsync(int categoryId, string? currentUserId = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Events
                .AsNoTracking()
                .Include(e => e.CreatedBy)
                .Include(e => e.Registrations)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .Where(e => e.EventCategories.Any(ec => ec.CategoryId == categoryId))
                .AsQueryable();

            // Filter out private events unless the current user has access
            if (string.IsNullOrEmpty(currentUserId))
            {
                // If no user is provided, only include public events
                query = query.Where(e => !e.IsPrivate);
            }
            else
            {
                // Include public events or events where the user is the creator or already registered
                query = query.Where(e => !e.IsPrivate ||
                    e.CreatedById == currentUserId ||
                    e.Registrations.Any(r => r.UserId == currentUserId));
            }
            // Count BEFORE pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var events = await query
                .OrderByDescending(e => e.EventDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Directly map using Select and await if mapping is async
            var tasks = events.Select(e => MapToEventDtoWithCategoryIncluded(e, currentUserId));
            var dtos = await Task.WhenAll(tasks).ContinueWith(t => t.Result.ToList());

            return new PaginatedResult<EventDto>
            {
                Items = dtos,
                TotalCount = totalCount
            };
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
                .AsNoTracking()
                .Include(e => e.CreatedBy)
                .Include(e => e.Registrations)
                .Include(e => e.EventCategories).ThenInclude(ec => ec.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null)
                return null;

            // Exclude private events unless the current user is the creator or registered
            if (eventEntity.IsPrivate &&
                (string.IsNullOrEmpty(currentUserId) ||
                 (eventEntity.CreatedById != currentUserId &&
                  !eventEntity.Registrations.Any(r => r.UserId == currentUserId))))
            {
                return null;
            }

            return MapToEventDto(eventEntity, currentUserId);
        }


        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
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

            var eventEntity = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == registerDto.EventId);

            if (eventEntity == null)
            {
                _logger.LogWarning("Attempt to register for non-existent event {EventId}", registerDto.EventId);
                return false;
            }
            // Prevent duplicate registrations
            if (eventEntity.Registrations.Any(r => r.UserId == userId))
            {
                return false;
            }
            // Event is full
            if (eventEntity.Capacity > 0 && eventEntity.Registrations.Count >= eventEntity.Capacity)
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