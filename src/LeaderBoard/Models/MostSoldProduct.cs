namespace LeaderBoard.Models;

public class MostSoldProduct : BaseScoreType
{
    public int Id { get; set; }
    public string CatalogId { get; set; } = null!;
}
