using ChatSupport.Application.Contracts.Agents;
using ChatSupport.Application.Contracts.RabbitMq;
using ChatSupport.Application.Contracts.Sessions;
using ChatSupport.Application.Models;
using ChatSupport.Domain.UoW;

namespace ChatSupport.Application.Features.Sessions;
public class ChatSessionMonitor : IChatSessionMonitor
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IAgentChatCoordinator _agentChatCoordinator;
    private readonly IRabbitMQService _rabbitMQService;    

    public ChatSessionMonitor(IUnitOfWorkFactory unitOfWorkFactory, IAgentChatCoordinator agentChatCoordinator, IRabbitMQService rabbitMQService)
    {   
        _unitOfWorkFactory = unitOfWorkFactory;
        _agentChatCoordinator = agentChatCoordinator;
        _rabbitMQService = rabbitMQService;
        _rabbitMQService.OnMessageReceived += MonitorChatSessionsAsync;
    }

    public async Task InactivateSessionsAsync(CancellationToken cancellationToken)
    {
        var unitOfWork = _unitOfWorkFactory.CreateNewUnitOfWork();
        var allSessions = await unitOfWork.ChatSessionRepository.GetAllAsync(cancellationToken);
        var allAgents = await unitOfWork.AgentRepository.GetAllAsync(cancellationToken);

        var inactiveSessions = allSessions.Where(s => (DateTime.Now - s.LastPollTime).TotalSeconds > 3);
        foreach (var session in inactiveSessions)
        {
            session.IsActive = false;
            await unitOfWork.ChatSessionRepository.UpdateAsync(session, cancellationToken);
        }       

        await unitOfWork.CompleteAsync(cancellationToken);
    }

    public async Task MonitorChatSessionsAsync(ChatSessionDto chatSessionRequest, CancellationToken cancellationToken)
    {   
        var unitOfWork = _unitOfWorkFactory.CreateNewUnitOfWork();
        var allAgents = await unitOfWork.AgentRepository.GetAllAsync(cancellationToken);

        var agent = _agentChatCoordinator.FindAvailableAgent(allAgents);

        if (agent is not null)
        {            
            await _agentChatCoordinator.AssignChatSessionToAgent(chatSessionRequest, agent, cancellationToken);
        }        
    }
}