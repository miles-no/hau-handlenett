namespace HandlenettAPI.Services
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

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

        // Upload a new blob
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

    }
}
