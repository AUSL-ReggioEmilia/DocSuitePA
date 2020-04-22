using VecompSoftware.Commons.Interfaces.ServiceLocator;

namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public class MapperUnitOfWork : IMapperUnitOfWork
    {
        #region [ Fields ]
        private readonly ILocator _serviceLocator;
        #endregion

        #region [ Constuctor ]

        public MapperUnitOfWork(ILocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }
        #endregion

        #region [ Methods ]

        public TMapper Repository<TMapper>()
            where TMapper : IBaseMapper
        {
            return _serviceLocator.GetService<TMapper>();
        }
        #endregion
    }
}
