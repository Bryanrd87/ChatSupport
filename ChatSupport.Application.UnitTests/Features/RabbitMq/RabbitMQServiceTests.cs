namespace ChatSupport.Application.UnitTests.Features.RabbitMq;
public class RabbitMQServiceTests
{
    private Mock<ILogger<RabbitMQService>> _loggerMock;
    private Mock<IConfiguration> _configurationMock;
    private RabbitMQService _rabbitMQService;

    public RabbitMQServiceTests()
    {
        _loggerMock = new Mock<ILogger<RabbitMQService>>();
        _configurationMock = new Mock<IConfiguration>();
        
        _configurationMock.Setup(x => x["RabbitMQ:HostName"]).Returns("localhost");
        _configurationMock.Setup(x => x["RabbitMQ:Port"]).Returns("5672");
        _configurationMock.Setup(x => x["RabbitMQ:UserName"]).Returns("guest");
        _configurationMock.Setup(x => x["RabbitMQ:Password"]).Returns("guest");
        _configurationMock.Setup(x => x["RabbitMQ:VirtualHost"]).Returns("/");

        _rabbitMQService = new RabbitMQService(_loggerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public void PublishChatSession_ShouldPublishMessageWithoutError()
    {
        // Arrange
        var chatSessionDto = new ChatSessionDto
        {
            UserId = 1,
            IsActive = true,
            LastPollTime = DateTime.Now,
            StartTime = DateTime.Now,
            UserName = "Test",
            ChatSessionId = 1,
            ChatMessages = new List<ChatMessageDto> { new ChatMessageDto { ChatSessionId = 1, Message = "Test", Sender = "Test", ChatMessageId = 1, Timestamp = DateTime.Now } }
        };

        // Act
        Exception exception = null;
        try
        {
            _rabbitMQService.PublishChatSession(chatSessionDto);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ConsumeChatSessionQueue_ShouldTriggerOnMessageReceivedEvent()
    {
        // Arrange
        var chatSessionDto = new ChatSessionDto
        {
            UserId = 1,
            IsActive = true,
            LastPollTime = DateTime.Now,
            StartTime = DateTime.Now,
            UserName = "Test",
            ChatSessionId = 1,
            ChatMessages = new List<ChatMessageDto> { new ChatMessageDto { ChatSessionId = 1, Message = "Test", Sender = "Test", ChatMessageId = 1, Timestamp = DateTime.Now } }
        };

        var messageReceived = new TaskCompletionSource<bool>();

        _rabbitMQService.OnMessageReceived += (session, token) =>
        {
            if (session.ChatSessionId == chatSessionDto.ChatSessionId)
            {
                messageReceived.SetResult(true);
            }
            return Task.CompletedTask;
        };

        // Act
        _rabbitMQService.PublishChatSession(chatSessionDto);

        _rabbitMQService.ConsumeChatSessionQueue(CancellationToken.None);
        
        var waitResult = await Task.WhenAny(messageReceived.Task, Task.Delay(5000));

        // Assert
        Assert.True(waitResult == messageReceived.Task && messageReceived.Task.Result, "OnMessageReceived event not triggered with correct ChatSessionDto");
    }

}