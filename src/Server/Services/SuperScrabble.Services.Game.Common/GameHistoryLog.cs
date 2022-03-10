using SuperScrabble.Services.Game.Common.Enums;

namespace SuperScrabble.Services.Game.Common;

public class GameHistoryLog
{
    public HistoryLogStatus Status { get; set; }

    public string UserName { get; set; } = "";

    public int ChangedTilesCount { get; set; }

    public IEnumerable<string> NewlyWrittenWords { get; set; } = new List<string>();
}
