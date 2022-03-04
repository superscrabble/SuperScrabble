namespace SuperScrabble.WebApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using SuperScrabble.Common;
    using SuperScrabble.Common.Exceptions.Users;
    using SuperScrabble.Data.Models;
    using SuperScrabble.Services.Data.Users;
    using SuperScrabble.WebApi.ViewModels.Users;

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService usersService;

        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet("by/email")]
        public async Task<IActionResult> GetByEmail(string? email)
        {
            try
            {
                AppUser user = await this.usersService.GetByEmailAsync(email);
                return this.Ok(new GetUserViewModel { UserName = user.UserName });
            }
            catch (UserNotFoundException ex)
            {
                return this.NotFound(ex.PropertyNamesByErrorCodes);
            }
        }

        [HttpGet("by/username")]
        public async Task<IActionResult> GetByUserName(string userName)
        {
            try
            {
                AppUser user = await this.usersService.GetByUserNameAsync(userName);
                return this.Ok(new GetUserViewModel { UserName = user.UserName });
            }
            catch (UserNotFoundException ex)
            {
                return this.NotFound(ex.PropertyNamesByErrorCodes);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState.AllErrorMessages());
            }

            try
            {
                string jsonWebToken = await this.usersService.AuthenticateAsync(input);
                return this.Ok(new { Token = jsonWebToken });
            }
            catch (UserNotFoundException ex)
            {
                return this.NotFound(ex.PropertyNamesByErrorCodes);
            }
            catch (UserOperationFailedException ex)
            {
                return this.BadRequest(ex.PropertyNamesByErrorCodes);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState.AllErrorMessages());
            }

            try
            {
                await this.usersService.CreateAsync(input);

                var loginInput = new LoginInputModel
                {
                    UserName = input.UserName,
                    Password = input.Password,
                };

                string jsonWebToken = await this.usersService.AuthenticateAsync(loginInput);
                return this.Ok(new { Token = jsonWebToken });
            }
            catch (UserNotFoundException ex)
            {
                return this.NotFound(ex.PropertyNamesByErrorCodes.Values.SelectMany(d => d).ToList());
            }
            catch (UserOperationFailedException ex)
            {
                return this.BadRequest(ex.PropertyNamesByErrorCodes.Values.SelectMany(d => d).ToList());
                //return this.BadRequest(ex.PropertyNamesByErrorCodes);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            try
            {
                string userName = this.User.Identity?.Name ?? string.Empty;
                await this.usersService.DeleteAsync(userName);
                return this.Ok();
            }
            catch (UserNotFoundException ex)
            {
                return this.NotFound(ex.PropertyNamesByErrorCodes);
            }
            catch (UserOperationFailedException ex)
            {
                return this.BadRequest(ex.PropertyNamesByErrorCodes);
            }
        }
    }
}
