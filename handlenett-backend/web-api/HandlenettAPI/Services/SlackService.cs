using Azure.Storage.Blobs;
using HandlenettAPI.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HandlenettAPI.Services
{
    public class SlackService
    {
        private readonly HttpClient _httpClient;
        private  readonly IConfiguration _config;

        public SlackService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient("SlackClient");
            _config = config;
        }

        public async Task<string> CopyImageToAzureBlobStorage(string oauthToken, SlackUser slackUser)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                using (HttpResponseMessage response = await _httpClient.GetAsync(slackUser.ImageUrlSlack, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    //TODO: Mer ryddig å ha get function her og upload i azureService, men hvordan håndtere stream da?
                    using (Stream imageStream = await response.Content.ReadAsStreamAsync())
                    {
                        var containerName = _config.GetValue<string>("AzureStorage:ContainerNameUserImages");
                        var accountName = _config.GetValue<string>("AzureStorage:AccountName");
                        if (string.IsNullOrEmpty(containerName)) throw new Exception("Missing config");

                        var blobService = new AzureBlobStorageService(containerName, _config);
                        await blobService.UploadBlobAsync(slackUser.Id + ".jpg", imageStream);
                        return $"https://{accountName}.blob.core.windows.net/{containerName}/{slackUser.Id}.jpg";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<SlackUser?> GetUserByEmailAsync(string oauthToken, string email)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

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
