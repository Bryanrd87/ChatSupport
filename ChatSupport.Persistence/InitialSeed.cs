using ChatSupport.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChatSupport.Persistence;
public class InitialSeed
{
    public static void Initialize(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.EnsureCreated();

        context.Users.AddRange(new List<User>
        {
            new User
            {
                UserId = 1,
                Username = "Test",
            },
            new User
            {
                UserId = 2,
                Username = "Test2",
            }
        });

        context.Agents.AddRange(new List<Agent>
        {
           new Agent { AgentId = 1, CurrentChatCount = 20 , Name = "Agent 1" , SeniorityLevel = Domain.Enums.SeniorityLevel.Junior, ShiftEndHour = 23, ShiftStartHour = 10},
           new Agent { AgentId = 2, CurrentChatCount = 0 , Name = "Agent 21" , SeniorityLevel = Domain.Enums.SeniorityLevel.Senior, ShiftEndHour = 11, ShiftStartHour = 8},
        });        

        context.SaveChanges();
    }
}

