namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Services;
    using SuperScrabble.WebApi.Extensions;
    using SuperScrabble.InputModels.Users;

    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using SuperScrabble.CustomExceptions.Users;
    using SuperScrabble.ViewModels;
    using System.Collections.Generic;
    using SuperScrabble.Utilities;
    using SuperScrabble.LanguageResources;

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

            if(this.User.Identity.Name.Equals(input.OldUserName)
                || this.User.IsInRole("Admin")) //TODO: put this into a global variable
            {
                try
                {
                    await this._usersService.UpdateUserNameAsync(input);
                    return Ok(); //TODO: decide whether to return the new user
                }
                catch (UserOperationFailedException ex)
                    when (ex is UpdateUserFailedException 
                          || ex is GetUserFailedException)
                {
                    return BadRequest(ex.Errors);
                }
            }
            else
            {
                return Unauthorized();
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
