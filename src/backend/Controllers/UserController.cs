using Microsoft.AspNetCore.Mvc;
using EventsSystem.Services;
using EventsSystem.DTOs;
using System.Security.Claims;

namespace EventsSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IEventService _eventService;

        public UsersController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{id}/events")]
        public async Task<ActionResult<PaginatedResult<EventDto>>> GetUserEvents(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var events = await _eventService.GetEventsByUserAsync(id, currentUserId, page, pageSize);
            return Ok(events);
        }
    }
}