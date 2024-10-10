using LeaderBoard.Database;
using LeaderBoard.Models;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscribe;

public class PlayerScoreChangedConsumer(
    SortedInMemoryDatabase sortedDatabase,
    LeaderBoardDbContext context) 
    : IConsumer<PlayerScoreChangedEvent>
{
    private readonly LeaderBoardDbContext _context = context;
    private readonly SortedInMemoryDatabase _SortedDatabse = sortedDatabase;

    public async Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
    {
        var message = context.Message;
        var item = await _context.PlayerScores
                                  .FirstOrDefaultAsync(f => f.Username == message.PlayerUsername);
        if (item is not null)
        {
            item.Score = message.Score;
        }
        else
        {
            var newItem = new PlayerScore
            {
                Score = message.Score,
                Username = message.PlayerUsername,
            };

            _context.PlayerScores.Add(newItem);
            _SortedDatabse.AddItem(newItem);
        }


        await _context.SaveChangesAsync(context.CancellationToken);
    }
}
