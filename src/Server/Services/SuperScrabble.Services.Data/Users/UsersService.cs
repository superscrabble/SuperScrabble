namespace SuperScrabble.Services.Data.Users
{
    using Microsoft.AspNetCore.Identity;

    using SuperScrabble.Common;
    using SuperScrabble.Common.Exceptions.Users;

    using SuperScrabble.Services.Common;

    using SuperScrabble.WebApi.Resources;
    using SuperScrabble.WebApi.ViewModels.Users;
    
    public class UsersService : IUsersService
    {
        private readonly IJsonWebTokenGenerator jsonWebTokenGenerator;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public UsersService(
            IJsonWebTokenGenerator jsonWebTokenGenerator,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            this.jsonWebTokenGenerator = jsonWebTokenGenerator;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<string> AuthenticateAsync(LoginInputModel input)
        {
            AppUser user = await this.GetByUserNameAsync(input.UserName);

            SignInResult result = await this.signInManager
                .CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new LoginUserFailedException();
            }

            return this.jsonWebTokenGenerator.GenerateToken(input.UserName);
        }

        public async Task CreateAsync(RegisterInputModel input)
        {
            var user = new AppUser
            {
                Email = input.Email,
                UserName = input.UserName,
                EmailConfirmed = false,
            };

            IdentityResult result = await this.userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                throw new RegisterUserFailedException(result.GetErrors());
            }
        }

        public async Task<AppUser> GetByUserNameAsync(string? userName)
        {
            AppUser user = await this.userManager.FindByNameAsync(userName);

            if (user == null)
            {
                throw new UserNotFoundException(new()
                {
                    ["UserName"] = new() { UserValidationErrorCodes.UserWithSuchNameNotFound }
                });
            }

            return user;
        }

        public async Task<AppUser> GetByEmailAsync(string? email)
        {
            AppUser user = await this.userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new UserNotFoundException(new()
                {
                    ["Email"] = new() { UserValidationErrorCodes.UserWithSuchEmailNotFound }
                });
            }

            return user;
        }

        public async Task DeleteAsync(string? userName)
        {
            AppUser user = await this.GetByUserNameAsync(userName);
            IdentityResult result = await this.userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new DeleteUserFailedException(result.GetErrors());
            }
        }
    }
}
