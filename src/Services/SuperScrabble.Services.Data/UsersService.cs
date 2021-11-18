namespace SuperScrabble.Services.Data
{
    using System;
    using System.Text;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.EntityFrameworkCore;

    using SuperScrabble.Data;
    using SuperScrabble.Common;
    using SuperScrabble.Models;
    using SuperScrabble.Utilities;
    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Users;
    using SuperScrabble.LanguageResources;
    using SuperScrabble.CustomExceptions.Users;

    public class UsersService : IUsersService
    {
        private static readonly Dictionary<string, Func<ModelStateErrorViewModel>> ErrorCodesAndViewModels = new()
        {
            [IdentityErrorCodes.DuplicateUserName] = () => new()
            {
                PropertyName = nameof(RegisterInputModel.UserName),
                DisplayName = Resource.UserNameDisplayName,
                ErrorMessages = new[] { Resource.UserNameAlreadyExists }
            },

            [IdentityErrorCodes.DuplicateEmail] = () => new()
            {
                PropertyName = nameof(RegisterInputModel.Email),
                DisplayName = Resource.EmailAddressDisplayName,
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
            AppUser user = await WrapGetAsync<LoginUserFailedException>(input.UserName);

            var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                var errors = new[]
                {
                    new ModelStateErrorViewModel
                    {
                        PropertyName = nameof(LoginInputModel.Password),
                        DisplayName = AttributesGetter.DisplayName<LoginInputModel>(nameof(LoginInputModel.Password)),
                        ErrorMessages = new[] { Resource.PasswordIsInvalid }
                    }
                };

                throw new LoginUserFailedException { Errors = errors };
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

            IdentityResult result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                HandleErrors<RegisterUserFailedException>(result);
            }
        }

        public async Task<AppUser> GetAsync(string userName)
        {
            AppUser user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null)
            {
                var errors = new[]
                {
                    new ModelStateErrorViewModel
                    {
                        PropertyName = nameof(LoginInputModel.UserName),
                        DisplayName = AttributesGetter.DisplayName<LoginInputModel>(nameof(LoginInputModel.UserName)),
                        ErrorMessages = new[] { Resource.UserNameDoesNotExist }
                    }
                };

                throw new GetUserFailedException() { Errors = errors };
            }

            return user;
        }

        public async Task<string> UpdateUserNameAsync(UpdateUserNameInputModel input, string oldUserName)
        {
            AppUser user = await WrapGetAsync<UpdateUserFailedException>(oldUserName);

            IdentityResult result = await _userManager.SetUserNameAsync(user, input.NewUserName);

            if (!result.Succeeded)
            {
                HandleErrors<UpdateUserFailedException>(result);
            }

            return GenerateToken(input.NewUserName);
        }

        public async Task UpdatePasswordAsync(UpdatePasswordInputModel input, string userName)
        {
            AppUser user = await WrapGetAsync<UpdateUserFailedException>(userName);

            IdentityResult result = await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);

            if (!result.Succeeded)
            {
                HandleErrors<UpdateUserFailedException>(result);
            }
        }

        public Task UpdateEmailAsync(UpdateEmailInputModel input, string userName)
        {
            throw new NotImplementedException(nameof(UpdateEmailAsync));
        }

        public async Task DeleteAsync(string userName)
        {
            AppUser user = await WrapGetAsync<DeleteUserFailedException>(userName);

            IdentityResult result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                HandleErrors<DeleteUserFailedException>(result);
            }
        }

        private async Task<AppUser> WrapGetAsync<TThrownException>(string userName) 
            where TThrownException : UserOperationFailedException, new()
        {
            try
            {
                return await GetAsync(userName);
            }
            catch (GetUserFailedException ex)
            {
                throw new TThrownException() { Errors = ex.Errors };
            }
        }

        private void HandleErrors<TException>(IdentityResult identityResult) 
            where TException : UserOperationFailedException, new()
        {
            var errors = new List<ModelStateErrorViewModel>();

            foreach (IdentityError error in identityResult.Errors)
            {
                if (ErrorCodesAndViewModels.ContainsKey(error.Code))
                {
                    ModelStateErrorViewModel errorViewModel = ErrorCodesAndViewModels[error.Code].Invoke();
                    errors.Add(errorViewModel);
                }
                else
                {
                    LogError(error);
                }
            }

            throw new TException() { Errors = errors };
        }

        private void LogError(IdentityError error)
        {
            throw new NotImplementedException(nameof(LogError));
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
    }
}
