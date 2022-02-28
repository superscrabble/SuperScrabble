using SuperScrabble.WebApi.ViewModels.Games;

namespace SuperScrabble.Services.Data.Games;

public interface IGamesService
{
    Task SaveGameAsync(SaveGameInputModel input);

    GameSummaryViewModel GetSummaryById(string id, string userName);
}
