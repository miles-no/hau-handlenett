using HandlenettAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using System.Net;

//[Authorize]
[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public HomeController(ITokenAcquisition tokenAcquisition, ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _tokenAcquisition = tokenAcquisition;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    [HttpGet(Name = "test")]
    public async Task<IActionResult> IndexAsync()
    {
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { $"api://{_config.GetValue<string>("AzureAd:ClientId")}/Tiny.Read" });

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7076/home/getData")
        {
            Headers =
            {
                { HeaderNames.Authorization, "Bearer "+ accessToken}
            }
        };

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.SendAsync(httpRequestMessage);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = await response.Content.ReadAsStringAsync();
        }
        return View();
    }

    //[AllowAnonymous]
    //public string getData()
    //{
    //    return "success";
    //}
}