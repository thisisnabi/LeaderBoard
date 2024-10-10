using LeaderBoard.Models;
using MassTransit.Initializers;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.BrokerConfigure();
builder.ConfigureRedis();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ScoreService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/game", async (string player, int score, IPublishEndpoint endpoint) =>
{
    await endpoint.Publish(new PlayerScoreChangedEvent(player, score));
});

app.MapPost("/ordering", async (string catalod_id, IPublishEndpoint endpoint) =>
{
    await endpoint.Publish(new SoldProductEvent(catalod_id));
});

app.MapGet("/top/{topic}", async (int K, string topic, ScoreService scoreService) =>
{
    var items = await scoreService.GetTop<MostSoldProduct>(topic, K);
    return Results.Ok(items);
});

app.MapGet("/top/{topic}/game", async (int K, string topic, ScoreService scoreService) =>
{
    var items = await scoreService.GetTop<PlayerScore>(topic, K);
    return Results.Ok(items);
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

    public static void ConfigureRedis(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(ps =>
        {
            var connectionString = builder.Configuration.GetConnectionString("RedisConnection");
            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            return ConnectionMultiplexer.Connect(connectionString);
        });
    }
}