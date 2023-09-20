using ChatSupport.Domain.Repositories;

namespace ChatSupport.Domain.UoW;
public interface IUnitOfWork : IDisposable
{
    IRepository<User> UserRepository { get; }
    IRepository<Agent> AgentRepository { get; }
    IRepository<ChatSession> ChatSessionRepository { get; }
    IRepository<ChatMessage> ChatMessageRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken);
}

