namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Common;
    using SuperScrabble.Services;
    using SuperScrabble.WebApi.Extensions;
    using SuperScrabble.InputModels.Users;
    using SuperScrabble.CustomExceptions.Users;

    using System;
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

            if (!User.Identity.Name.Equals(input.OldUserName) && !User.IsInRole(GlobalConstants.Roles.Admin))
            {
                return Unauthorized();
            }

            try
            {
                await _usersService.UpdateUserNameAsync(input);
                return Ok();
            }
            catch (UserOperationFailedException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPut("password")]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<UpdatePasswordInputModel>());
            }

            throw new NotImplementedException(nameof(UpdatePassword));
        }

        [HttpPut("email")]
        public async Task<ActionResult> UpdateEmail([FromBody] UpdateEmailInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<UpdateEmailInputModel>());
            }

            throw new NotImplementedException(nameof(UpdateEmail));
        }
    }
}
