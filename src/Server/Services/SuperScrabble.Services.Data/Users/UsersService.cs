namespace SuperScrabble.Services.Data.Users
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using SuperScrabble.Common.Exceptions.Users;
    using SuperScrabble.Services.Common;
    using SuperScrabble.WebApi.Resources;
    using SuperScrabble.WebApi.ViewModels.Users;
    
    public class UsersService : IUsersService
    {
        private readonly IJsonWebTokenGenerator jsonWebTokenGenerator;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IRepository<AppUser> usersRepository;

        public UsersService(
            IJsonWebTokenGenerator jsonWebTokenGenerator,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IRepository<AppUser> usersRepository)
        {
            this.jsonWebTokenGenerator = jsonWebTokenGenerator;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.usersRepository = usersRepository;
        }

        public async Task<string> AuthenticateAsync(LoginInputModel input)
        {
            AppUser user = await this.GetAsync(input.UserName);

            SignInResult result = await this.signInManager
                .CheckPasswordSignInAsync(user, input.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                var errorCodes = new List<string>();
                throw new LoginUserFailedException(errorCodes);
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
                throw new RegisterUserFailedException(
                    result.Errors.Select(err => err.Code));
            }
        }

        public async Task<AppUser> GetAsync(string userName)
        {
            AppUser? user = await this.usersRepository
                .All().FirstOrDefaultAsync(user => user.UserName == userName);

            if (user == null)
            {
                throw new UserNotFoundException(new[]
                {
                    UserValidationErrorCodes.UserWithSuchNameNotFound
                });
            }

            return user;
        }

        public async Task DeleteAsync(string userName)
        {
            AppUser user = await this.GetAsync(userName);
            IdentityResult result = await this.userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new DeleteUserFailedException(
                    result.Errors.Select(err => err.Code));
            }
        }
    }
}
