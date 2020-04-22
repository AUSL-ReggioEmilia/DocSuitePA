using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuiteWeb.Service.Entity
{
    public interface IEntityBaseService<TEntity> : IServiceAsync<TEntity, TEntity>
        where TEntity : DSWBaseEntity, new()
    {
        Task<TEntity> UpdateAsync(TEntity content, UpdateActionType actionType);

        Task<TEntity> CreateAsync(TEntity content, InsertActionType actionType);

        Task<bool> DeleteAsync(TEntity content, DeleteActionType actionType);
    }
}