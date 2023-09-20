using ChatSupport.Application.Models;
using ChatSupport.Domain.UoW;
using MediatR;

namespace ChatSupport.Application.Features.PollChats.Queries;
public class PollChatSessionQueryHandler : IRequestHandler<PollChatSessionQuery, ChatSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public PollChatSessionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ChatSessionDto> Handle(PollChatSessionQuery request, CancellationToken cancellationToken)
    {
        ChatSessionDto chatSessionDto = new();
        var chatSession = await _unitOfWork.ChatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession is not null)
        {
            chatSession.LastPollTime = DateTime.Now;
            await _unitOfWork.ChatSessionRepository.UpdateAsync(chatSession, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            chatSessionDto = new ChatSessionDto
            {
                UserId = chatSession.UserId,
                StartTime = chatSession.StartTime,
                LastPollTime = chatSession.LastPollTime,
                IsActive = chatSession.IsActive
            };

            return chatSessionDto;
        }

        return chatSessionDto;
    }
}
