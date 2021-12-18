namespace SuperScrabble.Services.Data
{
    using System.Threading.Tasks;

    using SuperScrabble.InputModels.Game;

    public interface IGamesService
    {
        Task SaveGameAsync(SaveGameInputModel input);
    }
}
