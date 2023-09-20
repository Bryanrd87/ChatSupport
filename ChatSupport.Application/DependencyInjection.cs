using ChatSupport.Application.Contracts.Agents;
using ChatSupport.Application.Contracts.RabbitMq;
using ChatSupport.Application.Contracts.Sessions;
using ChatSupport.Application.Features.Agents;
using ChatSupport.Application.Features.RabbitMq;
using ChatSupport.Application.Features.Sessions;
using ChatSupport.Application.Service;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace ChatSupport.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();

            config.NotificationPublisher = new TaskWhenAllPublisher();
        });        

        services.AddSingleton<IRabbitMQService, RabbitMQService>();

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory { HostName = configuration["RabbitMQ:HostName"] };
            return factory.CreateConnection();
        });

        services.AddTransient<IAgentChatCoordinator, AgentChatCoordinator>();

        services.AddTransient<IChatSessionMonitor, ChatSessionMonitor>();       

        services.AddHostedService<ChatSessionMonitorHostedService>();

        services.AddHostedService<ChatSessionQueueConsumerHostedService>();

        return services;
    }
}
