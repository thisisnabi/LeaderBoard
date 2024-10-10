using LeaderBoard.Models;
using StackExchange.Redis;

namespace LeaderBoard;

public class ScoreService(IConnectionMultiplexer connectionMultiplexer)
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public Task<bool> Add(string topic, string memberId, int Score)
    {
        return _database.SortedSetAddAsync(topic, memberId, Score);
    }

    public Task<double> Increment(string topic, string memberId)
    {
        return _database.SortedSetIncrementAsync(topic, memberId, 1);
    }

    public async Task<IEnumerable<T>> GetTop<T>(string topic, int k) where T : BaseScoreType
    {
        var items = await _database.SortedSetRangeByRankWithScoresAsync(topic, 0, k - 1, Order.Descending);

        var models = new List<T>();

        foreach (var item in items)
        {
            models.Add(item.ToModel<T>());
        }

        return models;
    }
}


public static class SortedSetEntryExtensions
{
    public static T ToModel<T>(this SortedSetEntry entry) where T : BaseScoreType
    {
        var model = Activator.CreateInstance<T>();
        model.Score = Convert.ToInt32(entry.Score);

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(ElementAttribute));

            if (attribute is not null && property.CanWrite)
                property.SetValue(model, entry.Element.ToString());
        }

        return model;
    }
}
