namespace SuperScrabble.Services.Data
{
    using System.Threading.Tasks;

    using SuperScrabble.InputModels.Game;
    using SuperScrabble.ViewModels;

    public interface IGamesService
    {
        Task SaveGameAsync(SaveGameInputModel input);

        Task<EndGameSummaryViewModel> GetSummaryById(string id, string userName);
    }
}
