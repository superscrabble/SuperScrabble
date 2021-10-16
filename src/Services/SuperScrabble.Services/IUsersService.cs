namespace SuperScrabble.Services
{
    using SuperScrabble.InputModels;
    using System.Threading.Tasks;

    public interface IUsersService
    {
        Task<string> LoginAsync(LoginInputModel input);

        Task RegisterAsync(RegisterInputModel input);
    }
}
