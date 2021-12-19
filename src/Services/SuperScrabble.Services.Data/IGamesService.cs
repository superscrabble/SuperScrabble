namespace SuperScrabble.Services.Data
{
    using System.Threading.Tasks;

    using SuperScrabble.ViewModels;
    using SuperScrabble.InputModels.Game;

    public interface IGamesService
    {
        Task SaveGameAsync(SaveGameInputModel input);

        GameSummaryViewModel GetSummaryById(string id, string userName);
    }
}
