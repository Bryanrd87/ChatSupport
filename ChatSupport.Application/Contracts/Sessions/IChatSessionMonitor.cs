using ChatSupport.Application.Models;

namespace ChatSupport.Application.Contracts.Sessions;
public interface IChatSessionMonitor
{
    Task InactivateSessionsAsync(CancellationToken cancellationToken);
    Task MonitorChatSessionsAsync(ChatSessionDto chatSessionRequest, CancellationToken cancellationToken);
}
