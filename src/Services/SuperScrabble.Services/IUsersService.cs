namespace SuperScrabble.Services
{
    using SuperScrabble.InputModels;
    using SuperScrabble.Models;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<string> AuthenticateAsync(LoginInputModel input);

        Task CreateAsync(RegisterInputModel input);

        Task<AppUser> GetAsync(string userName);

        Task UpdateAsync(UpdateInputModel input);
        
        Task DeleteAsync(string userName);
    }
}
