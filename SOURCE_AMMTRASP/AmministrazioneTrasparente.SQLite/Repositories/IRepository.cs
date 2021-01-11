using AmministrazioneTrasparente.SQLite.Entities;
using System;
using System.Collections.Generic;

namespace AmministrazioneTrasparente.SQLite.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity: IEntity
    {
        TEntity Find(params object[] keyValues);
        IEnumerable<TEntity> SelectQuery(string query, params object[] parameters);
        IEnumerable<TEntity> SelectQuery<TJoinEntity>(string query, params object[] parameters) where TJoinEntity : IEntity;
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
    }
}
