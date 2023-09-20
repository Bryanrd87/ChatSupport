namespace ChatSupport.Application.Models;
public record ChatSessionDto
{
    public int ChatSessionId { get; init; }
    public int UserId { get; init; }
    public string UserName { get; init; }
    public int? AgentId { get; init; }
    public string AgentName { get; init; }
    public DateTime StartTime { get; init; }
    public bool IsActive { get; init; }
    public DateTime LastPollTime { get; init; }
    public List<ChatMessageDto> ChatMessages { get; init; }
}