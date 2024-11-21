namespace HandlenettAPI.Services
{
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Sas;

    public class AzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureBlobStorageService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Azure Storage connection string is missing.");
            }
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, overwrite: true);
        }

        // Get existing blob
        public async Task<Stream> GetBlobAsync(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            return download.Content;
        }

        public string GenerateContainerSasToken(string containerName, int hoursValid = 48)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c", // container-level SAS
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(hoursValid)
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            if (!_blobServiceClient.CanGenerateAccountSasUri)
            {
                throw new InvalidOperationException("The provided client cannot generate SAS tokens.");
            }

            return blobContainerClient.GenerateSasUri(sasBuilder).Query;

            //Old code that worked, test new solution before deleting
            //var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
            //    _blobServiceClient.AccountName,
            //    _config.GetValue<string>("AzureStorage:AccountKey"))).ToString();

            //return sasToken;
        }

    }
}
