namespace SuperScrabble.Services.Data.Users
{
    using SuperScrabble.WebApi.ViewModels.Users;

    public interface IUsersService
    {
        Task<string> AuthenticateAsync(LoginInputModel input);

        Task CreateAsync(RegisterInputModel input);

        Task<AppUser> GetByUserNameAsync(string? userName);

        Task<AppUser> GetByEmailAsync(string? email);

        Task DeleteAsync(string? userName);
    }
}
