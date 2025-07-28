using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EventsSystem.Tests.Controllers
{
    public class UsersControllerTests
    {
        // Authenticated user gets own events
        [Fact]
        public async Task GetUserEvents_ReturnsOkResultForAuthenticatedUser()
        {
            // Arrange
            var userId = "user1"; 
            var email = "john@test.com";
            var mockService = new Mock<IEventService>();
            mockService
                .Setup(s => s.GetEventsByUserAsync(userId, userId, 1, 20))
                .ReturnsAsync(new PaginatedResult<EventDto>
                {
                    Items = new List<EventDto> { new EventDto { Id = 1, Title = "Event 1" } },
                    TotalCount = 1
                });

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, "John Doe")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(userId, 1, 20);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(returnValue.Items);
            Assert.Equal("Event 1", returnValue.Items[0].Title);
        }

        // No authenticated user gets public events

        [Fact]
        public async Task GetUserEvents_AnonymousUser_ReturnsEvents()
        {
            // Arrange
            var userId = "user2";
            var mockService = new Mock<IEventService>();
            var fakeResult = new PaginatedResult<EventDto>
            {
                Items = new List<EventDto> { new EventDto { Id = 2, Title = "Public Event" } },
                TotalCount = 1
            };

            mockService.Setup(s => s.GetEventsByUserAsync(userId, null, 1, 20))
                       .ReturnsAsync(fakeResult);

            var controller = new UsersController(mockService.Object);

            // Assign an empty user to simulate an anonymous user
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity()) // annonymous user
                }
            };

            // Act
            var result = await controller.GetUserEvents(userId, 1, 20);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
            Assert.Equal("Public Event", data.Items[0].Title);
        }

        // Authenticated user requests events of another user
        [Fact]
        public async Task GetUserEvents_AuthenticatedUser_RequestsOthersEvents()
        {
            // Arrange
            var otherUserId = "user2";
            var authenticatedUserId = "user1";
            var mockService = new Mock<IEventService>();
            var fakeResult = new PaginatedResult<EventDto>
            {
                Items = new List<EventDto> { new EventDto { Id = 3, Title = "Event of another user" } },
                TotalCount = 1
            };

            mockService.Setup(s => s.GetEventsByUserAsync(otherUserId, authenticatedUserId, 1, 20))
                       .ReturnsAsync(fakeResult);

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUserId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(otherUserId, 1, 20);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
            Assert.Equal("Event of another user", data.Items[0].Title);
        }


        // This test checks that the controller returns an empty list when no events are found for the authenticated user.
        [Fact]
        public async Task GetUserEvents_NoEventsFound_ReturnsEmptyList()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var fakeResult = new PaginatedResult<EventDto>
            {
                Items = new List<EventDto>(),
                TotalCount = 0
            };

            mockService.Setup(s => s.GetEventsByUserAsync(userId, userId, 1, 20))
                       .ReturnsAsync(fakeResult);

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(userId, 1, 20);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Empty(data.Items);
            Assert.Equal(0, data.TotalCount);
        }

        // This test checks that the controller throws an exception when the service throws an exception.
        [Fact]
        public async Task GetUserEvents_ServiceThrowsException_Returns500()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();

            mockService.Setup(s => s.GetEventsByUserAsync(userId, userId, 1, 20))
                       .ThrowsAsync(new Exception("DB error"));

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => controller.GetUserEvents(userId, 1, 20));
        }

        // This test checks that the controller calls the service with the correct parameters.

        [Fact]
        public async Task GetUserEvents_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var fakeResult = new PaginatedResult<EventDto>
            {
                Items = new List<EventDto>(),
                TotalCount = 0
            };

            mockService.Setup(s => s.GetEventsByUserAsync(userId, userId, 2, 10))
                       .ReturnsAsync(fakeResult)
                       .Verifiable();

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(userId, 2, 10);

            // Assert
            mockService.Verify();
        }

        // This test checks that the controller can handle custom pagination parameters.
        [Fact]
        public async Task GetUserEvents_CustomPagination_CallsServiceWithCorrectValues()
        {
            // Arrange
            var userId = "user1";
            var page = 3;
            var pageSize = 5;

            var mockService = new Mock<IEventService>();
            var fakeResult = new PaginatedResult<EventDto>
            {
                Items = new List<EventDto>
        {
            new EventDto { Id = 101, Title = "Evento paginado" }
        },
                TotalCount = 25
            };

            mockService
                .Setup(s => s.GetEventsByUserAsync(userId, userId, page, pageSize))
                .ReturnsAsync(fakeResult)
                .Verifiable();

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(userId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
            Assert.Equal("Evento paginado", data.Items[0].Title);

            mockService.Verify();
        }

        // This test checks that the controller returns Ok with null when the service returns null.
        [Fact]
        public async Task GetUserEvents_ServiceReturnsNull_ReturnsOkWithNull()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            mockService
                .Setup(s => s.GetEventsByUserAsync(userId, userId, 1, 20))
                .ReturnsAsync((PaginatedResult<EventDto>?)null);

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(userId, 1, 20);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Null(okResult.Value); 
        }

        // This test checks that the controller uses default parameters when none are provided.
        [Fact]
        public async Task GetUserEvents_DefaultParameters_UsesDefaultValues()
        {
            // Arrange
            var userId = "user1";
            var mockService = new Mock<IEventService>();
            var fakeResult = new PaginatedResult<EventDto>
            {
                Items = new List<EventDto> { new EventDto { Id = 55, Title = "Evento default" } },
                TotalCount = 1
            };

            mockService
                .Setup(s => s.GetEventsByUserAsync(userId, userId, 1, 20))
                .ReturnsAsync(fakeResult)
                .Verifiable();

            var controller = new UsersController(mockService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetUserEvents(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<PaginatedResult<EventDto>>(okResult.Value);
            Assert.Single(data.Items);
            mockService.Verify();
        }



    }
}