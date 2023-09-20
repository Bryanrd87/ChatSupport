using ChatSupport.Application.Models;
using MediatR;

namespace ChatSupport.Application.Features.PollChats.Queries;
public class PollChatSessionQuery : IRequest<ChatSessionDto>
{
    public int ChatSessionId { get; set; }
}