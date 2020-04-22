using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public interface IFascicolableService<TEntity, TFascicolable> : IEntityBaseService<TEntity>
        where TEntity : DSWBaseEntityFascicolable<TFascicolable>, new()
        where TFascicolable : DSWBaseEntity, new()
    {
    }
}
