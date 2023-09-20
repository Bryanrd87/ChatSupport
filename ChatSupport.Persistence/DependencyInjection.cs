using ChatSupport.Domain.Repositories;
using ChatSupport.Domain.UoW;
using ChatSupport.Persistence.Repositories;
using ChatSupport.Persistence.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace ChatSupport.Persistence;
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
    this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("ChatSupport")
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}