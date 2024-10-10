namespace LeaderBoard;

public class LeaderBoardOptions
{
    public required BrokerOptions BrokerOptions { get; set; }
}

public sealed class BrokerOptions
{
    public const string SectionName = "BrokerOptions";

    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}