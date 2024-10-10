namespace LeaderBoard.Subscriptions.PlayerScoreSubscribe;

public record PlayerScoreChangedEvent(string PlayerUsername, int Score);
