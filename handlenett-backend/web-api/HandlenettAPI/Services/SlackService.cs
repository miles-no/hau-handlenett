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

        public async Task<string> GetUsersListAsync(string oauthToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var response = await _httpClient.GetAsync("users.list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public async Task<string> GetUserFromIdAsync(string oauthToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var userResponse = await _httpClient.GetAsync($"users.profile.get?user={userId}");
            userResponse.EnsureSuccessStatusCode();
            var userProfileJson = await userResponse.Content.ReadAsStringAsync();

            return userProfileJson;
        }

        public async Task<SlackUser> GetUserProfileFromIdAsync(string oauthToken, string userId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var userResponse = await _httpClient.GetAsync($"users.profile.get?user={userId}");
            userResponse.EnsureSuccessStatusCode();
            var userProfileJson = await userResponse.Content.ReadAsStringAsync();
            var userImageUrl = ExtractImageUrl(userProfileJson);
            if (userImageUrl == null)
            {
                throw new Exception("Image not found");
            }
            return new SlackUser
            {
                Id = userId,
                ImageUrl = userImageUrl
            };
            }

        public async Task<string> GetUserFromEmailAddressAsync(string oauthToken, string userId)
        {
            var user = await GetUsersListAsync(oauthToken);
            //TODO: list --> linq --> GetUserFromIdAsync --> ExtractImageUrl --> DownloadImageAsync
            return "asd";
        }

        public async Task CopyImageToAzureBlobStorage(string oauthToken, SlackUser slackUser)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                using (HttpResponseMessage response = await _httpClient.GetAsync(slackUser.ImageUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream imageStream = await response.Content.ReadAsStreamAsync())
                    {
                        var containerName = _config.GetValue<string>("AzureStorage:ContainerNameUserImages");
                        if (string.IsNullOrEmpty(containerName)) throw new Exception("Missing config");

                        var blobService = new AzureBlobStorageService(containerName, _config);
                        await blobService.UploadBlobAsync(slackUser.Id + ".jpg", imageStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private string? ExtractImageUrl(string userProfileJson)
        {
            var userProfile = JsonSerializer.Deserialize<JsonElement>(userProfileJson);
            return userProfile.GetProperty("profile").GetProperty("image_192").GetString(); // Adjust property based on size needed
        }
    }
}
