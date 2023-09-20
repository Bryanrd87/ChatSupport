using ChatSupport.Application.Contracts.Sessions;
using Microsoft.Extensions.Hosting;

namespace ChatSupport.Application.Service;
public class ChatSessionMonitorHostedService : IHostedService
{
    private readonly IChatSessionMonitor _chatSessionMonitor;
    private Timer _timer;
    private CancellationToken _cancellationToken;
    public ChatSessionMonitorHostedService(IChatSessionMonitor chatSessionMonitor)
    {
        _chatSessionMonitor = chatSessionMonitor;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }
    private async void DoWork(object state)
    {
        await _chatSessionMonitor.InactivateSessionsAsync(_cancellationToken);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}