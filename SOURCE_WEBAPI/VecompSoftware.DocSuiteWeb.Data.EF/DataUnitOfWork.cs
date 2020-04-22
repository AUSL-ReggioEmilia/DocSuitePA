using System;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Repository.EF.UnitOfWorks;

namespace VecompSoftware.DocSuiteWeb.Data.EF
{
    [LogCategory(LogCategoryDefinition.UNITYOFWORK)]
    public class DataUnitOfWork : UnitOfWork, IDataUnitOfWork
    {
        #region [ Fields ]
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]

        public DataUnitOfWork(IDSWDataContext dataContext, ILogger logger)
            : base(dataContext, logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]

        public async Task<bool> SaveAsync()
        {
            try
            {
                bool ret = (await SaveChangesAsync()) > 0;
                Commit();
                return ret;
            }
            catch (DSWException dsw_ex)
            {
                _logger.WriteError(dsw_ex, LogCategories);
                Rollback();
                throw;
            }
            catch (Exception ex)
            {
                _logger.WriteCritical(ex, LogCategories);
                Rollback();
                throw new DSWException("BUG : generic submit", ex, DSWExceptionCode.UW_Anomaly);
            }
        }
        #endregion
    }
}
