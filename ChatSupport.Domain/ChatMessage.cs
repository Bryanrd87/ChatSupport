namespace ChatSupport.Domain;
public class ChatMessage
{
    public int ChatMessageId { get; set; }
    public int ChatSessionId { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public int UserId { get; set; }
}

