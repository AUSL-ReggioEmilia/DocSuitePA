using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuiteWeb.Service.Entity
{
    internal interface IServiceEntity<TEntity> : IServiceAsync<TEntity, TEntity>
        where TEntity : DSWBaseEntity, new()
    {
    }
}
