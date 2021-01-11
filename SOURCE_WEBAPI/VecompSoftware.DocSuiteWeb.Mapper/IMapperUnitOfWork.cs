
namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public interface IMapperUnitOfWork
    {
        TMapper Repository<TMapper>() where TMapper : IBaseMapper;
    }
}
