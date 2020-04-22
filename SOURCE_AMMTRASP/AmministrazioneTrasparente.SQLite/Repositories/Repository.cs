using AmministrazioneTrasparente.SQLite.Entities;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace AmministrazioneTrasparente.SQLite.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
        #region [Fields]
        private readonly Database _context;
        private bool disposed = false;
        #endregion

        #region [Dispose]       

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this._context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region [Constructor]
        public Repository()
        {
            this._context = new Database(ConfigurationManager.ConnectionStrings["AmmTraspLite"].ConnectionString, new SQLiteDatabaseProvider());
        }
        #endregion

        #region [Method]

        public TEntity Find(params object[] keyValues)
        {
            return this._context.SingleOrDefault<TEntity>(keyValues);
        }

        public IEnumerable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return this._context.Query<TEntity>(query, parameters);
        }

        public IEnumerable<TEntity> SelectQuery<TJoinEntity>(string query, params object[] parameters) where TJoinEntity : IEntity
        {
            return this._context.Query<TEntity, TJoinEntity>(query, parameters);
        }

        public void Insert(TEntity entity)
        {
            this._context.Insert(entity);
        }

        public void Update(TEntity entity)
        {
            this._context.Update(entity);
        }

        public void Delete(object id)
        {
            this._context.Delete<TEntity>(id);
        }

        public void Delete(TEntity entity)
        {
            this._context.Delete(entity);
        }

        #endregion
    }
}
