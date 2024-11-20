using Microsoft.Azure.Cosmos;
using Moq;
using System.Net;

namespace HandlenettAPITests.Helpers
{
    public static class CosmosResponseFactory
    {
        public static ItemResponse<T> CreateItemResponse<T>(T item)
        {
            var responseMock = new Mock<ItemResponse<T>>();
            responseMock.SetupGet(r => r.Resource).Returns(item);
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.Created);
            return responseMock.Object;
        }
    }
}