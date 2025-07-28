using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace EventsSystem.Tests.Controllers
{
    public class EventsControllerTests
    {
        // This test checks if the GetEvents method returns events for an authenticated user.
        [Fact]
        public async Task GetEvents_AuthenticatedUser_ReturnsEvents()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.GetAllEventsAsync(userId, 1, 20))
                .ReturnsAsync(new PaginatedResult<EventDto>
                {
                    Items = new List<EventDto> { new EventDto { Id = 1, Title = "Evento" } },
                    TotalCount = 1
                });

            var controller = new EventsController(mockService.Object, logger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
        }

        // This test checks if the GetEvents method returns events for an non-authenticated user.
        [Fact]
        public async Task GetEvents_AnonymousUser_ReturnsEvents()
        {
            // Arrange
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.GetAllEventsAsync(null, 1, 20))
                .ReturnsAsync(new PaginatedResult<EventDto>
                {
                    Items = new List<EventDto> { new EventDto { Id = 1, Title = "Evento público" } },
                    TotalCount = 1
                });

            var controller = new EventsController(mockService.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            // Act
            var result = await controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
        }

        // This test checks if the GetEventsByCategory method returns events for an authenticated user.
        [Fact]
        public async Task GetEventsByCategory_AuthenticatedUser_ReturnsEvents()
        {
            // Arrange
            var userId = "user1";
            var categoryId = 5;
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.GetEventsByCategoryAsync(categoryId, userId, 2, 10))
                .ReturnsAsync(new PaginatedResult<EventDto>
                {
                    Items = new List<EventDto> { new EventDto { Id = 11, Title = "Evento Cat" } },
                    TotalCount = 1
                });

            var controller = new EventsController(mockService.Object, logger.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetEventsByCategory(categoryId, 2, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
        }

        // This test checks if the GetEvent method returns Ok when the event exists.
        [Fact]
        public async Task GetEvent_EventExists_ReturnsEvent()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.GetEventByIdAsync(33, userId))
                .ReturnsAsync(new EventDto { Id = 33, Title = "Mi evento" });

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetEvent(33);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<EventDto>(okResult.Value);
            Assert.Equal(33, data.Id);
        }

        // This test checks if the GetEvent method returns NotFound when the event does not exist.

        [Fact]
        public async Task GetEvent_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.GetEventByIdAsync(44, userId))
                .ReturnsAsync((EventDto?)null);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetEvent(44);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // This test checks if the GetCategories method returns a list of categories.
        [Fact]
        public async Task GetCategories_ReturnsListOfCategories()
        {
            // Arrange
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();
            mockService.Setup(s => s.GetCategoriesAsync())
                .ReturnsAsync(new List<CategoryDto>
                {
            new CategoryDto { Id = 1, Name = "Tech" },
            new CategoryDto { Id = 2, Name = "Health" }
                });

            var controller = new EventsController(mockService.Object, logger.Object);

            // Act
            var result = await controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsType<List<CategoryDto>>(okResult.Value);
            Assert.Equal(2, categories.Count);
        }

        // This test checks if the CreateEvent method creates an event successfully.
        [Fact]
        public async Task CreateEvent_Valid_ReturnsCreatedEvent()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();
            var createEventDto = new CreateEventDto { Title = "Nuevo Evento" };

            mockService.Setup(s => s.CreateEventAsync(createEventDto, userId))
                .ReturnsAsync(new EventDto { Id = 101, Title = "Nuevo Evento" });

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.CreateEvent(createEventDto);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var data = Assert.IsType<EventDto>(createdAt.Value);
            Assert.Equal(101, data.Id);
            Assert.Equal("Nuevo Evento", data.Title);
        }

        // This test checks if the RegisterForEvent method successfully registers a user for an event.
        [Fact]
        public async Task RegisterForEvent_Success_ReturnsOk()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();
            var registerDto = new RegisterEventDto { UserId = userId };

            mockService.Setup(s => s.RegisterForEventAsync(It.IsAny<RegisterEventDto>(), userId))
                .ReturnsAsync(true);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.RegisterForEvent(42, registerDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // This test checks if the RegisterForEvent method returns BadRequest when registration fails.
        [Fact]
        public async Task RegisterForEvent_Failure_ReturnsBadRequest()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();
            var registerDto = new RegisterEventDto { UserId = userId };

            mockService.Setup(s => s.RegisterForEventAsync(It.IsAny<RegisterEventDto>(), userId))
                .ReturnsAsync(false);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.RegisterForEvent(42, registerDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Registration failed", badRequest.Value);
        }

        // This test checks if the UnregisterFromEvent method successfully unregisters a user from an event.
        [Fact]
        public async Task UnregisterFromEvent_Success_ReturnsOk()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.UnregisterFromEventAsync(42, userId))
                .ReturnsAsync(true);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.UnregisterFromEvent(42);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // This test checks if the UnregisterFromEvent method returns BadRequest when unregistration fails.
        [Fact]
        public async Task UnregisterFromEvent_Failure_ReturnsBadRequest()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.UnregisterFromEventAsync(42, userId))
                .ReturnsAsync(false);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.UnregisterFromEvent(42);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unregistration failed", badRequest.Value);
        }

        // This test checks if the DeleteEvent method successfully deletes an event.
        [Fact]
        public async Task DeleteEvent_Success_ReturnsNoContent()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.DeleteEventAsync(42, userId))
                .ReturnsAsync(true);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.DeleteEvent(42);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // This test checks if the DeleteEvent method returns BadRequest when deletion fails.
        [Fact]
        public async Task DeleteEvent_Failure_ReturnsBadRequest()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var logger = new Mock<ILogger<EventsController>>();

            mockService.Setup(s => s.DeleteEventAsync(42, userId))
                .ReturnsAsync(false);

            var controller = new EventsController(mockService.Object, logger.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.DeleteEvent(42);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Delete failed", badRequest.Value);
        }



    }
}
