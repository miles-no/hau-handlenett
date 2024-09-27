using Microsoft.AspNetCore.Mvc;

namespace HandlenettAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("Hello from Azure v14");
        }
    }
}
