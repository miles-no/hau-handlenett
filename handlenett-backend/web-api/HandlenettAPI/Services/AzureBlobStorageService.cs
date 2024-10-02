namespace HandlenettAPI.Services
{
    using Azure.Storage.Blobs;
    public class AzureBlobStorageService
    {
        public async Task UploadImageToBlobAsync(byte[] imageBytes, string blobName, string connectionString, string containerName)
        {
            // Inject Service client?, construct containerClient from param containerName
            var blobServiceClient = new BlobServiceClient(connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create the container if it doesn't exist, don't want this?
            //await blobContainerClient.CreateIfNotExistsAsync();

            // Get a reference to the blob, is this correct?
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            using var stream = new MemoryStream(imageBytes);
            await blobClient.UploadAsync(stream, overwrite: true);
        }

    }
}
