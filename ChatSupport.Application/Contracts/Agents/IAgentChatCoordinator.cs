using ChatSupport.Application.Models;
using ChatSupport.Domain;

namespace ChatSupport.Application.Contracts.Agents;
public interface IAgentChatCoordinator
{
    Agent FindAvailableAgent(IEnumerable<Agent> allAgents);
    Task AssignChatSessionToAgent(ChatSessionDto chatSessionDto, Agent agent, CancellationToken cancellationToken);
}
