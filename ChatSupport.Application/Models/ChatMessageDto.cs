namespace ChatSupport.Application.Models;
public record ChatMessageDto
{
    public int ChatMessageId { get; init; }
    public int ChatSessionId { get; init; }
    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
    public string Sender { get; init; }
}