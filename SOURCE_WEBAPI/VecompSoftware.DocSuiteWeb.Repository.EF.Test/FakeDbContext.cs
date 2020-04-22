using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.Test
{
    public abstract class FakeDbContext : IFakeDbContext
    {
        #region Private Fields  
        private readonly Dictionary<Type, object> _fakeDbSets;
        #endregion Private Fields

        #region [ Constructor ]

        protected FakeDbContext()
        {
            _fakeDbSets = new Dictionary<Type, object>();
        }
        #endregion

        public int SaveChanges() { return default(int); }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            // no implentation needed, unit tests which uses FakeDbContext since there is no actual database for unit tests, 
            // there is no actual DbContext to sync with, please look at the Integration Tests for test that will run against an actual database.
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) { return new Task<int>(() => default(int)); }

        public Task<int> SaveChangesAsync() { return new Task<int>(() => default(int)); }

        public void Dispose() { }

        public DbSet<T> Set<T>() where T : DSWBaseEntity { return (DbSet<T>)_fakeDbSets[typeof(T)]; }

        public void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : DSWBaseEntity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new()
        {
            var fakeDbSet = Activator.CreateInstance<TFakeDbSet>();
            _fakeDbSets.Add(typeof(TEntity), fakeDbSet);
        }

        public void SyncObjectsStatePostCommit() { }

        public IQueryable<T> DataSet<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> ExecuteModelFunction<TModel>(string functionName, params IQueryParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}