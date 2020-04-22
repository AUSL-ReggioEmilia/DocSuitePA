using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VecompSoftware.DocSuiteWeb.Repository.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;

namespace VecompSoftware.DocSuiteWeb.Repository.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        TEntity Find(params object[] keyValues);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertOrUpdateGraph(TEntity entity);
        void InsertGraphRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject, bool optimization = false);
        IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query, bool optimization = false);
        IQueryFluent<TEntity> Query(bool optimization = false);
        IQueryable<TEntity> Queryable(bool optimization = false);
        ICollection<TModel> ExecuteModelFunction<TModel>(string functionName, params IQueryParameter[] parameters);
        TModel ExecuteModelScalarFunction<TModel>(string functionName, params IQueryParameter[] parameters);
        void ExecuteProcedure(string procedureName, params IQueryParameter[] parameters);
    }
}