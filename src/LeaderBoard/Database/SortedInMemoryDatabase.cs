using LeaderBoard.Models;

namespace LeaderBoard.Database;

public class SortedInMemoryDatabase
{
    private SortedSet<PlayerScore> playerScores = new SortedSet<PlayerScore>(new BaseScoreTypeComparer());
    private SortedSet<MostSoldProduct> mostSoldProducts = new SortedSet<MostSoldProduct>(new BaseScoreTypeComparer());

    public SortedSet<PlayerScore> PlayerScores => playerScores;
    public SortedSet<MostSoldProduct> MostSoldProducts => mostSoldProducts;

    public SortedInMemoryDatabase()
    {
  
    }

    public void AddItem<TModel>(TModel model) where TModel : BaseScoreType
    { 
        if (model is PlayerScore player)
        {
            playerScores.Add(player);
        }
        else if (model is MostSoldProduct catalog)
        {
            mostSoldProducts.Add(catalog);
        }
    }

    public void Update<TModel>(TModel model) where TModel : BaseScoreType
    {
         // for contribute
    }

}
