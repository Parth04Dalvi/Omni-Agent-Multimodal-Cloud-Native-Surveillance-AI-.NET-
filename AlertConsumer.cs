
using StackExchange.Redis;
using Microsoft.AspNetCore.SignalR;

public class AlertConsumer : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AlertConsumer(IConnectionMultiplexer redis, IHubContext<NotificationHub> hubContext)
    {
        _redis = redis;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();
        var sub = _redis.GetSubscriber();

        [cite_start]// Listen for AI-generated alerts from the Python service [cite: 31]
        await sub.SubscribeAsync("alerts", async (channel, message) =>
        {
            [cite_start]// Push real-time notification to the Next.js dashboard [cite: 31, 40]
            await _hubContext.Clients.All.SendAsync("ReceiveAlert", message.ToString());
            
            [cite_start]// Log to PostgreSQL for later Data Analysis [cite: 22, 28]
            await SaveAlertToDatabase(message.ToString());
        });
    }
}
