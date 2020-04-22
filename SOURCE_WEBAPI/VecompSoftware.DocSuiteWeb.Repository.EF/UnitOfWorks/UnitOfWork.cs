using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Repository.DataContext;
using VecompSoftware.DocSuiteWeb.Repository.EF.Infrastructure;
using VecompSoftware.DocSuiteWeb.Repository.EF.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.UnitOfWorks
{
    [LogCategory(LogCategoryDefinition.UNITYOFWORK)]
    public class UnitOfWork : IUnitOfWorkAsync
    {
        #region [ Fields ]

        private IDataContextAsync _dataContext;
        private bool _disposed;
        private ObjectContext _objectContext;
        private DbTransaction _transaction;
        private Dictionary<string, dynamic> _repositories;
        private readonly ILogger _logger;
        private readonly Guid _instanceId;
        private static IEnumerable<LogCategory> _logCategories = null;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(UnitOfWork));
                }
                return _logCategories;
            }
        }

        public Guid InstanceId => _instanceId;

        #endregion

        #region [ Constuctor ]

        public UnitOfWork(IDataContextAsync dataContext, ILogger logger)
        {
            _dataContext = dataContext;
            _repositories = new Dictionary<string, dynamic>();
            _logger = logger;
            _instanceId = Guid.NewGuid();
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only

                try
                {
                    if (_objectContext != null && _objectContext.Connection != null && _objectContext.Connection.State == ConnectionState.Open)
                    {
                        _objectContext.Connection.Close();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // do nothing, the objectContext has already been disposed
                }

                if (_dataContext != null)
                {
                    _dataContext.Dispose();
                    _dataContext = null;
                }
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }

        #endregion Constuctor/Dispose

        #region [ Methods ]

        public int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DataHelper.TryCatchWithLogger(async () =>
            {
                return await _dataContext.SaveChangesAsync();
            }, _logger, LogCategories, "SaveChangesAsync", DSWExceptionCode.UW_Anomaly);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await DataHelper.TryCatchWithLogger(async () =>
            {
                return await _dataContext.SaveChangesAsync(cancellationToken);
            }, _logger, LogCategories, "SaveChangesAsync cancellationToken", DSWExceptionCode.UW_Anomaly);
        }

        public IRepositoryAsync<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            try
            {
                if (ServiceLocator.IsLocationProviderSet)
                {
                    return ServiceLocator.Current.GetInstance<IRepositoryAsync<TEntity>>();
                }

                if (_repositories == null)
                {
                    _repositories = new Dictionary<string, dynamic>();
                }

                string type = typeof(TEntity).Name;

                if (_repositories.ContainsKey(type))
                {
                    return (IRepositoryAsync<TEntity>)_repositories[type];
                }

                Type repositoryType = typeof(Repository<>);

                _repositories.Add(type, Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dataContext));

                return _repositories[type];

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(GetType(), string.Concat("RepositoryAsync -> ", ex.Message), ex, DSWExceptionCode.UW_Anomaly);
            }
        }
        #endregion

        #region Unit of Work Transactions

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            try
            {
                _objectContext = ((IObjectContextAdapter)_dataContext).ObjectContext;
                if (_objectContext.Connection.State != ConnectionState.Open)
                {
                    _objectContext.Connection.Open();
                }

                _transaction = _objectContext.Connection.BeginTransaction(isolationLevel);

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(GetType(), string.Concat("BeginTransaction -> ", ex.Message), ex, DSWExceptionCode.DB_TransactionError);
            }
        }

        public bool Commit()
        {
            try
            {
                _transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(GetType(), string.Concat("Commit -> ", ex.Message), ex, DSWExceptionCode.DB_TransactionError);
            }
        }

        public void Rollback()
        {
            try
            {
                _transaction.Rollback();
                _dataContext.SyncObjectsStatePostCommit();

            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(GetType(), string.Concat("Rollback -> ", ex.Message), ex, DSWExceptionCode.DB_TransactionError);
            }
        }

        #endregion
    }
}