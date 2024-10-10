 var builder = WebApplication.CreateBuilder(args);

builder.BrokerConfigure();
builder.ConfigureApplicationDbContext();
builder.Services.AddSingleton<SortedInMemoryDatabase>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();


app.MapGet("/{topic}", (string Topic, int K , LeaderBoardDbContext dbContext) =>
{
    if (Topic == "order")
    {
        var items = dbContext.MostSoldProducts.OrderByDescending(d => d.Score)
                                              .Take(K);
        return Results.Ok(items);
    }
    else if (Topic == "game")
    {
        var items = dbContext.PlayerScores.OrderByDescending(d => d.Score)
                                      .Take(K);
        return Results.Ok(items);
    }

    throw new InvalidOperationException();
});

app.MapGet("/{topic}/sorted-set", (string Topic, int K, SortedInMemoryDatabase sortedDatabase) =>
{
    if (Topic == "order")
    {
       
        return Results.Ok(sortedDatabase.MostSoldProducts);
    }
    else if (Topic == "game")
    {
        return Results.Ok(sortedDatabase.PlayerScores);
    }

    throw new InvalidOperationException();
});

app.MapPost("/game", async (string player, int score, IPublishEndpoint endpoint) =>
{
    await endpoint.Publish(new PlayerScoreChangedEvent(player, score));
});

app.MapPost("/ordering", async (string catalod_id, IPublishEndpoint endpoint) =>
{
    await endpoint.Publish(new SoldProductEvent(catalod_id));
});

app.Run();

public static class WebApplicationExtensions
{
    public static void BrokerConfigure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(configure =>
        {
            var brokerConfig = builder.Configuration.GetSection(BrokerOptions.SectionName)
                                                    .Get<BrokerOptions>();
            if (brokerConfig is null)
            {
                throw new ArgumentNullException(nameof(BrokerOptions));
            }

            configure.AddConsumers(Assembly.GetExecutingAssembly());
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(brokerConfig.Host, hostConfigure =>
                {
                    hostConfigure.Username(brokerConfig.Username);
                    hostConfigure.Password(brokerConfig.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static void ConfigureApplicationDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<LeaderBoardDbContext>(configure =>
        {
            configure.UseInMemoryDatabase(nameof(LeaderBoardDbContext));
        });
    }
}