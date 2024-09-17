using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;
using HandlenettAPI.Models;

namespace HandlenettAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class ItemsController : ControllerBase
    {
        private readonly GraphServiceClient _graphServiceClient;
        private static readonly List<Item> SampleItems = new List<Item>();


        private readonly ILogger<ItemsController> _logger;

        public ItemsController(ILogger<ItemsController> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            //_graphServiceClient = graphServiceClient;
            SampleItems.Add(new Item { Id = 1, Name = "Melk", CreatedBy = User.Identity.Name, UpdatedBy = User.Identity.Name});
        }

        [HttpGet(Name = "GetItems")]
        public async Task<List<Item>> Get()
        {
            //var user = await _graphServiceClient.Me.Request().GetAsync();
            return SampleItems;
        }
    }
}
