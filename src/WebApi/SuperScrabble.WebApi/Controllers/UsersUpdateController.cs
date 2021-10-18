namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Services;
    using SuperScrabble.WebApi.Extensions;
    using SuperScrabble.InputModels.Users;

    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> UpdateUserName([FromBody] UpdateUserNameInputModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors<UpdateUserNameInputModel>());
            }

            throw new NotImplementedException(nameof(UpdateUserName));
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
