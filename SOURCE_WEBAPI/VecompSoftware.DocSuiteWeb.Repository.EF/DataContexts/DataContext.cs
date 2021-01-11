using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Repository.DataContext;
using VecompSoftware.DocSuiteWeb.Repository.EF.Infrastructure;
using VecompSoftware.DocSuiteWeb.Repository.Entity;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;

namespace VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts
{
    [LogCategory(LogCategoryDefinition.DATACONTEXT)]
    public class DataContext : DbContext, IDataContextAsync
    {
        #region [ Fields ]
        private readonly Guid _instanceId;
        private readonly ILogger _logger;
        private readonly ICurrentIdentity _currentIdentity;
        private static IEnumerable<LogCategory> _logCategories = null;
        private static readonly Type _type_IUnauditableEntity = typeof(IUnauditableEntity);
        private bool _disposed;
        #endregion

        #region [ Constructor ]
        public DataContext(string nameOrConnectionstring, ILogger logger, ICurrentIdentity currentIdentity)
            : base(nameOrConnectionstring)
        {
            _instanceId = Guid.NewGuid();
            _logger = logger;
            _currentIdentity = currentIdentity;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(DataContext));
                }
                return _logCategories;
            }
        }

        public Guid InstanceId => _instanceId;

        #endregion

        #region [ Methods ]

        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.</exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.</exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.</exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.</exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.</exception>
        /// <seealso cref="DbContext.SaveChanges"/>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            return DataHelper.TryCatchWithLogger<int>(() =>
                {
                    SyncObjectsStatePreCommit();
                    int changes = base.SaveChanges();
                    SyncObjectsStatePostCommit();
                    return changes;
                }, _logger, LogCategories, "SaveChanges", DSWExceptionCode.DB_Anomaly);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.</exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.</exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.</exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.</exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.</exception>
        /// <seealso cref="DbContext.SaveChangesAsync"/>
        /// <returns>A task that represents the asynchronous save operation.  The 
        ///     <see cref="Task.Result">Task.Result</see> contains the number of 
        ///     objects written to the underlying database.</returns>
        public override async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(CancellationToken.None);
        }
        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.</exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.</exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.</exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.</exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.</exception>
        /// <seealso cref="DbContext.SaveChangesAsync"/>
        /// <returns>A task that represents the asynchronous save operation.  The 
        ///     <see cref="Task.Result">Task.Result</see> contains the number of 
        ///     objects written to the underlying database.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await DataHelper.TryCatchWithLogger(async () =>
                {
                    SyncObjectsStatePreCommit();
                    int changesAsync = await base.SaveChangesAsync(cancellationToken);
                    SyncObjectsStatePostCommit();
                    return changesAsync;
                }, _logger, LogCategories, "SaveChangesAsync", DSWExceptionCode.DB_Anomaly);
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            Entry(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        private void SyncObjectsStatePreCommit()
        {
            DateTimeOffset currentDate = DateTimeOffset.UtcNow;
            string currentRegistrationUser = _currentIdentity.FullUserName;
            string currentChangedUser = _currentIdentity.FullUserName;
            IEntity entityBase = null;

            foreach (DbEntityEntry dbEntityEntry in ChangeTracker.Entries())
            {
                entityBase = (IEntity)dbEntityEntry.Entity;
                dbEntityEntry.State = StateHelper.ConvertState(entityBase.ObjectState);
                if (_type_IUnauditableEntity.IsAssignableFrom(entityBase.GetType()) &&
                    !string.IsNullOrEmpty(entityBase.RegistrationUser))
                {
                    currentRegistrationUser = entityBase.RegistrationUser;
                }
                if (_type_IUnauditableEntity.IsAssignableFrom(entityBase.GetType()) &&
                    entityBase.RegistrationDate != DateTimeOffset.MinValue)
                {
                    currentDate = entityBase.RegistrationDate;
                }
                if (_type_IUnauditableEntity.IsAssignableFrom(entityBase.GetType()) &&
                    !string.IsNullOrEmpty(entityBase.LastChangedUser))
                {
                    currentChangedUser = entityBase.LastChangedUser;
                }

                if (entityBase.ObjectState == ObjectState.Added)
                {
                    entityBase.RegistrationDate = currentDate;
                    entityBase.RegistrationUser = currentRegistrationUser;
                }

                if (entityBase.ObjectState == ObjectState.Modified)
                {
                    entityBase.LastChangedDate = currentDate;
                    entityBase.LastChangedUser = currentChangedUser;
                }
            }
        }

        public void SyncObjectsStatePostCommit()
        {
            foreach (DbEntityEntry dbEntityEntry in ChangeTracker.Entries())
            {
                ((IObjectState)dbEntityEntry.Entity).ObjectState = StateHelper.ConvertState(dbEntityEntry.State);
            }
        }
        #endregion

        #region [ Dispose ]
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // free other managed objects that implement
                    // IDisposable only
                }

                // release any unmanaged objects
                // set object references to null

                _disposed = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}