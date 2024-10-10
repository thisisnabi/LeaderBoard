namespace LeaderBoard.Models;

public class PlayerScore : BaseScoreType
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
}
 