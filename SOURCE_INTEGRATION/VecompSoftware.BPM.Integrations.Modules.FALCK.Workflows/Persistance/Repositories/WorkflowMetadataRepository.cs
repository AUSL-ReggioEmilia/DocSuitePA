using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Workflows.Persistance.Repositories
{
    public class WorkflowMetadataRepository : IDisposable
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private NavisionDbContext _navisionDbContext;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WorkflowMetadataRepository));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public WorkflowMetadataRepository(ILogger logger)
        {
            _logger = logger;
            _navisionDbContext = new NavisionDbContext(logger, ModuleConfigurationHelper.GetModuleConfiguration().ConnectionString);
        }
        #endregion

        #region [ Methods ]
        public WorkflowMetadata GetManageableMetadata()
        {
            return _navisionDbContext.WorkflowMetadata
                .Include("WorkflowAttachments")
                .Where(x => x.WorkflowStatus == WorkflowStatusType.WorkflowBootable)
                .FirstOrDefault();
        }

        public WorkflowMetadata GetById(int id)
        {
            return _navisionDbContext.WorkflowMetadata
                .Include("WorkflowAttachments")
                .Where(x => x.MetadataId == id)
                .FirstOrDefault();
        }

        public WorkflowMetadata GetByInstance(Guid instanceId)
        {
            return _navisionDbContext.WorkflowMetadata
                .Include("WorkflowAttachments")
                .Where(x => x.WorkflowInstanceID == instanceId.ToString())
                .FirstOrDefault();
        }
        public WorkflowMetadata GetReferenceDocType(string referenceDocNumber)
        {
            return _navisionDbContext.WorkflowMetadata
               .Where(x => x.DocumentNumber == referenceDocNumber && x.DocumentType == DocumentType.RDA &&
                        x.WorkflowStatus == WorkflowStatusType.WorkflowCompleted && x.IsWorkflowApproved == 1 &&
                        x.IsWorkflowAutoapproval == 1)
                .OrderByDescending(f => f.WorkflowUpdateDate)
               .FirstOrDefault();
        }
        public void Update(WorkflowMetadata entity)
        {
            try
            {
                _navisionDbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                _navisionDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw;
            }
        }

        public void Dispose()
        {
            if (_navisionDbContext != null)
            {
                _navisionDbContext.Dispose();
                _navisionDbContext = null;
            }
        }
        #endregion        
    }
}
