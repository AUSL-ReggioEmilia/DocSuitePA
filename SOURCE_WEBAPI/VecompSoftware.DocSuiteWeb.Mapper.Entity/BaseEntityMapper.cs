using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public abstract class BaseEntityMapper<T, TTransformed> : BaseDomainMapper<T, TTransformed>, IDomainMapper<T, TTransformed>
        where T : IDSWEntity
        where TTransformed : IDSWEntity, new()
    {
        public BaseEntityMapper() { }

    }
}
