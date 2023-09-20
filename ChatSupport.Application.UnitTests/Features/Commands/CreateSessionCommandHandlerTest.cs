namespace ChatSupport.Application.UnitTests.Features.Commands;
public class CreateSessionCommandHandlerTest
{
    private readonly Mock<IUnitOfWork> _iUnitOfWorkMock;
    private readonly Mock<IRepository<ChatSession>> _chatSessionRepositoryMock;
    private readonly Mock<IRepository<ChatMessage>> _chatMessageRepositoryMock;
    private readonly Mock<IRabbitMQService> _iRabbitMQServiceMock;
    private readonly CreateSessionCommandHandler _handler;
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    public CreateSessionCommandHandlerTest()
    {
        _iUnitOfWorkMock = new Mock<IUnitOfWork>();
        _iRabbitMQServiceMock = new Mock<IRabbitMQService>();
        _chatSessionRepositoryMock = new Mock<IRepository<ChatSession>>();
        _chatMessageRepositoryMock = new Mock<IRepository<ChatMessage>>();
        _userRepositoryMock = new Mock<IRepository<User>>();            

        _handler = new CreateSessionCommandHandler(_iUnitOfWorkMock.Object, _iRabbitMQServiceMock.Object);

        _iUnitOfWorkMock.Setup(uow => uow.ChatSessionRepository).Returns(_chatSessionRepositoryMock.Object);
        _iUnitOfWorkMock.Setup(uow => uow.ChatMessageRepository).Returns(_chatMessageRepositoryMock.Object);
        _iUnitOfWorkMock.Setup(uow => uow.UserRepository).Returns(_userRepositoryMock.Object);
    } 
    [Fact]
    public async Task Handle_ShouldCreateChatSessionAndPublishMessage()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var messageText = "test message";

        var command = new CreateChatSessionCommand(userId, messageText);

        var initialChatSession = new ChatSession { ChatSessionId = 1, UserId = userId };
        var user = new User { UserId = userId, Username = username };
        var chatSessionWithUser = new ChatSession { ChatSessionId = 1, UserId = userId, User = user };
        var chatMessage = new ChatMessage { ChatMessageId = 1, ChatSessionId = 1, Message = messageText };

        SetupMocks(initialChatSession, chatMessage, user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        AssertResult(result, initialChatSession, user, chatMessage);
    }

    private void SetupMocks(ChatSession initialChatSession, ChatMessage chatMessage, User user)
    {          
        _iUnitOfWorkMock
            .Setup(x => x.UserRepository.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>(), null)).ReturnsAsync(user);

        _iUnitOfWorkMock
            .Setup(x => x.ChatSessionRepository.AddAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback((ChatSession cs, CancellationToken ct) =>
            {
                cs.ChatSessionId = 1;
                initialChatSession = cs;
            });

        _iUnitOfWorkMock
            .Setup(x => x.ChatMessageRepository.AddAsync(It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback((ChatMessage cm, CancellationToken ct) =>
            {
                cm.ChatMessageId = 1;
                chatMessage = cm;
            });


        _iUnitOfWorkMock.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()));            
    }

    private void AssertResult(ChatSessionDto result, ChatSession initialChatSession, User user, ChatMessage chatMessage)
    {
        result.ChatSessionId.ShouldBe(initialChatSession.ChatSessionId);
        result.UserId.ShouldBe(user.UserId);
        result.UserName.ShouldBe(user.Username);
        result.IsActive.ShouldBe(true);
        result.ChatMessages.First().Message.ShouldBe(chatMessage.Message);
        result.ChatMessages.First().Sender.ShouldBe(user.Username);

        _iUnitOfWorkMock.Verify(x => x.ChatSessionRepository.AddAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()), Times.Once);
        _iUnitOfWorkMock.Verify(x => x.ChatMessageRepository.AddAsync(It.IsAny<ChatMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _iUnitOfWorkMock.Verify(x => x.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        _iRabbitMQServiceMock.Verify(x => x.PublishChatSession(result), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidUserException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 10;
        var messageText = "test message";
        CreateChatSessionCommand command = new(userId, messageText);

        _iUnitOfWorkMock
            .Setup(x => x.UserRepository.GetByIdAsync(userId, It.IsAny<CancellationToken>(), null)).ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidUserException>(() => _handler.Handle(command, CancellationToken.None));
    }

}