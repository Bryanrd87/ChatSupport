namespace ChatSupport.Domain.UoW;
public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateNewUnitOfWork();
}
