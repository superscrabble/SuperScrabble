namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.InputModels;
    using SuperScrabble.Services;
    using SuperScrabble.CustomExceptions;

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("all")]
        public ActionResult GetAll()
        {
            var usernames = new[] { "Steven", "John", "Michael", "Martin" };
            return Ok(usernames);
        }

        [HttpGet("auth")]
        [Authorize]
        public ActionResult GetAllAuthenticated()
        {
            var usernames = new[] { "Steven Stevens", "John Doe", "Michael Jordan", "Martin Simons" };
            return Ok(usernames);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginInputModel input)
        {
            try
            {
                return Ok(new
                { 
                    Token = await _usersService.LoginAsync(input)
                });
            }
            catch (LoginFailedException ex)
            {
                return Unauthorized(ex.Errors);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterInputModel input)
        {
            try
            {
                await _usersService.RegisterAsync(input);
                return Ok();
            }
            catch (RegisterFailedException ex)
            {
                return Unauthorized(ex.Errors);
            }
        }
    }
}
