using HandlenettAPI.Models;
using Newtonsoft.Json.Linq;

namespace HandlenettAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Miles Haugesund Handlenett/1.0 (roger.torkelsen@miles.no)");
        }

        public async Task<Weather> GetWeatherByCoordinatesAsync(double latitude, double longitude)
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
                //returnModel.ImageUri = new Uri($"https://api.met.no/weatherapi/weathericon/2.0/?symbol={symbol}&content_type=image/png");
                //https://github.com/metno/weathericons
                return returnModel;
            }
            else
            {
                throw new Exception ("Unable to retrieve weather data from MET Norway.");
            }
        }


    }
}
