using HandlenettAPI.Configurations;
using HandlenettAPI.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HandlenettAPI.Services
{
    public class SlackService
    {
        private readonly HttpClient _httpClient;
        private readonly AzureBlobStorageService _blobStorageService;
        private readonly SlackSettings _settings;

        public SlackService(HttpClient httpClient, AzureBlobStorageService blobStorageService, IOptions<SlackSettings> options)
        {
            _httpClient = httpClient;
            _blobStorageService = blobStorageService;
            _settings = options.Value;
        }

        public string GetSlackToken()
        {
            if (string.IsNullOrEmpty(_settings.SlackBotUserOAuthToken))
            {
                throw new InvalidOperationException("Slack Bot User OAuth token is not configured.");
            }
            return _settings.SlackBotUserOAuthToken;
        }

        public async Task<string> CopyImageToAzureBlobStorage(SlackUser slackUser)
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(slackUser.ImageUrlSlack, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream imageStream = await response.Content.ReadAsStreamAsync())
                    {
                        var blobName = $"{slackUser.Id}.jpg";
                        await _blobStorageService.UploadBlobAsync(_settings.ContainerNameUserImages, blobName, imageStream);

                        return $"{_settings.BlobStoragePathIncludingContainer}{blobName}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error copying image to Azure Blob Storage", ex);
            }
        }

        public async Task<SlackUser?> GetUserByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync("users.list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            using (JsonDocument document = JsonDocument.Parse(content))
            {
                var members = document.RootElement.GetProperty("members").EnumerateArray();

                var user = members
                    .FirstOrDefault(m =>
                        m.GetProperty("profile").TryGetProperty("email", out JsonElement emailElement)
                        && emailElement.GetString() == email);

                if (user.ValueKind != JsonValueKind.Undefined)
                {
                    string? userId = user.GetProperty("id").GetString();
                    string? imageUrl = user.GetProperty("profile").GetProperty("image_192").GetString();

                    if (userId != null)
                    {
                        return new SlackUser
                        {
                            Id = userId,
                            ImageUrlSlack = imageUrl
                        };
                    }
                }
                return null;
            }
        }
    }
}
