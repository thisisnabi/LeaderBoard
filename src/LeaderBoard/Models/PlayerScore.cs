namespace LeaderBoard.Models;

public class PlayerScore : BaseScoreType
{
    public const string RedisKey = "PlayerScore";

    [Element]
    public string Username { get; set; } = null!;
}
 