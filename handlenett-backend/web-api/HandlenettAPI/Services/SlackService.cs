using HandlenettAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HandlenettAPI.Services
{
    public class SlackService
    {
        private readonly HttpClient _httpClient;

        public SlackService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SlackClient");
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

        public async Task<SlackUser> GetUserImageFromIdAsync(string oauthToken, string userId)
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
            var slackUser = new SlackUser
            {
                Id = userId,
                Image = await DownloadImageAsync(userImageUrl)
            };

            return slackUser; 
        }

        public async Task<string> GetUserFromEmailAddressAsync(string oauthToken, string userId)
        {
            var user = await GetUsersListAsync(oauthToken);
            //TODO: list --> linq --> GetUserFromIdAsync --> ExtractImageUrl --> DownloadImageAsync
            return "asd";
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        private string? ExtractImageUrl(string userProfileJson)
        {
            var userProfile = JsonSerializer.Deserialize<JsonElement>(userProfileJson);
            return userProfile.GetProperty("profile").GetProperty("image_192").GetString(); // Adjust property based on size needed
        }
    }
}
