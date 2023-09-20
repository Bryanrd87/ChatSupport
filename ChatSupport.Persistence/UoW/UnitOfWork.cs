using ChatSupport.Domain.UoW;
using ChatSupport.Domain;
using ChatSupport.Domain.Repositories;

namespace ChatSupport.Persistence.UoW;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IRepository<User> userRepository,
        IRepository<Agent> agentRepository,
        IRepository<ChatSession> chatSessionRepository,
        IRepository<ChatMessage> chatMessageRepository)
    {
        _context = context;
        UserRepository = userRepository;
        AgentRepository = agentRepository;
        ChatSessionRepository = chatSessionRepository;
        ChatMessageRepository = chatMessageRepository;
    }

    public IRepository<User> UserRepository { get; }
    public IRepository<Agent> AgentRepository { get; }
    public IRepository<ChatSession> ChatSessionRepository { get; }
    public IRepository<ChatMessage> ChatMessageRepository { get; }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

