using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts
{
    public abstract class FakeDbContext : IFakeDbContext
    {
        #region [ Fields ]
        private readonly Dictionary<Type, object> _fakeDbSets;
        #endregion

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

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) { return Task.Factory.StartNew(() => default(int), cancellationToken); }

        public Task<int> SaveChangesAsync() { return Task.Factory.StartNew(() => default(int)); }

        public void Dispose() { }

        public DbSet<T> Set<T>() where T : class { return (DbSet<T>)_fakeDbSets[typeof(T)]; }

        public void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : BaseEntity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new()
        {
            TFakeDbSet fakeDbSet = Activator.CreateInstance<TFakeDbSet>();
            _fakeDbSets.Add(typeof(TEntity), fakeDbSet);
        }

        public void SyncObjectsStatePostCommit() { }
    }
}