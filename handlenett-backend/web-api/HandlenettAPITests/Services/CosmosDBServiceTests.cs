using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Azure.Cosmos;
using HandlenettAPI.Services;
using HandlenettAPI.DTO;
using HandlenettAPI.Models;


namespace HandlenettAPITests.Services
{
    public class CosmosDBServiceTests
    {
        private readonly Mock<Container> _mockContainer;
        private readonly CosmosDBService _service;

        public CosmosDBServiceTests()
        {
            _mockContainer = new Mock<Container>();
            var mockCosmosClient = new Mock<CosmosClient>();
            
            mockCosmosClient
                .Setup(client => client.GetContainer("TestDatabase", "TestContainer"))
                .Returns(_mockContainer.Object);

            _service = new CosmosDBService(mockCosmosClient.Object, "TestDatabase", "TestContainer");
        }

        [Fact]
        public async Task Add_ReturnsCreatedItem()
        {
            // Arrange: Prepare input data
            var itemDto = new ItemPostDTO { Name = "Test Item" };
            var username = "testuser";
            var expectedItem = new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Item",
                CreatedBy = username,
                UpdatedBy = username
            };

            // Simulate Cosmos DB's response
            _mockContainer
                .Setup(container => container.CreateItemAsync(
                    It.Is<Item>(i => i.Name == expectedItem.Name && i.CreatedBy == expectedItem.CreatedBy),
                    It.IsAny<PartitionKey>(),
                    null,
                    default))
                .ReturnsAsync(Helpers.CosmosResponseFactory.CreateItemResponse(expectedItem));


            // Act: Call the method under test
            var result = await _service.Add(itemDto, username);

            // Assert: Verify the returned item
            Assert.NotNull(result);
            Assert.Equal(expectedItem.Name, result.Name);
            Assert.Equal(expectedItem.CreatedBy, result.CreatedBy);

            // Verify that the container's CreateItemAsync method was called
            _mockContainer.Verify(container => container.CreateItemAsync(
                It.Is<Item>(i => i.Name == "Test Item"),
                It.IsAny<PartitionKey>(),
                null,
                default), Times.Once);
        }
    }
}
