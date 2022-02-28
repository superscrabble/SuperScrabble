namespace SuperScrabble.Data.Models;

public class PlayedGame
{
    public PlayedGame(string id)
    {
        Id = id;
        PlayedOn = DateTime.UtcNow;
        Users = new HashSet<GameUser>();
    }

    [Key]
    public string Id { get; set; }

    public DateTime PlayedOn { get; set; }

    public virtual ICollection<GameUser> Users { get; set; }
}
