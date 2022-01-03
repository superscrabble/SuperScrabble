namespace SuperScrabble.Services.Data.Users
{
    using SuperScrabble.WebApi.ViewModels.Users;

    public interface IUsersService
    {
        Task<string> AuthenticateAsync(LoginInputModel input);

        Task CreateAsync(RegisterInputModel input);

        Task<AppUser> GetAsync(string userName);

        Task DeleteAsync(string userName);
    }
}
