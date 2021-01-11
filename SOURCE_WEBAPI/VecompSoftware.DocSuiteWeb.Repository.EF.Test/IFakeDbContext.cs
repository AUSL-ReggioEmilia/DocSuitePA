using System.Data.Entity;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.Test
{
    public interface IFakeDbContext : IDSWDataContext
    {
        DbSet<T> Set<T>() where T : DSWBaseEntity;

        void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : DSWBaseEntity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new();
    }
}
