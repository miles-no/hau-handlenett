namespace HandlenettAPI.Services
{
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Sas;

    public class AzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _config;
        private readonly string _containerName;

        public AzureBlobStorageService(string containerName, IConfiguration config)
        {
            _config = config;
            _containerName = containerName;
            var asd = _config.GetConnectionString("AzureStorageUsingAccessKey");
            _blobServiceClient = new BlobServiceClient(_config.GetConnectionString("AzureStorageUsingAccessKey"));
        }

        public async Task UploadBlobAsync(string blobName, Stream content)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, overwrite: true);
        }

        // Get existing blob
        public async Task<Stream> GetBlobAsync(string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            return download.Content;
        }

        public string GenerateContainerSasToken()
        {
            var blobServiceClient = new BlobServiceClient(_config.GetConnectionString("AzureStorageUsingAccessKey"));

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                Resource = "c", // container-level SAS
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(48)
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
                blobServiceClient.AccountName,
                _config.GetValue<string>("AzureStorage:AccountKey"))).ToString();

            return sasToken;
        }

    }
}
