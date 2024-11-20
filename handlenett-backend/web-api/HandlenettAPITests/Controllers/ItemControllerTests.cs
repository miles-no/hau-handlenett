using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using HandlenettAPI.Controllers;
using HandlenettAPI.Services;
using Microsoft.Extensions.Configuration;
using HandlenettAPI.Models;
using HandlenettAPI.Interfaces;

namespace HandlenettAPITests.Controllers
{
    public class ItemControllerTests
    {
        private readonly Mock<ILogger<ItemController>> _mockLogger;
        private readonly Mock<ICosmosDBService> _mockCosmosDBService;
        private readonly ItemController _controller;

        //TODO: Add test for config values

        public ItemControllerTests()
        {
            // Set up mocks
            _mockLogger = new Mock<ILogger<ItemController>>();
            _mockCosmosDBService = new Mock<ICosmosDBService>();

            // Create the controller with mocked dependencies
            _controller = new ItemController(_mockLogger.Object, _mockCosmosDBService.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkWithItems_WhenItemsExist()
        {
            // Arrange
            var expectedItems = new List<Item>
        {
            new Item { Id = "1", Name = "Test Item 1" },
            new Item { Id = "2", Name = "Test Item 2" }
        };

            _mockCosmosDBService
                .Setup(service => service.GetByQuery(It.IsAny<string>()))
                .ReturnsAsync(expectedItems);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result); // Check for 200 OK
            var items = Assert.IsAssignableFrom<List<Item>>(okResult.Value); // Ensure correct type
            Assert.Equal(expectedItems.Count, items.Count); // Verify returned items match
        }

        [Fact]
        public async Task Get_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockCosmosDBService
                .Setup(service => service.GetByQuery(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _controller.Get();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // Check for 500
            Assert.Equal(500, statusCodeResult.StatusCode); // Verify status code
            Assert.NotNull(statusCodeResult.Value); // Ensure response body is not null

            // Verify logging
            _mockLogger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(level => level == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to get items")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
