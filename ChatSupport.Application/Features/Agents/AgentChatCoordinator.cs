using ChatSupport.Application.Contracts.Agents;
using ChatSupport.Application.Models;
using ChatSupport.Domain.UoW;
using ChatSupport.Domain;
using ChatSupport.Domain.Enums;

namespace ChatSupport.Application.Features.Agents;
public class AgentChatCoordinator : IAgentChatCoordinator
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public AgentChatCoordinator(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public Agent FindAvailableAgent(IEnumerable<Agent> allAgents)
    {
        var currentTime = DateTime.Now.Hour;
        var availableAgents = allAgents.Where(a => a.CurrentChatCount < GetAgentCapacity(a) && a.ShiftStartHour <= currentTime && a.ShiftEndHour > currentTime);
        return availableAgents.OrderBy(a => GetAgentPriority(a)).ThenBy(a => a.CurrentChatCount).FirstOrDefault();
    }

    private static int GetAgentCapacity(Agent agent)
    {
        double seniorityMultiplier = agent.SeniorityLevel switch
        {
            SeniorityLevel.Junior => 0.4,
            SeniorityLevel.MidLevel => 0.6,
            SeniorityLevel.Senior => 0.8,
            SeniorityLevel.TeamLead => 0.5,
            _ => throw new InvalidOperationException("Unknown seniority level")
        };

        return (int)(10 * seniorityMultiplier);
    }

    private static int GetAgentPriority(Agent agent)
    {
        int seniorityPriorities = agent.SeniorityLevel switch
        {
            SeniorityLevel.Junior => 1,
            SeniorityLevel.MidLevel => 2,
            SeniorityLevel.Senior => 3,
            SeniorityLevel.TeamLead => 5,
            _ => throw new InvalidOperationException("Unknown seniority level")
        };

        return seniorityPriorities;
    }

    public async Task AssignChatSessionToAgent(ChatSessionDto chatSessionDto, Agent agent, CancellationToken cancellationToken)
    {
        var unitOfWork = _unitOfWorkFactory.CreateNewUnitOfWork();
        var chatSession = await unitOfWork.ChatSessionRepository.GetByIdAsync(chatSessionDto.ChatSessionId, cancellationToken);
        if (chatSession is not null)
        {
            chatSession.AgentId = agent.AgentId;
            chatSession.Agent = agent;

            await unitOfWork.ChatSessionRepository.UpdateAsync(chatSession, cancellationToken);

            agent.CurrentChatCount += 1;
            await unitOfWork.AgentRepository.UpdateAsync(agent, cancellationToken);

            await unitOfWork.CompleteAsync(cancellationToken);
        }
    }    
}
