namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.InputModels;
    using SuperScrabble.Services;
    using SuperScrabble.CustomExceptions;
    using SuperScrabble.WebApi.Extensions;

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SuperScrabble.Models;

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string userName)
        {
            try
            {
                AppUser result = await _usersService.GetAsync(userName);
                return Ok(result);
            }
            catch(GetUserFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Errors());
            }

            try
            {
                return Ok(new
                {
                    Token = await _usersService.AuthenticateAsync(input)
                });
            }
            catch (LoginFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Errors());
            }

            try
            {
                await _usersService.CreateAsync(input);
                return Ok();
            }
            catch (RegisterFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(string userName)
        {
            try
            {
                await _usersService.DeleteAsync(userName);
                return Ok();
            }
            catch(ModelStateFailedException ex)
                when (ex is GetUserFailedException 
                      || ex is DeleteUserFailedException)
            {
                return BadRequest(ex.Errors);
            }
        }
    }
}
