using ChatSupport.Application.Models;

namespace ChatSupport.Application.Contracts.RabbitMq;
public interface IRabbitMQService
{
    event Func<ChatSessionDto, CancellationToken, Task> OnMessageReceived;
    void PublishChatSession(ChatSessionDto session);
    void ConsumeChatSessionQueue(CancellationToken cancellationToken);
}
