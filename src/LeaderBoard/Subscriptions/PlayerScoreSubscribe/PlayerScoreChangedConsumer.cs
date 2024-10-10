using LeaderBoard.Models;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscribe;

public class PlayerScoreChangedConsumer(ScoreService scoreService)
    : IConsumer<PlayerScoreChangedEvent>
{
    private readonly ScoreService _scoreService = scoreService;

    public Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
    {
        var message = context.Message;
        return _scoreService.Add(PlayerScore.RedisKey, message.PlayerUsername, message.Score);
    }
}
