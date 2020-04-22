using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Configurations;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities;
using VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Mappings;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data
{
    [LogCategory(LogCategoryDefinition.DATACONTEXT)]
    [DbConfigurationType(typeof(NavisionDbConfiguration))]
    public class NavisionDbContext : DbContext
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(NavisionDbContext));
                }
                return _logCategories;
            }
        }

        public DbSet<WorkflowMetadata> WorkflowMetadata { get; set; }
        public DbSet<WorkflowAttachment> WorkflowAttachment { get; set; }
        #endregion

        #region [ Constructor ]

        public NavisionDbContext(ILogger logger, string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<NavisionDbContext>(null);
            Configuration.ProxyCreationEnabled = false;
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
                modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

                modelBuilder.Configurations
                    .Add(new WorkflowMetadataMap())
                    .Add(new WorkflowAttachmentMap());
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
            }
        }
        #endregion
    }
}
