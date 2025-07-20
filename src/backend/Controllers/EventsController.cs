using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventsSystem.Services;
using EventsSystem.DTOs;
using System.Security.Claims;

namespace EventsSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventDto>>> GetEvents()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = await _eventService.GetAllEventsAsync(currentUserId);
            return Ok(events);
        }

        [HttpGet("categories/{categoryId}")]
        public async Task<ActionResult<List<EventDto>>> GetEventsByCategory(int categoryId)
        {
            var currentUserId = User.Identity?.IsAuthenticated == true 
                ? User.FindFirstValue(ClaimTypes.NameIdentifier) 
                : null;
            var events = await _eventService.GetEventsByCategoryAsync(categoryId, currentUserId);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            string? currentUserId = null;
            if (User.Identity.IsAuthenticated)
            {
                currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            
            var eventDto = await _eventService.GetEventByIdAsync(id, currentUserId: null);
            
            if (eventDto == null)
            {
                return NotFound();
            }

            return Ok(eventDto);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _eventService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto createEventDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            _logger.LogInformation($"Creating event for user: {createEventDto.CreatedById ?? currentUserId ?? "anonymous"}");
            
            var createdEvent = await _eventService.CreateEventAsync(createEventDto, currentUserId);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPost("{id}/register")]
        [Authorize]
        public async Task<ActionResult> RegisterForEvent(int id, RegisterEventDto registerDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            registerDto.EventId = id;
            var success = await _eventService.RegisterForEventAsync(registerDto, currentUserId);
            
            if (!success)
            {
                return BadRequest("Registration failed");
            }

            _logger.LogInformation($"User {registerDto.UserId ?? currentUserId ?? "anonymous"} registered for event {id}");
            
            return Ok();
        }


    }
}