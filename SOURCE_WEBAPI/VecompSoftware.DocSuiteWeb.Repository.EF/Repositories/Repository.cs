using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Repository.DataContext;
using VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts;
using VecompSoftware.DocSuiteWeb.Repository.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.Repositories
{
    public class Repository<TEntity> : IRepositoryAsync<TEntity> where TEntity : BaseEntity
    {
        #region [ Fields ]

        private readonly IDataContextAsync _context;
        private readonly DbSet<TEntity> _dbSet;
        private bool _optimization = false;
        #endregion

        #region [ Constructor ]
        public Repository(IDataContextAsync context)
        {
            _context = context;
            // Temporarily for FakeDbContext, Unit Test and Fakes
            DbContext dbContext = context as DbContext;

            if (dbContext != null)
            {
                _dbSet = dbContext.Set<TEntity>();
            }
            else
            {
                IFakeDbContext fakeContext = context as IFakeDbContext;

                if (fakeContext != null)
                {
                    _dbSet = fakeContext.Set<TEntity>();
                }
            }
        }
        #endregion

        #region [ Methods ]
        public virtual TEntity Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsNoTracking().AsQueryable();
        }

        public virtual void Insert(TEntity entity)
        {
            entity.ObjectState = ObjectState.Added;
            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                Insert(entity);
            }
        }

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            entity.ObjectState = ObjectState.Modified;
            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entity = _dbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            entity.ObjectState = ObjectState.Deleted;
            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }
        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                Delete(entity);
            }
        }

        public IQueryFluent<TEntity> Query(bool optimization = false)
        {
            _optimization = optimization; ;
            return new QueryFluent<TEntity>(this);
        }

        public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject, bool optimization = false)
        {
            _optimization = optimization; ;
            return new QueryFluent<TEntity>(this, queryObject);
        }

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query, bool optimization = false)
        {
            _optimization = optimization;
            return new QueryFluent<TEntity>(this, query);
        }

        public IQueryable<TEntity> Queryable(bool optimization = false)
        {
            if (optimization)
            {
                return _dbSet.AsNoTracking();
            }
            return _dbSet;
        }

        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = Queryable(_optimization);

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }
            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        internal async Task<IEnumerable<TEntity>> SelectAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            return await Select(filter, orderBy, includes, page, pageSize).ToListAsync();
        }

        public virtual void InsertOrUpdateGraph(TEntity entity)
        {
            SyncObjectGraph(entity);
            _entitesChecked = null;
            _dbSet.Attach(entity);
        }

        private HashSet<object> _entitesChecked; // tracking of all process entities in the object graph when calling SyncObjectGraph

        private void SyncObjectGraph(object entity) // scan object graph for all 
        {
            if (_entitesChecked == null)
            {
                _entitesChecked = new HashSet<object>();
            }

            if (_entitesChecked.Contains(entity))
            {
                return;
            }

            _entitesChecked.Add(entity);

            IObjectState objectState = entity as IObjectState;

            if (objectState != null && objectState.ObjectState == ObjectState.Added)
            {
                _context.SyncObjectState(entity as BaseEntity);
            }

            // Set tracking state for child collections
            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                IObjectState trackableRef = prop.GetValue(entity, null) as IObjectState;
                if (trackableRef != null)
                {
                    if (trackableRef.ObjectState == ObjectState.Added)
                    {
                        _context.SyncObjectState(entity as BaseEntity);
                    }

                    SyncObjectGraph(prop.GetValue(entity, null));
                }

                // Apply changes to 1-M properties
                IEnumerable<IObjectState> items = prop.GetValue(entity, null) as IEnumerable<IObjectState>;
                if (items == null)
                {
                    continue;
                }

                foreach (IObjectState item in items)
                {
                    SyncObjectGraph(item);
                }
            }
        }

        internal IEnumerable<TEntity> ExecuteFunction(string functionName, params IQueryParameter[] parameters)
        {
            string sql = string.Format("SELECT * FROM {0}({1})", functionName, string.Join(", ", parameters.Select(s => s.ParameterName)));
            IEnumerable<TEntity> results = _dbSet.SqlQuery(sql, parameters.Select(s => new SqlParameter(s.ParameterName, s.ParameterValue) { TypeName = s.ParameterTypeName }).ToArray())
                .AsNoTracking();
            return results;
        }

        public ICollection<TModel> ExecuteModelFunction<TModel>(string functionName, params IQueryParameter[] parameters)
        {
            if (_context is DbContext)
            {
                string sql = string.Format("SELECT * FROM {0}({1})", functionName, string.Join(", ", parameters.Select(s => s.ParameterName)));
                ICollection<TModel> results = (_context as DbContext).Database.SqlQuery<TModel>(sql, parameters.Select(s => new SqlParameter(s.ParameterName, s.ParameterValue) { TypeName = s.ParameterTypeName }).ToArray())
                    .ToList();
                return results;
            }
            return new List<TModel>();
        }

        public TModel ExecuteModelScalarFunction<TModel>(string functionName, params IQueryParameter[] parameters)
        {
            if (_context is DbContext)
            {
                string sql = string.Format("SELECT {0}({1})", functionName, string.Join(", ", parameters.Select(s => s.ParameterName)));
                TModel result = (_context as DbContext).Database.SqlQuery<TModel>(sql, parameters.Select(s => new SqlParameter(s.ParameterName, s.ParameterValue) { TypeName = s.ParameterTypeName }).ToArray()).Single();
                return result;
            }
            return default(TModel);
        }
        public void ExecuteProcedure(string procedureName, params IQueryParameter[] parameters)
        {
            if (_context is DbContext)
            {
                string sql = string.Format("EXECUTE {0} {1}", procedureName, string.Join(", ", parameters.Select(s => s.ParameterName)));
                (_context as DbContext).Database.ExecuteSqlCommand(sql, parameters.Select(s => new SqlParameter(s.ParameterName, s.ParameterValue) { TypeName = s.ParameterTypeName }).ToArray());
            }
        }
        #endregion

        #region [ Async Methods ]

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await _dbSet.FindAsync(cancellationToken, keyValues);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await DeleteAsync(CancellationToken.None, keyValues);
        }


        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            TEntity entity = await FindAsync(cancellationToken, keyValues);

            if (entity == null)
            {
                return false;
            }

            Delete(entity);

            return true;
        }

        #endregion
    }
}