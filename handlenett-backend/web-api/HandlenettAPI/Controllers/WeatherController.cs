using HandlenettAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandlenettAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private Uri yrLogo = new Uri("https://www.yr.no/assets/images/logo-yr--circle.svg");

        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet] //Default lat/long for Miles Haugesund
        public async Task<IActionResult> GetWeather(double latitude = 59.408175834503076, double longitude = 5.273991771778363)
        {
            return Ok(await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude));
        }
    }
}
