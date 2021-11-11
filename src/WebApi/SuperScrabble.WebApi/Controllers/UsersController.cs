namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Models;
    using SuperScrabble.Services;
    using SuperScrabble.ViewModels;
    using SuperScrabble.WebApi.Extensions;
    using SuperScrabble.InputModels.Users;
    using SuperScrabble.CustomExceptions.Users;

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

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
                AppUser user = await _usersService.GetAsync(userName);
                var userViewModel = new UserViewModel { UserName = user.UserName };
                return Ok(userViewModel);
            }
            catch (GetUserFailedException ex)
            {
                return NotFound(ex.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<LoginInputModel>());
            }

            try
            {
                var token = new { Token = await _usersService.AuthenticateAsync(input) };
                return Ok(token);
            }
            catch (LoginUserFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<RegisterInputModel>());
            }

            try
            {
                await _usersService.CreateAsync(input);
                LoginInputModel loginModel = new()
                {
                    UserName = input.UserName,
                    Password = input.Password
                };
                var token = new { Token = await _usersService.AuthenticateAsync(loginModel) };
                return Ok(token);
            }
            catch (UserOperationFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            try
            {
                await _usersService.DeleteAsync(User.Identity.Name);
                return Ok();
            }
            catch (UserOperationFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }
    }
}
