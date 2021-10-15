namespace SuperScrabble.WebApi.Controllers
{
    using SuperScrabble.Common;
    using SuperScrabble.InputModels;
    using SuperScrabble.Models;
    using SuperScrabble.Data;
    using SuperScrabble.WebApi.Utillities;

    using System;
    using System.Text;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.Identity;

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UsersController(AppDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
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
            AppUser user = _dbContext.Users.FirstOrDefault(user => user.UserName == input.UserName);

            if (user == null)
            {
                return Unauthorized(ApiMessages.InvalidLoginMessage);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                return Unauthorized(ApiMessages.InvalidLoginMessage);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(GlobalConstants.EncryptionKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, input.UserName)
                }),

                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(
                                            new SymmetricSecurityKey(keyBytes), 
                                            SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterInputModel input)
        {
            var user = new AppUser
            {
                Email = input.Email,
                UserName = input.UserName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                var errorsBuilder = new StringBuilder();

                foreach (var error in result.Errors)
                {
                    errorsBuilder.AppendLine(error.Description);
                }

                return BadRequest(errorsBuilder.ToString());
            }

            return Ok(new { Result = ApiMessages.RegistrationSucceeded });
        }
    }
}
