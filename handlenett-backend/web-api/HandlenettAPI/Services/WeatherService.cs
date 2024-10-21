using Azure.Identity;
using HandlenettAPI.Models;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Text.Json;

namespace HandlenettAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ConnectionMultiplexer _redis;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Miles Haugesund Handlenett/1.0 (roger.torkelsen@miles.no)");
            _redis = ConnectionMultiplexer.Connect(config.GetConnectionString("AzureRedisCache"));
        }

        public async Task<Weather> GetWeatherByCoordinatesAsync(double latitude, double longitude)
        {
            Weather? weather = null;

            var cachedValue = GetCacheValue("Weather");
            if (cachedValue != null)
            {
                weather = JsonSerializer.Deserialize<Weather>(cachedValue.ToString());
            }

            if (weather == null || weather.LastUpdated <= DateTime.UtcNow.AddHours(-1))
                {
                    weather = await GetDataFromMet(latitude, longitude);
                    weather.LastUpdated = DateTime.UtcNow;
                    SetCacheValue("Weather", JsonSerializer.Serialize<Weather>(weather));
                }
            return weather;
        }

        private async Task<Weather> GetDataFromMet(double latitude, double longitude)
        {
            var returnModel = new Weather();
            var url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latitude}&lon={longitude}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                returnModel.Temperature = json["properties"]["timeseries"][0]["data"]["instant"]["details"]["air_temperature"].ToString();
                var symbol = json["properties"]["timeseries"][0]["data"]["next_1_hours"]["summary"]["symbol_code"].ToString();

                //TODO: add icons for displaying weather in web app
                //returnModel.ImageUri = new Uri($"https://api.met.no/weatherapi/weathericon/2.0/?symbol={symbol}&content_type=image/png");
                //https://github.com/metno/weathericons

                return returnModel;
            }
            else
            {
                throw new Exception("Unable to retrieve weather data from MET Norway.");
            }
        }

        private void SetCacheValue(string key, string value)
        {
            var db = _redis.GetDatabase();
            db.StringSet(key, value);
        }

        private string? GetCacheValue(string key)
        {
            var db = _redis.GetDatabase();
            return db.StringGet(key);
        }
    }
}
