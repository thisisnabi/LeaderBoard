using LeaderBoard.Models;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscribe;

public class SoldProductConsumer(ScoreService scoreService) : IConsumer<SoldProductEvent>
{
    private readonly ScoreService _scoreService = scoreService;
    public Task Consume(ConsumeContext<SoldProductEvent> context)
    {
        var message = context.Message;
        return _scoreService.Increment(MostSoldProduct.RedisKey, message.slug);
    }
}
