using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using System;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Customs.AUSL_RE.BandiDiGara;
using VecompSoftware.DocSuite.Public.Core.Models.Domains;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Archives.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.PECMails;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.Resolutions;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using VecompSoftware.DocSuite.Public.WebAPI.Handlers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using FascicleModel = VecompSoftware.DocSuite.Public.Core.Models.Domains.Fascicles.FascicleModel;

namespace VecompSoftware.DocSuite.Public.WebAPI
{
    public static class ODataConfig
    {
        public const string ODATA_FINDER_PARAMETER = "finder";
        public const string ODATA_INPUT_PARAMETER = "input";

        public static void Register(HttpConfiguration config)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            MapCommonOData(builder);

            MapProtocolOData(builder);

            MapResolutionOData(builder);

            MapPECMailOData(builder);

            MapFascilceOData(builder);

            MapWorkflowOData(builder);

            MapDocumentUnitReferenceOData(builder);

            MapDossierOData(builder);

            MapCustomModules(builder);

            config.AddODataQueryFilter();
            config.Filter(QueryOptionSetting.Allowed);
            config.Expand(QueryOptionSetting.Allowed);
            config.Select(QueryOptionSetting.Allowed);
            config.OrderBy(QueryOptionSetting.Allowed);
            config.Count(QueryOptionSetting.Allowed);
            config.MaxTop(100);
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: "odata",
                model: builder.GetEdmModel(),
                batchHandler: new WebAPIDataBatchHandler(GlobalConfiguration.DefaultServer));

        }

        private static void MapProtocolOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<ProtocolModel>("Protocols")
               .EntityType.HasKey(p => p.Id);

            builder
              .EntitySet<ProtocolModel>("ProtocolAuthorized")
              .EntityType.HasKey(p => p.Id);

            builder
               .EntitySet<ProtocolContactModel>("ProtocolContacts")
               .EntityType.HasKey(p => p.Id);

            builder
               .EntitySet<ProtocolSectorModel>("ProtocolSectors")
               .EntityType.HasKey(p => p.Id);

            builder
             .EntitySet<ProtocolUserModel>("ProtocolUsers")
             .EntityType.HasKey(p => p.Id);

            #region [ Functions ]
            FunctionConfiguration getProtocolSummaryFunc = builder
                  .EntityType<ProtocolModel>().Collection
                  .Function("GetProtocolSummary");

            getProtocolSummaryFunc.Namespace = "ProtocolService";
            getProtocolSummaryFunc.ReturnsCollectionFromEntitySet<ProtocolModel>("Protocols");
            getProtocolSummaryFunc.Parameter<short>("year");
            getProtocolSummaryFunc.Parameter<int>("number");

            FunctionConfiguration getOutgoingPECCount = builder
                  .EntityType<ProtocolModel>().Collection
                  .Function("GetProtocolOutgoingPECCount")
                  .Returns<int>();

            getOutgoingPECCount.Namespace = "ProtocolService";
            getOutgoingPECCount.Parameter<short>("year");
            getOutgoingPECCount.Parameter<int>("number");

            FunctionConfiguration getIngoingPECCount = builder
                  .EntityType<ProtocolModel>().Collection
                  .Function("GetProtocolIngoingPECCount")
                  .Returns<int>();

            getIngoingPECCount.Namespace = "ProtocolService";
            getIngoingPECCount.Parameter<short>("year");
            getIngoingPECCount.Parameter<int>("number");

            FunctionConfiguration getProtocolSummaryNoAuthFunc = builder
                  .EntityType<ProtocolModel>().Collection
                  .Function("GetProtocolSummary");

            getProtocolSummaryNoAuthFunc.Namespace = "ProtocolAuthorizedService";
            getProtocolSummaryNoAuthFunc.ReturnsCollectionFromEntitySet<ProtocolModel>("Protocols");
            getProtocolSummaryNoAuthFunc.Parameter<Guid>("id");

            FunctionConfiguration getOutgoingPECCountNoAuth = builder
                  .EntityType<ProtocolModel>().Collection
                  .Function("GetProtocolOutgoingPECCount")
                  .Returns<int>();

            getOutgoingPECCountNoAuth.Namespace = "ProtocolAuthorizedService";
            getOutgoingPECCountNoAuth.Parameter<short>("year");
            getOutgoingPECCountNoAuth.Parameter<int>("number");

            FunctionConfiguration getIngoingPECCountNoAuth = builder
                  .EntityType<ProtocolModel>().Collection
                  .Function("GetProtocolIngoingPECCount")
                  .Returns<int>();

            getIngoingPECCountNoAuth.Namespace = "ProtocolAuthorizedService";
            getIngoingPECCountNoAuth.Parameter<short>("year");
            getIngoingPECCountNoAuth.Parameter<int>("number");

            FunctionConfiguration getUserAuthorized = builder
                 .EntityType<ProtocolModel>().Collection
                 .Function("GetUserAuthorizedProtocols")
                 .ReturnsCollectionFromEntitySet<ProtocolModel>("Protocol");

            getUserAuthorized.Namespace = "ProtocolAuthorizedService";
            getUserAuthorized.Parameter<int>("skip");
            getUserAuthorized.Parameter<int>("top");
            getUserAuthorized.Parameter<string>("subject");
            getUserAuthorized.Parameter<DateTimeOffset?>("dateFrom");
            getUserAuthorized.Parameter<DateTimeOffset?>("dateTo");
            getUserAuthorized.Parameter<string>("contact");

            FunctionConfiguration countUserAuthorized = builder
                .EntityType<ProtocolModel>().Collection
                .Function("GetUserAuthorizedProtocolsCount")
                .Returns<int>();
            countUserAuthorized.Namespace = "ProtocolAuthorizedService";
            countUserAuthorized.Parameter<string>("subject");
            countUserAuthorized.Parameter<DateTimeOffset?>("dateFrom");
            countUserAuthorized.Parameter<DateTimeOffset?>("dateTo");
            countUserAuthorized.Parameter<string>("contact");

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapResolutionOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<ResolutionModel>("Resolution")
               .EntityType.HasKey(p => p.Id);

            #region [ Functions ]
            FunctionConfiguration getExecutiveResolutionsFunc = builder
                  .EntityType<ResolutionModel>().Collection
                  .Function("GetExecutiveResolutions");

            getExecutiveResolutionsFunc.Namespace = "ResolutionService";
            getExecutiveResolutionsFunc.ReturnsCollectionFromEntitySet<ResolutionModel>("Resolution");
            getExecutiveResolutionsFunc.Parameter<int>("skip");
            getExecutiveResolutionsFunc.Parameter<int>("top");
            getExecutiveResolutionsFunc.Parameter<ResolutionType>("type");
            getExecutiveResolutionsFunc.Parameter<short?>("year");
            getExecutiveResolutionsFunc.Parameter<int?>("number");
            getExecutiveResolutionsFunc.Parameter<string>("subject");
            getExecutiveResolutionsFunc.Parameter<string>("adoptionDate");
            getExecutiveResolutionsFunc.Parameter<string>("proposer");

            FunctionConfiguration getPublishedResolutionsFunc = builder
                  .EntityType<ResolutionModel>().Collection
                  .Function("GetPublishedResolutions");

            getPublishedResolutionsFunc.Namespace = "ResolutionService";
            getPublishedResolutionsFunc.ReturnsCollectionFromEntitySet<ResolutionModel>("Resolution");
            getPublishedResolutionsFunc.Parameter<int>("skip");
            getPublishedResolutionsFunc.Parameter<int>("top");
            getPublishedResolutionsFunc.Parameter<ResolutionType>("type");
            getPublishedResolutionsFunc.Parameter<short?>("year");
            getPublishedResolutionsFunc.Parameter<int?>("number");
            getPublishedResolutionsFunc.Parameter<string>("subject");
            getPublishedResolutionsFunc.Parameter<string>("adoptionDate");
            getPublishedResolutionsFunc.Parameter<string>("proposer");

            FunctionConfiguration getExecutiveReslCount = builder
                  .EntityType<ResolutionModel>().Collection
                  .Function("GetExecutiveResolutionsCount")
                  .Returns<int>();

            getExecutiveReslCount.Namespace = "ResolutionService";
            getExecutiveReslCount.Parameter<ResolutionType>("type");
            getExecutiveReslCount.Parameter<short?>("year");
            getExecutiveReslCount.Parameter<string>("subject");
            getExecutiveReslCount.Parameter<int?>("number");
            getExecutiveReslCount.Parameter<string>("adoptionDate");
            getExecutiveReslCount.Parameter<string>("proposer");

            FunctionConfiguration getPublishedReslCount = builder
                  .EntityType<ResolutionModel>().Collection
                  .Function("GetPublishedResolutionsCount")
                  .Returns<int>();

            getPublishedReslCount.Namespace = "ResolutionService";
            getPublishedReslCount.Parameter<ResolutionType>("type");
            getPublishedReslCount.Parameter<short?>("year");
            getPublishedReslCount.Parameter<int?>("number");
            getPublishedReslCount.Parameter<string>("subject");
            getPublishedReslCount.Parameter<string>("adoptionDate");
            getPublishedReslCount.Parameter<string>("proposer");

            FunctionConfiguration getOnlineResolutionsFunc = builder
                 .EntityType<ResolutionModel>().Collection
                 .Function("GetOnlinePublishedResolutions");

            getOnlineResolutionsFunc.Namespace = "ResolutionService";
            getOnlineResolutionsFunc.ReturnsCollectionFromEntitySet<ResolutionModel>("Resolution");
            getOnlineResolutionsFunc.Parameter<ResolutionType>("type");


            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapPECMailOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<PECMailModel>("PECMails")
               .EntityType.HasKey(p => p.Id);


            #region [ Functions ]
            FunctionConfiguration getPECMailsFunc = builder
                  .EntityType<PECMailModel>().Collection
                  .Function("GetProtocolPECMails");

            getPECMailsFunc.Namespace = "PECMailService";
            getPECMailsFunc.ReturnsCollectionFromEntitySet<PECMailModel>("PECMails");
            getPECMailsFunc.Parameter<short>("year");
            getPECMailsFunc.Parameter<int>("number");
            getPECMailsFunc.Parameter<short>("direction");

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapCommonOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<DocumentModel>("Documents")
                .EntityType.HasKey(p => p.Id);

            builder
              .EntitySet<Core.Models.Workflows.Parameters.MetadataModel>("MetadataModel")
              .EntityType.HasKey(p => new { p.KeyName, p.Value });

            builder
                .EntitySet<CategoryModel>("Categories")
                .EntityType.HasKey(p => p.Id);

            builder
                .EntitySet<ContainerModel>("Containers")
                .EntityType.HasKey(p => p.Id);

            builder
             .EntitySet<GenericDocumentUnitModel>("GenericDocumentUnits")
             .EntityType.HasKey(p => p.Id);

            #region [ Functions ]

            FunctionConfiguration getProtocolDocumentsFunc = builder
                  .EntityType<DocumentModel>().Collection
                  .Function("GetDocuments");
            getProtocolDocumentsFunc.Namespace = "DocumentUnitService";
            getProtocolDocumentsFunc.ReturnsCollectionFromEntitySet<DocumentModel>("DocumentUnits");
            getProtocolDocumentsFunc.Parameter<Guid>("uniqueId");
            getProtocolDocumentsFunc.Parameter<Guid?>("workflowArchiveChainId");

            FunctionConfiguration getActivityDocuments = builder
                .EntityType<DocumentModel>().Collection
                .Function("GetDocumentsByArchiveChain");
            getActivityDocuments.Namespace = "DocumentUnitService";
            getActivityDocuments.ReturnsCollectionFromEntitySet<DocumentModel>("DocumentUnits");
            getActivityDocuments.Parameter<Guid>("idArchiveChain");
            #endregion
        }

        private static void MapFascilceOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<FascicleModel>("Fascicles")
               .EntityType.HasKey(p => p.Id);

            builder
              .EntitySet<FascicleContactModel>("FascicleContacts")
              .EntityType.HasKey(p => p.Id);

            builder
              .EntitySet<GenericDocumentUnitModel>("FascicleDocumentUnits")
              .EntityType.HasKey(p => p.Id);

            builder
               .EntitySet<FascicleFolderTableValuedModel>("FascicleFolders")
               .EntityType.HasKey(p => p.IdFascicleFolder);

            #region [ Functions ]
            FunctionConfiguration getFascicleSummary = builder
                  .EntityType<FascicleModel>().Collection
                  .Function("GetFascicleSummary");

            getFascicleSummary.Namespace = "FascicleService";
            getFascicleSummary.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFascicleSummary.Parameter<Guid>("uniqueId");

            FunctionConfiguration getFascicleSummaryByYearAndNumber = builder
                  .EntityType<FascicleModel>().Collection
                  .Function("GetFascicleSummaryByYearAndNumber");

            getFascicleSummaryByYearAndNumber.Namespace = "FascicleService";
            getFascicleSummaryByYearAndNumber.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFascicleSummaryByYearAndNumber.Parameter<short>("year");
            getFascicleSummaryByYearAndNumber.Parameter<int>("number");
            getFascicleSummaryByYearAndNumber.Parameter<short>("code");

            FunctionConfiguration getFascicleSummaryByTitle = builder
                  .EntityType<FascicleModel>().Collection
                  .Function("GetFascicleSummaryByTitle");

            getFascicleSummaryByTitle.Namespace = "FascicleService";
            getFascicleSummaryByTitle.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFascicleSummaryByTitle.Parameter<string>("title");

            FunctionConfiguration getFascicleDocumentUnits = builder
                   .EntityType<FascicleModel>().Collection
                   .Function("GetFascicleDocumentUnits");

            getFascicleDocumentUnits.Namespace = "FascicleService";
            getFascicleDocumentUnits.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFascicleDocumentUnits.Parameter<Guid>("uniqueId");
            getFascicleDocumentUnits.Parameter<string>("filter");

            FunctionConfiguration getFascicleDocuments = builder
                 .EntityType<FascicleModel>().Collection
                 .Function("GetFascicleDocuments");

            getFascicleDocuments.Namespace = "FascicleService";
            getFascicleDocuments.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFascicleDocuments.Parameter<Guid>("uniqueId");

            FunctionConfiguration getNextFascicleFolders = builder
                  .EntityType<FascicleFolderTableValuedModel>().Collection
                  .Function("GetNextFascicleFolders");
            getNextFascicleFolders.Namespace = "FascicleService";
            getNextFascicleFolders.Parameter<Guid>("id");
            getNextFascicleFolders.ReturnsCollectionFromEntitySet<FascicleFolderTableValuedModel>("FascicleFolders");

            FunctionConfiguration getFascicleDocumentUnitFromFolder = builder
                  .EntityType<FascicleFolderTableValuedModel>().Collection
                  .Function("GetFascicleDocumentUnitFromFolder");
            getFascicleDocumentUnitFromFolder.Namespace = "FascicleService";
            getFascicleDocumentUnitFromFolder.Parameter<Guid>("id");
            getFascicleDocumentUnitFromFolder.ReturnsCollection<GenericDocumentUnitModel>();

            FunctionConfiguration getFascicleDocumentFromFolder = builder
                  .EntityType<FascicleFolderTableValuedModel>().Collection
                  .Function("GetFascicleDocumentFromFolder");
            getFascicleDocumentFromFolder.Namespace = "FascicleService";
            getFascicleDocumentFromFolder.Parameter<Guid>("id");
            getFascicleDocumentFromFolder.ReturnsCollection<FascicleDocumentModel>();

            FunctionConfiguration getFascicleFlatDocuments = builder
                 .EntityType<FascicleModel>().Collection
                 .Function("GetFascicleFlatDocuments");

            getFascicleFlatDocuments.Namespace = "FascicleService";
            getFascicleFlatDocuments.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFascicleFlatDocuments.Parameter<Guid>("uniqueId");

            FunctionConfiguration getFasciclesByMetadataIdentifier = builder
                 .EntityType<FascicleModel>().Collection
                 .Function("GetFasciclesByMetadataIdentifier");

            getFasciclesByMetadataIdentifier.Namespace = "FascicleService";
            getFasciclesByMetadataIdentifier.ReturnsCollectionFromEntitySet<FascicleModel>("Fascicles");
            getFasciclesByMetadataIdentifier.Parameter<string>("name");
            getFasciclesByMetadataIdentifier.Parameter<string>("identifier");

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapWorkflowOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<WorkflowStatusModel>("Workflows")
               .EntityType.HasKey(p => p.Id);
            builder
                .EntitySet<WorkflowActivityModel>("WorkflowActivities")
                .EntityType.HasKey(p => p.IdWorkflowActivity);

            #region [ Functions ]
            FunctionConfiguration myInstances = builder
                  .EntityType<WorkflowStatusModel>().Collection
                  .Function("MyInstances");

            myInstances.Namespace = "WorkflowService";
            myInstances.ReturnsCollectionFromEntitySet<WorkflowStatusModel>("Workflows");
            myInstances.Parameter<string>("workflowName");

            FunctionConfiguration myActivities = builder
                .EntityType<WorkflowActivityModel>().Collection
                .Function("MyActivities");
            myActivities.Namespace = "WorkflowService";
            myActivities.ReturnsCollectionFromEntitySet<WorkflowActivityModel>("WorkflowActivities");
            myActivities.Parameter<WorkflowActivityFinderModel>("finder");

            FunctionConfiguration getLastWorkflowActivityFromDocumentUnit = builder
                .EntityType<WorkflowActivityModel>().Collection
                .Function("GetLastWorkflowActivityFromDocumentUnit");
            getLastWorkflowActivityFromDocumentUnit.Namespace = "WorkflowService";
            getLastWorkflowActivityFromDocumentUnit.ReturnsFromEntitySet<WorkflowActivityModel>("WorkflowActivities");
            getLastWorkflowActivityFromDocumentUnit.Parameter<Guid>("idDocumentUnit");

            FunctionConfiguration currentWorkflowActivityFromDocumentUnit = builder
                .EntityType<WorkflowActivityModel>().Collection
                .Function("CurrentWorkflowActivityFromDocumentUnit");
            currentWorkflowActivityFromDocumentUnit.Namespace = "WorkflowService";
            currentWorkflowActivityFromDocumentUnit.ReturnsFromEntitySet<WorkflowActivityModel>("WorkflowActivities");
            currentWorkflowActivityFromDocumentUnit.Parameter<Guid>("idDocumentUnit");

            FunctionConfiguration getUserAuthorizedWorkflowActivitiesCount = builder
                .EntityType<WorkflowActivityModel>().Collection
                .Function("CountUserAuthorizedWorkflowActivities");
            getUserAuthorizedWorkflowActivitiesCount.Namespace = "WorkflowService";
            getUserAuthorizedWorkflowActivitiesCount.Returns<long>();
            getUserAuthorizedWorkflowActivitiesCount.Parameter<WorkflowActivityFinderModel>("finder");

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapDocumentUnitReferenceOData(ODataModelBuilder builder)
        {
            builder
             .EntitySet<DocumentUnitReferenceModel>("DocumentUnitReferences")
             .EntityType.HasKey(p => p.Id);

            #region [ Functions ]

            FunctionConfiguration getByContacts = builder
                  .EntityType<DocumentUnitReferenceModel>().Collection
                  .Function("ProtocolByContacts")
                  .ReturnsCollectionFromEntitySet<DocumentUnitReferenceModel>("DocumentUnitReferences");

            getByContacts.Namespace = "FinderService";
            getByContacts.Parameter<string>("searchCode");
            getByContacts.Parameter<DateTimeOffset?>("dateFrom");
            getByContacts.Parameter<DateTimeOffset?>("dateTo");

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapDossierOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<DossierModel>("Dossiers")
               .EntityType.HasKey(p => p.UniqueId);
            builder
               .EntitySet<DossierFolderTableValuedModel>("DossierFolders")
               .EntityType.HasKey(p => p.IdDossierFolder);

            #region [ Functions ]

            FunctionConfiguration getDossierById = builder
                  .EntityType<DossierModel>().Collection
                  .Function("GetDossierById");
            getDossierById.Namespace = "DossierService";
            getDossierById.Parameter<Guid>("id");
            getDossierById.ReturnsCollectionFromEntitySet<DossierModel>("Dossiers");

            FunctionConfiguration getDossierByYearAndNumber = builder
                  .EntityType<DossierModel>().Collection
                  .Function("GetDossierByYearAndNumber");
            getDossierByYearAndNumber.Namespace = "DossierService";
            getDossierByYearAndNumber.Parameter<short>("year");
            getDossierByYearAndNumber.Parameter<int>("number");
            getDossierByYearAndNumber.ReturnsCollectionFromEntitySet<DossierModel>("Dossiers");

            FunctionConfiguration getNextDossierFolders = builder
                  .EntityType<DossierFolderTableValuedModel>().Collection
                  .Function("GetNextDossierFolders");
            getNextDossierFolders.Namespace = "DossierService";
            getNextDossierFolders.Parameter<Guid>("id");
            getNextDossierFolders.ReturnsCollectionFromEntitySet<DossierFolderTableValuedModel>("DossierFolders");

            FunctionConfiguration hasChildren = builder
                  .EntityType<DossierFolderTableValuedModel>().Collection
                  .Function("HasChildren");
            hasChildren.Namespace = "DossierService";
            hasChildren.Parameter<Guid>("id");
            hasChildren.Returns<bool>();

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapCustomModules(ODataModelBuilder builder)
        {
            builder
               .EntitySet<MenuModel>("AUSLRE_BandiDiGaraMenu")
               .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<ArchiveModel>("AUSLRE_BandiDiGaraArchives")
               .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<MenuModel>("AUSLRE_CommittenteMenu")
               .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<ArchiveModel>("AUSLRE_CommittenteArchives")
               .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            #region [BandiDiGara]
            FunctionConfiguration getMenu = builder
                 .EntityType<MenuModel>().Collection
                 .Function("GetMenu");

            getMenu.Namespace = "MenuModelService";
            getMenu.ReturnsCollectionFromEntitySet<MenuModel>("AUSLRE_BandiDiGaraMenu");

            ActionConfiguration countArchiveByGrid = builder
                .EntityType<ArchiveModel>().Collection
                .Action("CountArchiveByGrid");

            countArchiveByGrid.Namespace = "ArchiveModelService";
            countArchiveByGrid.ReturnsCollectionFromEntitySet<ArchiveModel>("AUSLRE_BandiDiGaraArchives");
            countArchiveByGrid.Parameter<ArchiveFinderModel>("finder");

            ActionConfiguration searchArchiveByGrid = builder
                .EntityType<ArchiveModel>().Collection
                .Action("SearchArchiveByGrid");

            searchArchiveByGrid.Namespace = "ArchiveModelService";
            searchArchiveByGrid.ReturnsCollectionFromEntitySet<ArchiveModel>("AUSLRE_BandiDiGaraArchives");
            searchArchiveByGrid.Parameter<ArchiveFinderModel>("finder");


            FunctionConfiguration getArchiveInfo = builder
                .EntityType<ArchiveModel>().Collection
                .Function("GetArchiveInfo");

            getArchiveInfo.Namespace = "ArchiveModelService";
            getArchiveInfo.Parameter<Guid>("uniqueId");
            getArchiveInfo.ReturnsCollectionFromEntitySet<ArchiveModel>("AUSLRE_BandiDiGaraArchives");

            #endregion

            #region [Commitente]
            FunctionConfiguration getMenuCommittente = builder
                 .EntityType<MenuModel>().Collection
                 .Function("GetMenu");

            getMenuCommittente.Namespace = "MenuModelService";
            getMenuCommittente.ReturnsCollectionFromEntitySet<MenuModel>("AUSLRE_CommittenteMenu");

            ActionConfiguration countArchiveByGridCommittente = builder
                .EntityType<ArchiveModel>().Collection
                .Action("CountArchiveByGrid");

            countArchiveByGridCommittente.Namespace = "ArchiveModelService";
            countArchiveByGridCommittente.ReturnsCollectionFromEntitySet<ArchiveModel>("AUSLRE_CommittenteArchives");
            countArchiveByGridCommittente.Parameter<ArchiveFinderModel>("finder");

            ActionConfiguration searchArchiveByGridCommittente = builder
                .EntityType<ArchiveModel>().Collection
                .Action("SearchArchiveByGrid");

            searchArchiveByGridCommittente.Namespace = "ArchiveModelService";
            searchArchiveByGridCommittente.ReturnsCollectionFromEntitySet<ArchiveModel>("AUSLRE_CommittenteArchives");
            searchArchiveByGridCommittente.Parameter<ArchiveFinderModel>("finder");


            FunctionConfiguration getArchiveInfoCommittente = builder
                .EntityType<ArchiveModel>().Collection
                .Function("GetArchiveInfo");

            getArchiveInfoCommittente.Namespace = "ArchiveModelService";
            getArchiveInfoCommittente.Parameter<Guid>("uniqueId");
            getArchiveInfoCommittente.ReturnsCollectionFromEntitySet<ArchiveModel>("AUSLRE_CommittenteArchives");
            #endregion


            #endregion

            #region [ Navigation Properties ]

            #endregion
        }
    }
}
