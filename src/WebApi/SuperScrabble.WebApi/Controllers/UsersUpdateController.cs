namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Services;
    using SuperScrabble.WebApi.Extensions;
    using SuperScrabble.InputModels.Users;
    using SuperScrabble.CustomExceptions.Users;

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/users/update")]
    public class UsersUpdateController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersUpdateController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPut("username")]
        [Authorize]
        public async Task<ActionResult> UpdateUserName([FromBody] UpdateUserNameInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<UpdateUserNameInputModel>());
            }

            try
            {
                string token = await _usersService.UpdateUserNameAsync(input, User.Identity.Name);
                return Ok(new { Token = token });
            }
            catch (UserOperationFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<UpdatePasswordInputModel>());
            }

            try
            {
                await _usersService.UpdatePasswordAsync(input, User.Identity.Name);
                return Ok();
            }
            catch (UpdateUserFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("email")]
        [Authorize]
        public async Task<ActionResult> UpdateEmail([FromBody] UpdateEmailInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<UpdateEmailInputModel>());
            }

            try
            {
                await _usersService.UpdateEmailAsync(input, User.Identity.Name);
                return Ok();
            }
            catch (UpdateUserFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }
    }
}
