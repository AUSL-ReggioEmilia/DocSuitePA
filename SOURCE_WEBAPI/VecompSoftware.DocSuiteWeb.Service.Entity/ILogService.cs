using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuiteWeb.Service.Entity
{
    public interface ILogService<TEntity, TLogEntity, TType> : IEntityBaseService<TEntity>
        where TEntity : DSWBaseLogEntity<TLogEntity, TType>, new()
        where TLogEntity : DSWBaseEntity, new()
    {
    }
}
