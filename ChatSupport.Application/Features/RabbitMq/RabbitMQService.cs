using ChatSupport.Application.Contracts.RabbitMq;
using ChatSupport.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatSupport.Application.Features.RabbitMq;
public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _hostName;
    private readonly int _port;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _virtualHost;   
    private readonly ILogger<RabbitMQService> _logger;
    public event Func<ChatSessionDto, CancellationToken, Task> OnMessageReceived;
    private bool _disposed;

    public RabbitMQService(ILogger<RabbitMQService> logger, IConfiguration configuration)
    {
        _logger = logger;        
        _hostName = configuration["RabbitMQ:HostName"];
        _port = int.Parse(configuration["RabbitMQ:Port"]);
        _userName = configuration["RabbitMQ:UserName"];
        _password = configuration["RabbitMQ:Password"];
        _virtualHost = configuration["RabbitMQ:VirtualHost"];
        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            Port = _port,
            UserName = _userName,
            Password = _password,
            VirtualHost = _virtualHost
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "ChatSessionQueue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void PublishChatSession(ChatSessionDto session)
    {
        try
        {
            var messageJson = JsonConvert.SerializeObject(session);
            var body = Encoding.UTF8.GetBytes(messageJson);
            _channel.BasicPublish(exchange: "",
                                  routingKey: "ChatSessionQueue",
                                  basicProperties: null,
                                  body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}.");
            throw;
        }
    }

    public void ConsumeChatSessionQueue(CancellationToken cancellationToken)
    {
        try
        {
            var consumer = new EventingBasicConsumer(_channel);
            cancellationToken.ThrowIfCancellationRequested();

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var chatSessionRequest = JsonConvert.DeserializeObject<ChatSessionDto>(message);
                
                if (OnMessageReceived is not null)
                {
                    await OnMessageReceived.Invoke(chatSessionRequest, cancellationToken);
                }
            };

            _channel.BasicConsume(queue: "ChatSessionQueue", autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}.");
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _connection.Dispose();
        _disposed = true;
    }
}
