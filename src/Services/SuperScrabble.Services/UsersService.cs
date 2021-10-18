namespace SuperScrabble.Services
{
    using SuperScrabble.Common;
    using SuperScrabble.CustomExceptions;
    using SuperScrabble.Data;
    using SuperScrabble.InputModels;
    using SuperScrabble.LanguageResources;
    using SuperScrabble.Models;
    using SuperScrabble.ViewModels;

    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;

    public class UsersService : IUsersService
    {
        private static readonly Dictionary<string, Func<ModelStateErrorViewModel>> ErrorCodesAndViewModels = new()
        {
            ["DuplicateUserName"] = () => new()
            {
                PropertyName = nameof(RegisterInputModel.UserName),
                ErrorMessages = new[] { Resource.UserNameAlreadyExists }
            },

            ["DuplicateEmail"] = () => new()
            {
                PropertyName = nameof(RegisterInputModel.Email),
                ErrorMessages = new[] { Resource.EmailAddressAlreadyExists }
            }
        };

        private readonly AppDbContext _dbContext;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public UsersService(AppDbContext dbContext, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<string> AuthenticateAsync(LoginInputModel input)
        {
            AppUser user = _dbContext.Users.FirstOrDefault(user => user.UserName == input.UserName);
            var errors = new List<ModelStateErrorViewModel>();

            if (user == null)
            {
                var errorViewModel = new ModelStateErrorViewModel
                {
                    PropertyName = nameof(LoginInputModel.UserName),
                    ErrorMessages = new[] { Resource.UserNameDoesNotExist }
                };

                errors.Add(errorViewModel);
                throw new LoginFailedException(errors);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                var errorViewModel = new ModelStateErrorViewModel
                {
                    PropertyName = nameof(LoginInputModel.Password),
                    ErrorMessages = new[] { Resource.PasswordIsInvalid }
                };

                errors.Add(errorViewModel);
                throw new LoginFailedException(errors);
            }

            return GenerateToken(input.UserName);
        }

        public async Task CreateAsync(RegisterInputModel input)
        {
            var user = new AppUser
            {
                Email = input.Email,
                UserName = input.UserName,
                EmailConfirmed = false,
            };

            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                var errors = new List<ModelStateErrorViewModel>();

                foreach (var error in result.Errors)
                {
                    if (ErrorCodesAndViewModels.ContainsKey(error.Code))
                    {
                        errors.Add(ErrorCodesAndViewModels[error.Code].Invoke());
                    }
                }

                throw new RegisterFailedException(errors);
            }
        }

        private static string GenerateToken(string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(GlobalConstants.EncryptionKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userName)
                }),

                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(
                                            new SymmetricSecurityKey(keyBytes),
                                            SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Task<AppUser> GetAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UpdateInputModel input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
