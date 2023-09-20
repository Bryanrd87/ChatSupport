using ChatSupport.Application.Models;
using MediatR;

namespace ChatSupport.Application.Features.Sessions.Commands;
public class CreateChatSessionCommand : IRequest<ChatSessionDto>
{
    public int UserId { get; set; }
    public string Message { get; set; }
    public CreateChatSessionCommand(int userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}

