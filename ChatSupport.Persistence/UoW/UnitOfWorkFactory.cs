using ChatSupport.Domain.UoW;
using Microsoft.Extensions.DependencyInjection;

namespace ChatSupport.Persistence.UoW;
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UnitOfWorkFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IUnitOfWork CreateNewUnitOfWork()
    {
        var scope = _serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
}
