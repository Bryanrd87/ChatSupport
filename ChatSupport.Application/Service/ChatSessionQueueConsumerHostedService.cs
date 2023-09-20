using ChatSupport.Application.Contracts.RabbitMq;
using Microsoft.Extensions.Hosting;

namespace ChatSupport.Application.Service;
public class ChatSessionQueueConsumerHostedService : IHostedService, IDisposable
{
    private readonly IRabbitMQService _rabbitMQService;
    private Timer _timer;
    private CancellationToken _cancellationToken;

    public ChatSessionQueueConsumerHostedService(IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        
        _timer = new Timer(ConsumeQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

        return Task.CompletedTask;
    }

    private void ConsumeQueue(object state)
    {
        _rabbitMQService.ConsumeChatSessionQueue(_cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

