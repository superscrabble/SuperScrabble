namespace SuperScrabble.Services.Data
{
    using System.Threading.Tasks;

    using SuperScrabble.Models;
    using SuperScrabble.InputModels.Users;

    public interface IUsersService
    {
        Task<string> AuthenticateAsync(LoginInputModel input);

        Task CreateAsync(RegisterInputModel input);

        Task<AppUser> GetAsync(string userName);

        Task<string> UpdateUserNameAsync(UpdateUserNameInputModel input, string oldUserName);

        Task UpdatePasswordAsync(UpdatePasswordInputModel input, string userName);

        Task UpdateEmailAsync(UpdateEmailInputModel input, string userName);
        
        Task DeleteAsync(string userName);
    }
}
