using HandlenettAPI.DTO;
using HandlenettAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HandlenettAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserInitializationService _userInitializationService;

        public UserController(UserService userService, UserInitializationService userInitializationService)
        {
            _userService = userService;
            _userInitializationService = userInitializationService;
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<List<UserDTO>>> Get()
        {
            return Ok(_userService.GetUsers());
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<UserDTO>> Get(Guid id)
        {
            return Ok(_userService.GetUser(id));
        }
    }
}
