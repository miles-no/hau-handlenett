using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace HandlenettNotifications
{
    public class ItemsChangeFeed
    {
        private IQueueClient? queueClient;
        private readonly ILogger<ItemsChangeFeed> _logger;

        public ItemsChangeFeed(ILogger<ItemsChangeFeed> logger)
        {
            _logger = logger;
        }

        //only works sometimes, likely because of low throughput on free cost tier 
        [Function("ItemsChangeFeed")]
        public async Task Run([CosmosDBTrigger(
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


                    queueClient = new QueueClient(
                       Environment.GetEnvironmentVariable("ConnectionStrings--ServiceBus"),
                       "cosmosdb-updates");

                    foreach (var document in input)
                    {
                        var messageBody = $"Document Id: {document.id} changed";
                        var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                        await queueClient.SendAsync(message);
                        _logger.LogInformation($"Message sent to queue: {messageBody}");
                    }

                    await queueClient.CloseAsync();
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
