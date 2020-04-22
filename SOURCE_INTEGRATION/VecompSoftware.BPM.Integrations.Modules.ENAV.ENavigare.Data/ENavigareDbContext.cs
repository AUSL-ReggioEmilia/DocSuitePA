using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ENavigare.Data.Configurations;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ENavigare.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ENavigare.Data.Mappings;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ENavigare.Data
{
    [LogCategory(LogCategoryDefinition.DATACONTEXT)]
    [DbConfigurationType(typeof(ENavigareDbConfiguration))]
    public class ENavigareDbContext : DbContext
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private static IEnumerable<LogCategory> _logCategories;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ENavigareDbContext));
                }
                return _logCategories;
            }
        }

        public virtual DbSet<ENavigareDocumentSeriesItem> DocumentSeries { get; set; }

        #endregion

        #region [ Constructor ]
        public ENavigareDbContext(ILogger logger, JsonSerializerSettings jsonSerializerSettings, string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<ENavigareDbContext>(null);
            Configuration.ProxyCreationEnabled = false;
            _logger = logger;
            _jsonSerializerSettings = jsonSerializerSettings;
        }
        #endregion

        #region [ Methods ]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Configurations
                    .Add(new ENavigareDocumentSeriesItemMap());
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
            }

        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                int changes = await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbv_ex)
            {
                EvaluateExeption(_logger, LogCategories, _jsonSerializerSettings, "SaveChanges", dbv_ex);
            }
            catch (DbUpdateConcurrencyException dbc_ex)
            {
                EvaluateExeption(_logger, LogCategories, "SaveChanges", dbc_ex);
            }
            catch (DbUpdateException dbu_ex)
            {
                EvaluateExeption(_logger, LogCategories, "SaveChanges", dbu_ex);
            }
            catch (Exception ex)
            {
                EvaluateExeption(_logger, LogCategories, "SaveChanges", ex);
            }
            return 0;
        }
        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, Exception ex)
        {
            logger.WriteError(new LogMessage(ex.GetType().ToString()), ex, logCategories);
            throw new Exception(string.Concat(methodName, ".Exception -> ", ex.Message), ex);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, UpdateException dbu_ex)
        {
            if (dbu_ex.StateEntries != null)
            {
                foreach (ObjectStateEntry result in dbu_ex.StateEntries.Where(f => f.Entity != null))
                {
                    logger.WriteWarning(new LogMessage(string.Concat("Type: ", result.Entity.GetType().Name, " was part of the problem")), logCategories);
                }
            }

            logger.WriteError(dbu_ex, logCategories);
            throw new Exception(string.Concat(methodName, ".UpdateException -> ", dbu_ex.Message), dbu_ex);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DbUpdateException dbu_ex)
        {
            if (dbu_ex.Entries != null)
            {
                foreach (DbEntityEntry result in dbu_ex.Entries.Where(f => f.Entity != null))
                {
                    logger.WriteWarning(new LogMessage(string.Concat("Type: ", result.Entity.GetType().Name, " was part of the problem")), logCategories);
                }
            }

            if (dbu_ex.InnerException != null && dbu_ex.InnerException is UpdateException)
            {
                EvaluateExeption(logger, logCategories, methodName, dbu_ex.InnerException as UpdateException);
            }
            logger.WriteError(dbu_ex, logCategories);
            throw new Exception(string.Concat(methodName, ".UpdateException -> ", dbu_ex.Message), dbu_ex);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, string methodName, DbUpdateConcurrencyException dbc_ex)
        {
            if (dbc_ex.Entries != null)
            {
                foreach (DbEntityEntry result in dbc_ex.Entries.Where(f => f.Entity != null))
                {
                    logger.WriteWarning(new LogMessage(string.Concat("Type: ", result.Entity.GetType().Name, " was part of the problem")), logCategories);
                }
            }

            logger.WriteError(dbc_ex, logCategories);
            throw new Exception(string.Concat(methodName, ".UpdateConcurrencyException -> ", dbc_ex.Message), dbc_ex);
        }

        private static void EvaluateExeption(ILogger logger, IEnumerable<LogCategory> logCategories, JsonSerializerSettings jsonSerializerSettings, string methodName, DbEntityValidationException dbv_ex)
        {
            foreach (DbEntityValidationResult errors in dbv_ex.EntityValidationErrors)
            {
                if (errors.Entry != null && errors.Entry.Entity != null)
                {
                    logger.WriteWarning(new LogMessage(JsonConvert.SerializeObject(errors.Entry.Entity, Formatting.Indented, jsonSerializerSettings)), logCategories);
                }

                foreach (DbValidationError error in errors.ValidationErrors)
                {
                    logger.WriteWarning(new LogMessage(string.Concat(error.PropertyName, " -> ", error.ErrorMessage)), logCategories);
                }
            }
            logger.WriteError(dbv_ex, logCategories);
            throw new Exception(string.Concat(methodName, ".ValidationException -> ", dbv_ex.Message), dbv_ex);
        }
        #endregion

    }
}
