namespace VecompSoftware.DocSuiteWeb.Service
{
    public interface IServiceUnitOfWork
    {
        TService Repository<TService>() where TService : IBaseService;
    }
}
