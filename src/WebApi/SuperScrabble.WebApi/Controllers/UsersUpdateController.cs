namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.InputModels.Users;
    using SuperScrabble.Services;

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
            throw new NotImplementedException(nameof(UpdateUserName));
        }

        [HttpPut("password")]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordInputModel input)
        {
            throw new NotImplementedException(nameof(UpdatePassword));
        }

        [HttpPut("email")]
        public async Task<AcceptedAtActionResult> UpdateEmail([FromBody] UpdateEmailInputModel input)
        {
            throw new NotImplementedException(nameof(UpdateEmail));
        }
    }
}
