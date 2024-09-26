using Microsoft.Azure.Documents.Client;
using System.Reflection.Metadata;
using Microsoft.Extensions.Configuration;

namespace HandlenettAPI.Services
{
    public static class Repository<T> where T : class
    {
    //    private static readonly string DatabaseId = ConfigurationHelper.config.GetSection("database").Value;
    //    //private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
    //    private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
    //    private static DocumentClient client;

    //    public static void Initialize()
    //    {
    //        client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
    //        CreateDatabaseIfNotExistsAsync().Wait();
    //        CreateCollectionIfNotExistsAsync().Wait();
    //        //var asd = ConfigurationManager.getva
    //    }

    //    private static async Task CreateDatabaseIfNotExistsAsync()
    //    {
    //        try
    //        {
    //            await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
    //        }
    //        catch (DocumentClientException e)
    //        {
    //            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
    //            {
    //                await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //    }

    //    private static async Task CreateCollectionIfNotExistsAsync()
    //    {
    //        try
    //        {
    //            await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
    //        }
    //        catch (DocumentClientException e)
    //        {
    //            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
    //            {
    //                await client.CreateDocumentCollectionAsync(
    //                    UriFactory.CreateDatabaseUri(DatabaseId),
    //                    new DocumentCollection { Id = CollectionId },
    //                    new RequestOptions { OfferThroughput = 1000 });
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //    }

    //    public static async Task<IEnumerable<T>> GetItemsAsync()
    //    {
    //        IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
    //            UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
    //            .AsDocumentQuery();

    //        List<T> results = new List<T>();
    //        while (query.HasMoreResults)
    //        {
    //            results.AddRange(await query.ExecuteNextAsync<T>());
    //        }

    //        return results;
    //    }

    //    public static async Task<Document> CreateItemAsync(T item)
    //    {
    //        return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
    //    }
    }
}
