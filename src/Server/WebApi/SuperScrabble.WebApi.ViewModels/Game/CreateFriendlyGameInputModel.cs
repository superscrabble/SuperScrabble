using SuperScrabble.Services.Game.Common.Enums;

namespace SuperScrabble.WebApi.ViewModels.Game
{
    public class CreateFriendlyGameInputModel
    {
        public TimerType TimerType { get; set; }

        public TimerDifficulty TimerDifficulty { get; set; }
    }
}
