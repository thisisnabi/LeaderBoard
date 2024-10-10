
namespace LeaderBoard.Models;

public class MostSoldProduct : BaseScoreType
{
    public const string RedisKey = "MostSoldProduct";

    public int Id { get; set; }
    
    [Element]
    public string CatalogId { get; set; }
}
