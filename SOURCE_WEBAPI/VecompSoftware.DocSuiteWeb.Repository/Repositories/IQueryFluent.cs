using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;

namespace VecompSoftware.DocSuiteWeb.Repository.Repositories
{
    public interface IQueryFluent<TEntity> where TEntity : IObjectState
    {
        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount);
        IEnumerable<TEntity> Top(int number);
        IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector = null);
        IEnumerable<TEntity> Select();
        IQueryable<TEntity> SelectAsQueryable();
        Task<IEnumerable<TEntity>> SelectAsync();
        IEnumerable<TEntity> SqlFunction(string functionName, params IQueryParameter[] parameters);
    }
}