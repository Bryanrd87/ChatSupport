namespace ChatSupport.Domain;
public class ChatSession
{
    public int ChatSessionId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? AgentId { get; set; }
    public Agent? Agent { get; set; }
    public DateTime StartTime { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastPollTime { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
