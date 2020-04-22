using System;
using System.Data;
using VecompSoftware.DocSuiteWeb.Repository.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        IRepositoryAsync<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        bool Commit();
        void Rollback();
    }
}