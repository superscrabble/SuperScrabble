namespace SuperScrabble.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using SuperScrabble.Models;
    using SuperScrabble.InputModels.Game;
    using SuperScrabble.Data.Repositories;
    using SuperScrabble.Services.Game.Models;
    using SuperScrabble.ViewModels;
    using System;
    using SuperScrabble.LanguageResources;

    public class GamesService : IGamesService
    {
        private readonly IUsersService usersService;
        private readonly IRepository<Game> gamesRepository;
        private readonly IRepository<UserGame> usersGamesRepository;

        public GamesService(IUsersService usersService, IRepository<Game> gamesRepository, IRepository<UserGame> usersGamesRepository)
        {
            this.usersService = usersService;
            this.gamesRepository = gamesRepository;
            this.usersGamesRepository = usersGamesRepository;
        }

        public async Task SaveGameAsync(SaveGameInputModel input)
        {
            var sortedPlayersByPoints = input.Players.OrderByDescending(p => p.Points);

            int maxPoints = sortedPlayersByPoints.First().Points;

            var gameOutcomesByUserNames = new Dictionary<string, GameOutcome>();

            if (!HasWinner(sortedPlayersByPoints))
            {
                foreach (Player tiebreaker in GetAllTiebreakers(sortedPlayersByPoints, maxPoints))
                {
                    gameOutcomesByUserNames.Add(tiebreaker.UserName, GameOutcome.Draw);
                }
            }
            else
            {
                gameOutcomesByUserNames.Add(sortedPlayersByPoints.First().UserName, GameOutcome.Win);
            }

            foreach (Player loser in GetAllLosers(sortedPlayersByPoints, maxPoints))
            {
                gameOutcomesByUserNames.Add(loser.UserName, GameOutcome.Loss);
            }

            var game = new Game(input.GameId);

            foreach (Player player in sortedPlayersByPoints)
            {
                AppUser user = await this.usersService.GetAsync(player.UserName);

                game.Users.Add(new UserGame
                {
                    UserId = user.Id,
                    GameId = game.Id,
                    Points = player.Points,
                    GameOutcome = gameOutcomesByUserNames[player.UserName],
                });
            }

            await this.gamesRepository.AddAsync(game);
            await this.gamesRepository.SaveChangesAsync();
        }

        private static bool HasWinner(IOrderedEnumerable<Player> sortedPlayersByPoints)
        {
            Player first = sortedPlayersByPoints.First();
            Player second = sortedPlayersByPoints.Skip(1).First();

            return first.Points != second.Points;
        }

        private static IEnumerable<Player> GetAllTiebreakers(IOrderedEnumerable<Player> sortedPlayersByPoints, int maxPoints)
        {
            return sortedPlayersByPoints.Where(p => p.Points == maxPoints);
        }

        private static IEnumerable<Player> GetAllLosers(IOrderedEnumerable<Player> sortedPlayersByPoints, int maxPoints)
        {
            return sortedPlayersByPoints.Where(p => p.Points < maxPoints);
        }

        public EndGameSummaryViewModel GetSummaryById(string id, string userName)
        {
            Game game = this.gamesRepository.All().FirstOrDefault(x => x.Id == id);

            if (game == null)
            {
                return null;
            }

            EndGameSummaryViewModel result = new()
            {
                PointsByUserNames = game.Users
                    .ToList()
                    .OrderByDescending(x => x.Points)
                    .Select(x => new KeyValuePair<string, int>(x.User.UserName, x.Points))
            };

            UserGame userGame = game.Users.FirstOrDefault(u => u.User.UserName == userName);

            var gameOutcomeMessages = new Dictionary<GameOutcome, string>
            {
                [GameOutcome.Loss] = Resource.LossGameOutcome,
                [GameOutcome.Win] = Resource.WinGameOutcome,
                [GameOutcome.Draw] = Resource.DrawGameOutcome,
            };

            result.GameOutcome = gameOutcomeMessages[userGame.GameOutcome];
            return result;
        }
    }
}
