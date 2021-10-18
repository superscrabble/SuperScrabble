﻿namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Models;
    using SuperScrabble.Services;
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
                AppUser result = await _usersService.GetAsync(userName);
                return Ok(result);
            }
            catch(GetUserFailedException ex)
            {
                return BadRequest(ex.Errors);
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
                return Ok(new
                {
                    Token = await _usersService.AuthenticateAsync(input)
                });
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
                return Ok();
            }
            catch (RegisterUserFailedException ex)
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
            catch(UserOperationFailedException ex)
                when (ex is GetUserFailedException 
                      || ex is DeleteUserFailedException)
            {
                return BadRequest(ex.Errors);
            }
        }
    }
}
