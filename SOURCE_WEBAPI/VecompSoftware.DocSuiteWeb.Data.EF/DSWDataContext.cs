using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Securities;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Collaborations;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Commons;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Conservations;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Desks;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Dossiers;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Fascicles;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Messages;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Monitors;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.OCharts;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Parameters;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PECMails;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.PosteWeb;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Processes;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Protocols;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Resolutions;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tasks;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Templates;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Tenants;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.UDS;
using VecompSoftware.DocSuiteWeb.Data.EF.Mapping.Workflows;
using VecompSoftware.DocSuiteWeb.Repository.EF.DataContexts;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;

namespace VecompSoftware.DocSuiteWeb.Data.EF
{
    [LogCategory(LogCategoryDefinition.DATACONTEXT)]
    public class DSWDataContext : DataContext, IDSWDataContext
    {
        #region [ Fields ]

        private readonly ILogger _logger;

        #endregion

        #region [ Properties ]

        public string UDSAssemblyFullName { get; set; }

        public string UDSAssemblyFileName { get; set; }

        #endregion

        #region [ Constructor ]
        public DSWDataContext(ILogger logger, ICurrentIdentity currentIdentity)
            : base("DSWDataContext", logger, currentIdentity)
        {
            Database.SetInitializer<DSWDataContext>(null);
            Configuration.ProxyCreationEnabled = false;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        public IQueryable<T> DataSet<T>() where T : class
        {
            return Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
                modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

                #region [ External UDS ]
                AppDomain myDomain = Thread.GetDomain();
                if (File.Exists(UDSAssemblyFileName))
                {
                    Assembly udsExternalAssembly = myDomain.GetAssemblies().SingleOrDefault(f => f.FullName == UDSAssemblyFullName);
                    if (udsExternalAssembly == null)
                    {
                        udsExternalAssembly = Assembly.LoadFile(UDSAssemblyFileName);
                    }
                    modelBuilder.Configurations.AddFromAssembly(udsExternalAssembly);
                    foreach (Type t in udsExternalAssembly.DefinedTypes.Where(f => f.IsDefined(typeof(TableAttribute), false)))
                    {
                        modelBuilder.RegisterEntityType(t);
                    }
                }
                #endregion

                #region [ Collaborations ]
                modelBuilder.Configurations
                    .Add(new CollaborationLogMap())
                    .Add(new CollaborationMap())
                    .Add(new CollaborationAggregateMap())
                    .Add(new CollaborationVersioningMap())
                    .Add(new CollaborationSignMap())
                    .Add(new CollaborationUserMap());

                #endregion

                #region [ Templates ]
                modelBuilder.Configurations
                    .Add(new TemplateCollaborationMap())
                    .Add(new TemplateCollaborationDocumentRepositoryMap())
                    .Add(new TemplateCollaborationUserMap())
                    .Add(new TemplateReportMap());
                #endregion

                #region [ Commons ]

                modelBuilder.Configurations
                    .Add(new LocationMap())
                    .Add(new RoleMap())
                    .Add(new CategoryMap())
                    .Add(new ContactMap())
                    .Add(new ContactPlaceNameMap())
                    .Add(new ContactTitleMap())
                    .Add(new SecurityGroupMap())
                    .Add(new SecurityUserMap())
                    .Add(new ContainerMap())
                    .Add(new ContainerGroupMap())
                    .Add(new ContainerPropertyMap())
                    .Add(new RoleGroupMap())
                    .Add(new RoleUserMap())
                    .Add(new CategoryFascicleMap())
                    .Add(new CategoryFascicleRightMap())
                    .Add(new IncrementalMap())
                    .Add(new CategorySchemaMap())
                    .Add(new TableLogMap())
                    .Add(new MetadataRepositoryMap())
                    .Add(new ParameterEnvMap())
                    .Add(new PrivacyLevelMap())
                    .Add(new UserLogMap())
                    .Add(new MetadataValueMap())
                    .Add(new MetadataValueContactMap());
                #endregion

                #region [ Dossiers ]
                modelBuilder.Configurations
                    .Add(new DossierMap())
                    .Add(new DossierLogMap())
                    .Add(new DossierDocumentMap())
                    .Add(new DossierRoleMap())
                    .Add(new DossierCommentMap())
                    .Add(new DossierFolderMap())
                    .Add(new DossierFolderRoleMap())
                    .Add(new DossierLinkMap());
                #endregion

                #region [ Desks ]
                modelBuilder.Configurations
                    .Add(new DeskCollaborationMap())
                    .Add(new DeskDocumentEndorsementMap())
                    .Add(new DeskDocumentMap())
                    .Add(new DeskDocumentVersionMap())
                    .Add(new DeskLogMap())
                    .Add(new DeskMap())
                    .Add(new DeskMessageMap())
                    .Add(new DeskRoleUsersMap())
                    .Add(new DeskStoryBoardMap());
                #endregion

                #region [ DocumentArchives ]
                modelBuilder.Configurations
                    .Add(new DocumentSeriesMap())
                    .Add(new DocumentSeriesItemMap())
                    .Add(new DocumentSeriesItemRoleMap())
                    .Add(new DocumentSeriesItemLogMap())
                    .Add(new DocumentSeriesConstraintMap())
                    .Add(new DocumentSeriesItemLinkMap());
                #endregion

                #region [ DocumentUnits ]
                modelBuilder.Configurations
                    .Add(new DocumentUnitMap())
                    .Add(new DocumentUnitRoleMap())
                    .Add(new DocumentUnitChainMap())
                    .Add(new DocumentUnitFascicleHistoricizedCategoryMap())
                    .Add(new DocumentUnitFascicleCategoryMap())
                    .Add(new DocumentUnitUserMap());
                #endregion

                #region [ Fascicles ]
                modelBuilder.Configurations
                    .Add(new FascicleMap())
                    .Add(new FasciclePeriodMap())
                    .Add(new FascicleLogMap())
                    .Add(new FascicleLinkMap())
                    .Add(new FascicleDocumentMap())
                    .Add(new FascicleRoleMap())
                    .Add(new FascicleFolderMap())
                    .Add(new FascicleDocumentUnitMap());
                #endregion

                #region [ Messages ]
                modelBuilder.Configurations
                    .Add(new MessageAttachmentMap())
                    .Add(new MessageContactEmailMap())
                    .Add(new MessageContactMap())
                    .Add(new MessageEmailMap())
                    .Add(new MessageLogMap())
                    .Add(new MessageMap());
                #endregion

                #region[ OCharts ]
                modelBuilder.Configurations
                    .Add(new OChartMap())
                    .Add(new OChartItemMap())
                    .Add(new OChartItemContainerMap());
                #endregion

                #region[ PECMails ]
                modelBuilder.Configurations
                    .Add(new PECMailMap())
                    .Add(new PECMailBoxMap())
                    .Add(new PECMailLogMap())
                    .Add(new PECMailReceiptMap())
                    .Add(new PECMailAttachmentMap())
                    .Add(new PECMailBoxConfigurationMap());
                #endregion

                #region [ Protocols ]
                modelBuilder.Configurations
                    .Add(new ProtocolTypeMap())
                    .Add(new ProtocolMap())
                    .Add(new ProtocolRoleMap())
                    .Add(new ProtocolRoleUserMap())
                    .Add(new ProtocolLogMap())
                    .Add(new ProtocolContactMap())
                    .Add(new ProtocolParerMap())
                    .Add(new ProtocolLinkMap())
                    .Add(new ProtocolDocumentTypeMap())
                    .Add(new ProtocolUserMap())
                    .Add(new ProtocolContactManualMap())
                    .Add(new ProtocolDraftMap())
                    .Add(new AdvancedProtocolMap()); ;

                #endregion

                #region [ TemplateDocumentRepositories ]
                modelBuilder.Configurations
                    .Add(new TemplateDocumentRepositoryMap());
                #endregion

                #region [ Resolutions ]
                modelBuilder.Configurations
                    .Add(new ResolutionMap())
                    .Add(new ResolutionRoleMap())
                    .Add(new ResolutionContactMap())
                    .Add(new FileResolutionMap())
                    .Add(new ResolutionKindMap())
                    .Add(new ResolutionKindDocumentSeriesMap())
                    .Add(new ResolutionDocumentSeriesItemMap())
                    .Add(new ResolutionLogMap());
                #endregion

                #region [ Workflow ]
                modelBuilder.Configurations
                    .Add(new WorkflowActivityMap())
                    .Add(new WorkflowActivityLogMap())
                    .Add(new WorkflowInstanceMap())
                    .Add(new WorkflowPropertyMap())
                    .Add(new WorkflowRepositoryMap())
                    .Add(new WorkflowAuthorizationMap())
                    .Add(new WorkflowRoleMappingMap())
                    .Add(new WorkflowInstanceLogMap())
                    .Add(new WorkflowInstanceRoleMap())
                    .Add(new WorkflowEvaluationPropertyMap());
                #endregion

                #region [ UDS ]

                modelBuilder.Configurations
                    .Add(new UDSRepositoryMap())
                    .Add(new UDSSchemaRepositoryMap())
                    .Add(new UDSTypologyMap())
                    .Add(new UDSLogMap())
                    .Add(new UDSRoleMap())
                    .Add(new UDSUserMap())
                    .Add(new UDSContactMap())
                    .Add(new UDSMessageMap())
                    .Add(new UDSDocumentUnitMap())
                    .Add(new UDSPECMailMap())
                    .Add(new UDSCollaborationMap());
                #endregion

                #region [ MassimariScarto ]
                modelBuilder.Configurations
                    .Add(new MassimarioScartoMap());
                #endregion

                #region [ Conservation ]
                modelBuilder.Configurations
                    .Add(new ConservationMap());
                #endregion

                #region [ Monitors ]
                modelBuilder.Configurations
                    .Add(new TransparentAdministrationMonitorLogMap());
                #endregion

                #region [ Parameter ]
                modelBuilder.Configurations
                    .Add(new ParameterMap());
                #endregion

                #region [ PosteOnLine ]
                modelBuilder.Configurations
                    .Add(new PosteOnLineRequestMap())
                    .Add(new LOLRequestMap())
                    .Add(new TOLRequestMap())
                    .Add(new ROLRequestMap())
                    .Add(new SOLRequestMap());
                
                #endregion

                #region [ JeepServiceHosts ]
                modelBuilder.Configurations
                    .Add(new JeepServiceHostMap());
                #endregion

                #region[ Tenants ]
                modelBuilder.Configurations
                    .Add(new TenantMap())
                    .Add(new TenantConfigurationMap())
                    .Add(new TenantWorkflowRepositoryMap())
                    .Add(new TenantAOOMap());
                #endregion

                #region [ Processes ]

                modelBuilder.Configurations
                    .Add(new ProcessMap())
                    .Add(new ProcessFascicleTemplateMap())
                    .Add(new ProcessFascicleWorkflowRepositoryMap());

                #endregion

                #region [ Tasks ]
                modelBuilder.Configurations
                    .Add(new TaskHeaderMap())
                    .Add(new TaskHeaderProtocolMap());
                #endregion
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("DSW DataContext - unexpected exception was thrown while invoking OnModelCreating: ", ex.Message), ex, DSWExceptionCode.DB_ModelMappingError);
            }
        }

        public IEnumerable<TModel> ExecuteModelFunction<TModel>(string functionName, params IQueryParameter[] parameters)
        {
            string sql = string.Format("SELECT * FROM {0}({1})", functionName, string.Join(", ", parameters.Select(s => s.ParameterName)));
            IEnumerable<TModel> results = Database.SqlQuery<TModel>(sql, parameters.Select(s => new SqlParameter(s.ParameterName, s.ParameterValue)).ToArray())
                .ToList();
            return results;
        }
        #endregion

    }
}
