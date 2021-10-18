namespace SuperScrabble.Services
{
    using SuperScrabble.InputModels.Users;
    using SuperScrabble.Models;

    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<string> AuthenticateAsync(LoginInputModel input);

        Task CreateAsync(RegisterInputModel input);

        Task<AppUser> GetAsync(string userName);

        Task UpdateUserNameAsync(UpdateUserNameInputModel input);

        Task UpdatePasswordAsync(UpdatePasswordInputModel input);

        Task UpdateEmailAsync(UpdateEmailInputModel input);
        
        Task DeleteAsync(string userName);
    }
}
