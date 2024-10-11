using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HandlenettNotifications
{
    public class ItemsChangeFeed
    {
        private readonly ILogger _logger;

        public ItemsChangeFeed(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ItemsChangeFeed>();
        }

        [Function("ItemsChangeFeed")]
        public void Run([CosmosDBTrigger(
            databaseName: "Handlenett",
            containerName: "ShoppingListItems",
            Connection = "ConnectionStrings--AzureFunctionsCosmosDB",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<MyDocument> input)
        {

            try
            {
                if (input != null && input.Count > 0)
                {
                    _logger.LogInformation("Documents modified: " + input.Count);
                    _logger.LogInformation("First document Id: " + input[0].id);
                }
                else
                {
                    _logger.LogInformation("Input was null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the Cosmos DB trigger.");
            }
        }
    }

    public class MyDocument
    {
        public string id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }
    }
}
