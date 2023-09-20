using ChatSupport.Application.Contracts.RabbitMq;
using ChatSupport.Application.Exceptions;
using ChatSupport.Application.Models;
using ChatSupport.Domain;
using ChatSupport.Domain.UoW;
using MediatR;

namespace ChatSupport.Application.Features.Sessions.Commands;
public class CreateSessionCommandHandler : IRequestHandler<CreateChatSessionCommand, ChatSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMQService _rabbitMQService;
    private const string IncludeEntityUser = "User";
    public CreateSessionCommandHandler(IUnitOfWork unitOfWork, IRabbitMQService rabbitMQService)
    {
        _unitOfWork = unitOfWork;
        _rabbitMQService = rabbitMQService;
    }
    public async Task<ChatSessionDto> Handle(CreateChatSessionCommand request, CancellationToken cancellationToken)
    {
        var sender = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken) ?? throw new InvalidUserException(nameof(User), request.UserId);

        var chatSession = new ChatSession
        {
            UserId = request.UserId,
            StartTime = DateTime.Now,
            IsActive = true,
            LastPollTime = DateTime.Now
        };

        await _unitOfWork.ChatSessionRepository.AddAsync(chatSession, cancellationToken);

        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSession.ChatSessionId,
            Message = request.Message,
            Timestamp = DateTime.Now
        };

        await _unitOfWork.ChatMessageRepository.AddAsync(chatMessage, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        var result = new ChatSessionDto
        {
            ChatSessionId = chatSession.ChatSessionId,
            UserId = chatSession.UserId,
            StartTime = chatSession.StartTime,
            IsActive = chatSession.IsActive,
            LastPollTime = chatSession.LastPollTime,
            UserName = sender.Username,
            ChatMessages = new List<ChatMessageDto> { new ChatMessageDto { ChatMessageId = chatMessage.ChatMessageId, Message = chatMessage.Message, Sender = sender.Username, Timestamp = chatMessage.Timestamp } }
        };

        _rabbitMQService.PublishChatSession(result);

        return result;
    }
}