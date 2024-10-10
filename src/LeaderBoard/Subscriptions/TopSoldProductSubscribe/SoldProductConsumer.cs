using LeaderBoard.Database;
using LeaderBoard.Models;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscribe;

public class SoldProductConsumer(
    SortedInMemoryDatabase sortedDatabase,
    LeaderBoardDbContext context) : IConsumer<SoldProductEvent>
{
    private readonly LeaderBoardDbContext _context = context;
    private readonly SortedInMemoryDatabase _SortedDatabse = sortedDatabase;
    public async Task Consume(ConsumeContext<SoldProductEvent> context)
    {
        var message = context.Message;
        var item = await _context.MostSoldProducts
                                  .FirstOrDefaultAsync(f => f.CatalogId == message.slug);
        if (item is not null)
        {
            item.Score++;
        }
        else
        {
            var newItem = new MostSoldProduct
            {
                CatalogId = message.slug,
                Score = 1
            };
            _context.MostSoldProducts.Add(newItem);
            _SortedDatabse.AddItem(newItem);
        }
    
        await _context.SaveChangesAsync(context.CancellationToken);
    }
}
