﻿using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using VecompSoftware.Commons.Interfaces.ODATA;
using VecompSoftware.DocSuite.Private.WebAPI.Handlers;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.PosteWeb;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Tenders;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Dossiers;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Entities.Processes;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Entities.Templates;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Model.Reports;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public static class ODataConfig
    {
        private static readonly Type _type_IODATAModelBuilder = typeof(IODATAModelBuilder);
        public const string ODATA_FINDER_PARAMETER = "finder";


        public static void Register(HttpConfiguration config)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            MapCollaborationOData(builder);

            MapDeskOData(builder);

            MapDocumentArchiveOData(builder);

            MapMessageOData(builder);

            MapOChartOData(builder);

            MapWorkflowOData(builder);

            MapProtocolOData(builder);

            MapResolutionOData(builder);

            MapFascicleOData(builder);

            MapPECMailData(builder);

            MapUDSOData(builder);

            MapMassimarioScartoOData(builder);

            MapDocumentUnitsOData(builder);

            MapCommonOData(builder);

            MapSecurityOData(builder);

            MapTemplateCollaborationOData(builder);

            MapTemplateDocumentRepositoryOData(builder);

            MapTemplateReportOData(builder);

            MapDossierOdata(builder);

            MapDossierFolderOdata(builder);

            MapParametersOData(builder);

            MapExternalUDS(builder);

            MapFascicleFolderOdata(builder);

            MapConservationOdata(builder);

            MapTransparentAdministrationMonitorLogsOData(builder);

            MapServiceBusTopicsOData(builder);

            MapJeepServiceHostsOData(builder);

            MapTenantsOData(builder);

            MapReportsOData(builder);

            MapProcessesOData(builder);

            MapPosteWebOData(builder);

            MapTasksOData(builder);

            MapTendersOData(builder);

            MapUserLogOData(builder);

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
                batchHandler: new DSWODataBatchHandler(GlobalConfiguration.DefaultServer));

        }
        private static void AddODATAModel(ICollection<IODATAModel> odataModels, ODataModelBuilder builder)
        {
            foreach (IODATAModel model in odataModels)
            {
                AddEntitySet(model, builder);
                AddODATAModel(model.NavigationProperties, builder);
            }
        }

        private static void AddEntitySet(IODATAModel oDATAModel, ODataModelBuilder builder)
        {
            EntityTypeConfiguration entityTypeConfiguration = new EntityTypeConfiguration(builder, oDATAModel.EntityType);
            entityTypeConfiguration.HasKey(oDATAModel.EntityKey);
            builder.AddEntitySet(oDATAModel.EntityName, entityTypeConfiguration);
        }
        private static void MapExternalUDS(ODataModelBuilder builder)
        {
            AppDomain myDomain = Thread.GetDomain();
            if (File.Exists(WebApiApplication.UDSAssemblyFileName))
            {
                Assembly udsExternalAssembly = myDomain.GetAssemblies().SingleOrDefault(f => f.FullName == WebApiApplication.UDSAssemblyFileName);
                if (udsExternalAssembly == null)
                {
                    udsExternalAssembly = Assembly.LoadFile(WebApiApplication.UDSAssemblyFileName);
                }
                IODATAModelBuilder modelBuilder;

                foreach (Type t in udsExternalAssembly.DefinedTypes.Where(f => _type_IODATAModelBuilder.IsAssignableFrom(f)))
                {
                    modelBuilder = Activator.CreateInstance(t) as IODATAModelBuilder;
                    AddODATAModel(modelBuilder.MapEntityOData(), builder);
                }
            }
        }

        private static void MapCommonOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Category>("Categories")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
                .EntitySet<CategoryFascicle>("CategoryFascicles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<CategoryFascicleRight>("CategoryFascicleRights")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<Contact>("Contacts")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<Container>("Containers")
                .EntityType.HasKey(p => p.EntityShortId);

            #region [ Functions ]

            FunctionConfiguration hasProtocolInsertRight = builder
                .EntityType<Container>().Collection
                .Function("HasProtocolInsertRight");

            hasProtocolInsertRight.Namespace = "ContainerService";
            hasProtocolInsertRight.Returns<bool>();
            hasProtocolInsertRight.Parameter<string>("username");
            hasProtocolInsertRight.Parameter<string>("domain");

            FunctionConfiguration getAuthorizedContainers = builder
                .EntityType<Container>().Collection
                .Function("GetDossierInsertAuthorizedContainers");

            getAuthorizedContainers.Namespace = "ContainerService";
            getAuthorizedContainers.Returns<Container>();
            getAuthorizedContainers.Parameter<Guid>("tenantId");

            FunctionConfiguration getAnyDossierAuthorizedContainers = builder
               .EntityType<Container>().Collection
               .Function("GetAnyDossierAuthorizedContainers");

            getAnyDossierAuthorizedContainers.Namespace = "ContainerService";
            getAnyDossierAuthorizedContainers.Returns<Container>();
            getAnyDossierAuthorizedContainers.Parameter<Guid>("tenantId");

            FunctionConfiguration geAvailablePeriodicCategoryFascicles = builder
                .EntityType<CategoryFascicle>().Collection
                .Function("GeAvailablePeriodicCategoryFascicles");

            geAvailablePeriodicCategoryFascicles.Namespace = "CategoryFascicleService";
            geAvailablePeriodicCategoryFascicles.Returns<CategoryFascicle>();
            geAvailablePeriodicCategoryFascicles.Parameter<short>("idCategory");

            FunctionConfiguration isProcedureSecretary = builder
                .EntityType<CategoryFascicle>().Collection
                .Function("IsProcedureSecretary");

            isProcedureSecretary.Namespace = "CategoryFascicleService";
            isProcedureSecretary.Returns<bool>();
            isProcedureSecretary.Parameter<short>("idCategory");

            FunctionConfiguration findCategory = builder
                .EntityType<Category>().Collection
                .Function("FindCategory");

            findCategory.Namespace = "CategoryService";
            findCategory.Returns<CategoryModel>();
            findCategory.Parameter<short>("idCategory");
            findCategory.Parameter<DocSuiteWeb.Entity.Fascicles.FascicleType?>("fascicleType");

            FunctionConfiguration findCategories = builder
                .EntityType<Category>().Collection
                .Function("FindCategories");

            findCategories.Namespace = "CategoryService";
            findCategories.Returns<CategoryModel>();
            findCategories.Parameter<CategoryFinderModel>("finder");

            FunctionConfiguration findRoles = builder
                .EntityType<Role>().Collection
                .Function("FindRoles");

            findRoles.Namespace = "RoleService";
            findRoles.Returns<RoleModel>();
            findRoles.Parameter<RoleFinderModel>("finder");

            FunctionConfiguration countRoles = builder
             .EntityType<Role>().Collection
             .Function("CountFindRoles");

            countRoles.Namespace = "RoleService";
            countRoles.Returns<int>();
            countRoles.Parameter<RoleFinderModel>("finder");

            FunctionConfiguration getInsertAuthorizedContainers = builder
                .EntityType<Container>().Collection
                .Function("GetInsertAuthorizedContainers");

            getInsertAuthorizedContainers.Namespace = "ContainerService";
            getInsertAuthorizedContainers.Returns<Container>();

            FunctionConfiguration getFascicleInsertAuthorizedContainers = builder
                .EntityType<Container>().Collection
                .Function("GetFascicleInsertAuthorizedContainers");

            getFascicleInsertAuthorizedContainers.Namespace = "ContainerService";
            getFascicleInsertAuthorizedContainers.Returns<Container>();
            getFascicleInsertAuthorizedContainers.Parameter<short?>("idCategory");
            getFascicleInsertAuthorizedContainers.Parameter<DocSuiteWeb.Entity.Fascicles.FascicleType?>("fascicleType");

            FunctionConfiguration findContacts = builder
                .EntityType<Contact>().Collection
                .Function("FindContacts");

            findContacts.Namespace = "ContactService";
            findContacts.Returns<ContactModel>();
            findContacts.Parameter<ContactFinderModel>("finder");

            FunctionConfiguration getContactParents = builder
                .EntityType<Contact>().Collection
                .Function("GetContactParents");

            getContactParents.Namespace = "ContactService";
            getContactParents.Returns<ContactModel>();
            getContactParents.Parameter<int>("idContact");

            FunctionConfiguration getRoleContacts = builder
                .EntityType<Contact>().Collection
                .Function("GetRoleContacts");

            getRoleContacts.Namespace = "ContactService";
            getRoleContacts.Returns<ContactModel>();

            FunctionConfiguration getContactsByParentId = builder
                .EntityType<Contact>().Collection
                .Function("GetContactsByParentId");

            getContactsByParentId.Namespace = "ContactService";
            getContactsByParentId.Returns<ContactModel>();
            getContactsByParentId.Parameter<int>("idContact");
            getContactsByParentId.Parameter<short>("idRole");


            FunctionConfiguration findFascicolableCategory = builder
                .EntityType<Category>().Collection
                .Function("FindFascicolableCategory");

            findFascicolableCategory.Namespace = "CategoryService";
            findFascicolableCategory.Returns<CategoryModel>();
            findFascicolableCategory.Parameter<short>("idCategory");

            FunctionConfiguration hasCategoryFascicleRole = builder
                .EntityType<Role>().Collection
                .Function("HasCategoryFascicleRole");

            hasCategoryFascicleRole.Namespace = "RoleService";
            hasCategoryFascicleRole.Returns<bool>();
            hasCategoryFascicleRole.Parameter<short>("idCategory");

            FunctionConfiguration getRoleUsersFromDossier = builder
                .EntityType<RoleUser>().Collection
                .Function("GetRoleUsersFromDossier");

            getRoleUsersFromDossier.Namespace = "RoleUserService";
            getRoleUsersFromDossier.Returns<RoleUserModel>();
            getRoleUsersFromDossier.Parameter<RoleUserFinderModel>("finder");

            #endregion

            builder
                .EntitySet<ContactPlaceName>("ContactPlaceNames")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
                .EntitySet<ContactTitle>("ContactTitles")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<Location>("Locations")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
                .EntitySet<Role>("Roles")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
                .EntitySet<RoleUser>("RoleUsers")
                .EntityType.HasKey(p => p.EntityId);

            builder
               .EntitySet<ContainerGroup>("ContainerGroups")
               .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<RoleGroup>("RoleGroups")
               .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<SecurityGroup>("SecurityGroups")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<SecurityUser>("SecurityUsers")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<CategorySchema>("CategorySchemas")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<Incremental>("Incrementals")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<TableLog>("TableLogs")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<MetadataRepository>("MetadataRepositories")
                .EntityType.HasKey(p => p.UniqueId);

            builder.
                EntitySet<PrivacyLevel>("PrivacyLevels")
                .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<ContainerProperty>("ContainerProperties")
               .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<MetadataValue>("MetadataValues")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<MetadataValueContact>("MetadataValueContacts")
                .EntityType.HasKey(p => p.UniqueId);
        }

        private static void MapSecurityOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<DomainUserModel>("DomainUsers")
                .EntityType.HasKey(p => p.SDDL_SID);

            builder
                .EntitySet<DomainGroupModel>("DomainGroups")
                .EntityType.HasKey(p => p.SDDL_SID);

            #region [ Functions ]
            FunctionConfiguration getMembersFunc = builder
                   .EntityType<DomainUserModel>().Collection
                   .Function("GetMembers");
            getMembersFunc.Namespace = "DomainUserService";
            getMembersFunc.ReturnsCollectionFromEntitySet<DomainUserModel>("DomainUsers");
            getMembersFunc.Parameter<string>("groupName");

            FunctionConfiguration usersFinderFunc = builder
                   .EntityType<DomainUserModel>().Collection
                   .Function("UsersFinder");
            usersFinderFunc.Namespace = "DomainUserService";
            usersFinderFunc.ReturnsCollectionFromEntitySet<DomainUserModel>("DomainUsers");
            usersFinderFunc.Parameter<string>("text");

            FunctionConfiguration getUserFunc = builder
                    .EntityType<DomainUserModel>().Collection
                    .Function("GetUser");
            getUserFunc.Namespace = "DomainUserService";
            getUserFunc.ReturnsFromEntitySet<DomainUserModel>("DomainUsers");
            getUserFunc.Parameter<string>("username");
            getUserFunc.Parameter<string>("domain");

            FunctionConfiguration groupsFinderFunc = builder
                .EntityType<DomainGroupModel>().Collection
                .Function("GroupsFinder");
            groupsFinderFunc.Namespace = "DomainGroupService";
            groupsFinderFunc.ReturnsCollectionFromEntitySet<DomainGroupModel>("DomainGroups");
            groupsFinderFunc.Parameter<string>("text");

            FunctionConfiguration groupsFromUserFunc = builder
                .EntityType<DomainGroupModel>().Collection
                .Function("GroupsFromUser");
            groupsFromUserFunc.Namespace = "DomainGroupService";
            groupsFromUserFunc.ReturnsCollectionFromEntitySet<DomainGroupModel>("DomainGroups");
            groupsFromUserFunc.Parameter<string>("username");

            FunctionConfiguration getInvalidatingRoleUser = builder
                .EntityType<RoleUser>().Collection
                .Function("GetInvalidatingRoleUser");
            getInvalidatingRoleUser.Namespace = "RoleUserService";
            getInvalidatingRoleUser.ReturnsCollectionFromEntitySet<RoleUser>("RoleUsers");

            FunctionConfiguration getRights = builder
                .EntityType<DomainUserModel>().Collection
                .Function("GetCurrentRights");
            getRights.Namespace = "DomainUserService";
            getRights.ReturnsFromEntitySet<DomainUserModel>("DomainUsers");

            #endregion

        }

        private static void MapDocumentUnitsOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<DocumentUnit>("DocumentUnits")
                .EntityType.HasKey(p => p.UniqueId);


            builder
                .EntitySet<DocumentUnitChain>("DocumentUnitChains")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DocumentUnitContact>("DocumentUnitContacts")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DocumentUnitRole>("DocumentUnitRoles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<DocumentUnitUser>("DocumentUnitUsers")
               .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<DocumentUnitFascicleHistoricizedCategory>("DocumentUnitFascicleHistoricizedCategories")
               .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<DocumentUnitFascicleCategory>("DocumentUnitFascicleCategories")
               .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DocumentModel>("BiblosDocuments")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            FunctionConfiguration getBiblosDocumentContentFunc = builder
                    .EntityType<DocumentModel>().Collection
                    .Function("GetBiblosDocumentContent");

            getBiblosDocumentContentFunc.Namespace = "DocumentUnitService";
            getBiblosDocumentContentFunc.Parameter<Guid>("documentId");
            getBiblosDocumentContentFunc.Returns<byte[]>();

            FunctionConfiguration getBiblosDocumentsFunc = builder
                    .EntityType<DocumentModel>().Collection
                    .Function("GetBiblosDocuments");

            getBiblosDocumentsFunc.Namespace = "DocumentUnitService";
            getBiblosDocumentsFunc.Parameter<Guid>("uniqueId");
            getBiblosDocumentsFunc.Parameter<Guid?>("workflowArchiveChainId");
            getBiblosDocumentsFunc.ReturnsCollectionFromEntitySet<DocumentModel>("Documents");

            FunctionConfiguration getBiblosDocumentsByArchiveChain = builder
                    .EntityType<DocumentModel>().Collection
                    .Function("GetDocumentsByArchiveChain");

            getBiblosDocumentsByArchiveChain.Namespace = "DocumentUnitService";
            getBiblosDocumentsByArchiveChain.Parameter<Guid>("idArchiveChain");

            getBiblosDocumentsByArchiveChain.ReturnsCollectionFromEntitySet<DocumentModel>("Documents");

            FunctionConfiguration fascicolableDocumentUnitsFunc = builder
                   .EntityType<DocumentUnit>().Collection
                   .Function("GetFascicolableDocumentUnits");

            fascicolableDocumentUnitsFunc.Namespace = "DocumentUnitService";
            fascicolableDocumentUnitsFunc.ReturnsCollection<DocumentUnitModel>();
            fascicolableDocumentUnitsFunc.Parameter<string>("dateFrom");
            fascicolableDocumentUnitsFunc.Parameter<string>("dateTo");
            fascicolableDocumentUnitsFunc.Parameter<bool>("includeThreshold");
            fascicolableDocumentUnitsFunc.Parameter<string>("threshold");
            fascicolableDocumentUnitsFunc.Parameter<Guid>("idTenantAOO");
            fascicolableDocumentUnitsFunc.Parameter<bool>("excludeLinked");

            FunctionConfiguration documentUnitsFunc = builder
                .EntityType<DocumentUnit>().Collection
                .Function("FascicleDocumentUnits");
            documentUnitsFunc.Namespace = "DocumentUnitService";
            documentUnitsFunc.ReturnsCollection<DocumentUnitModel>();
            documentUnitsFunc.Parameter<Guid>("idFascicle");
            documentUnitsFunc.Parameter<Guid?>("idFascicleFolder");
            documentUnitsFunc.Parameter<Guid>("idTenantAOO");

            FunctionConfiguration authorizedDocumentUnitsSecurityFunc = builder
                   .EntityType<DocumentUnit>().Collection
                   .Function("AuthorizedDocumentUnits");

            ///TODO: Attenzione, non prendere MAI come riferimento la funzione seguente in quanto la lista dei parametri di filtro
            ///DEVE essere passata sempre tramite la struttura ODATA dei filter. Questa funzione è stata creata per necessità di performance e limiti
            ///di utilizzo.
            authorizedDocumentUnitsSecurityFunc.Namespace = "DocumentUnitService";
            authorizedDocumentUnitsSecurityFunc.ReturnsCollection<DocumentUnitModel>();
            authorizedDocumentUnitsSecurityFunc.Parameter<DocumentUnitFinderModel>("finder");

            FunctionConfiguration countauthorizedDocumentUnitsFunc = builder
                  .EntityType<DocumentUnit>().Collection
                  .Function("CountAuthorizedDocumentUnits");

            countauthorizedDocumentUnitsFunc.Namespace = "DocumentUnitService";
            countauthorizedDocumentUnitsFunc.Returns<int>();
            countauthorizedDocumentUnitsFunc.Parameter<DocumentUnitFinderModel>("finder");

            FunctionConfiguration authorizedDocumentsFunc = builder
                   .EntityType<DocumentUnit>().Collection
                   .Function("GetAuthorizedDocuments");

            authorizedDocumentsFunc.Namespace = "DocumentUnitService";
            authorizedDocumentsFunc.ReturnsCollection<DocumentUnitModel>();
            authorizedDocumentsFunc.Parameter<string>("dateFrom");
            authorizedDocumentsFunc.Parameter<string>("dateTo");
            authorizedDocumentsFunc.Parameter<Guid>("idTenantAOO");

            FunctionConfiguration hasViewableDocumentFunc = builder
                    .EntityType<DocumentUnit>().Collection
                    .Function("HasViewableDocument");

            hasViewableDocumentFunc.Namespace = "DocumentUnitService";
            hasViewableDocumentFunc.Returns<bool>();
            hasViewableDocumentFunc.Parameter<Guid>("idDocumentUnit");
            hasViewableDocumentFunc.Parameter<string>("username");
            hasViewableDocumentFunc.Parameter<string>("domain");

            FunctionConfiguration fullTextSearchDocumentUnitsFunc = builder
                    .EntityType<DocumentUnit>().Collection
                    .Function("FullTextSearchDocumentUnits");

            fullTextSearchDocumentUnitsFunc.Namespace = "DocumentUnitService";
            fullTextSearchDocumentUnitsFunc.ReturnsCollection<DocumentUnitModel>();
            fullTextSearchDocumentUnitsFunc.Parameter<string>("filter");
            fullTextSearchDocumentUnitsFunc.Parameter<Guid>("idTenant");

            #endregion

            #region [ Navigation Properties ]

            builder
                .EntitySet<DocumentUnitRole>("DocumentUnitRoles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DocumentUnitChain>("DocumentUnitChains")
                .EntityType.HasKey(p => p.UniqueId);

            #endregion

        }

        private static void MapPECMailData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<PECMail>("PECMails")
                .EntityType.HasKey(p => p.EntityId);

            #region [ Navigation Properties ]
            builder
                .EntitySet<PECMailBox>("PECMailBoxes")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
                .EntitySet<PECMailLog>("PECMailLogs")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<PECMailReceipt>("PECMailReceipts")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<PECMailAttachment>("PECMailAttachments")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<PECMailBoxConfiguration>("PECMailBoxConfigurations")
                .EntityType.HasKey(p => p.EntityId);

            #endregion
        }

        private static void MapResolutionOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Resolution>("Resolutions")
                .EntityType.HasKey(p => p.EntityId);

            #region [ Functions ]
            #endregion

            #region [ Navigation Properties ]
            builder
                .EntitySet<ResolutionRole>("ResolutionRoles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<ResolutionContact>("ResolutionContacts")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<FileResolution>("FileResolutions")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<ResolutionLog>("ResolutionLogs")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<ResolutionKind>("ResolutionKinds")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<ResolutionKindDocumentSeries>("ResolutionKindDocumentSeriess")
                .EntityType.HasKey(p => p.UniqueId);

            builder
             .EntitySet<ResolutionDocumentSeriesItem>("ResolutionDocumentSeriesItems")
             .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<WebPublication>("WebPublications")
                .EntityType.HasKey(p => p.EntityId);

            #endregion
        }

        private static void MapProtocolOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Protocol>("Protocols")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            FunctionConfiguration authorizedProtocolsFunc = builder
                   .EntityType<Protocol>().Collection
                   .Function("GetAuthorizedProtocols");

            authorizedProtocolsFunc.Namespace = "ProtocolService";
            authorizedProtocolsFunc.Returns<ProtocolModel>();
            authorizedProtocolsFunc.Parameter<string>("username");
            authorizedProtocolsFunc.Parameter<string>("domain");
            authorizedProtocolsFunc.Parameter<string>("dateFrom");
            authorizedProtocolsFunc.Parameter<string>("dateTo");

            FunctionConfiguration getProtocolSummaryFunc = builder
              .EntityType<Protocol>().Collection
              .Function("GetProtocolSummary");

            getProtocolSummaryFunc.Namespace = "ProtocolService";
            getProtocolSummaryFunc.ReturnsFromEntitySet<Protocol>("Protocols");
            getProtocolSummaryFunc.Parameter<Guid>("id");

            FunctionConfiguration getUserAuthorizedProtocols = builder
             .EntityType<Protocol>().Collection
             .Function("GetUserAuthorizedProtocols");

            getUserAuthorizedProtocols.Namespace = "ProtocolService";
            getUserAuthorizedProtocols.ReturnsCollectionFromEntitySet<Protocol>("Protocols");
            getUserAuthorizedProtocols.Parameter<int>("skip");
            getUserAuthorizedProtocols.Parameter<int>("top");
            getUserAuthorizedProtocols.Parameter<string>("subject");
            getUserAuthorizedProtocols.Parameter<DateTimeOffset?>("dateFrom");
            getUserAuthorizedProtocols.Parameter<DateTimeOffset?>("dateTo");
            getUserAuthorizedProtocols.Parameter<string>("contact");


            FunctionConfiguration getUserAuthorizedProtocolsCount = builder
           .EntityType<Protocol>().Collection
           .Function("GetUserAuthorizedProtocolsCount");

            getUserAuthorizedProtocolsCount.Namespace = "ProtocolService";
            getUserAuthorizedProtocolsCount.Returns<long>();
            getUserAuthorizedProtocolsCount.Parameter<string>("subject");
            getUserAuthorizedProtocolsCount.Parameter<DateTimeOffset?>("dateFrom");
            getUserAuthorizedProtocolsCount.Parameter<DateTimeOffset?>("dateTo");
            getUserAuthorizedProtocolsCount.Parameter<string>("contact");

            FunctionConfiguration countProtocolToRead = builder
              .EntityType<Protocol>().Collection
              .Function("CountProtocolToRead");

            countProtocolToRead.Namespace = "ProtocolService";
            countProtocolToRead.Returns<long>();



            FunctionConfiguration getProtocolToRead = builder
           .EntityType<Protocol>().Collection
           .Function("GetProtocolToRead");

            getProtocolToRead.Namespace = "ProtocolService";
            getProtocolToRead.ReturnsCollectionFromEntitySet<Protocol>("Protocols");




            #endregion

            #region [ Navigation Properties ]
            builder
                .EntitySet<ProtocolType>("ProtocolTypes")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
              .EntitySet<ProtocolLog>("ProtocolLogs")
              .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<ProtocolJournal>("ProtocolJournals")
              .EntityType.HasKey(p => p.EntityId);

            builder
              .EntitySet<ProtocolContact>("ProtocolContacts")
                .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<ProtocolLink>("ProtocolLinks")
                .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<ProtocolRole>("ProtocolRoles")
              .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<ProtocolRoleUser>("ProtocolRoleUsers")
              .EntityType.HasKey(p => p.UniqueId);

            builder
             .EntitySet<ProtocolDocumentType>("ProtocolDocumentType")
                .EntityType.HasKey(p => p.EntityShortId);

            builder
                .EntitySet<ProtocolUser>("ProtocolUsers")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<ProtocolContactManual>("ProtocolContactManuals")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<AdvancedProtocol>("AdvancedProtocols")
                .EntityType.HasKey(p => p.UniqueId);

            #endregion
        }

        private static void MapWorkflowOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<WorkflowRepository>("WorkflowRepositories")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration collaborationUsersByTagFunc = builder
                  .EntityType<WorkflowRoleMapping>().Collection
                  .Function("GetCollaborationUsers");

            collaborationUsersByTagFunc.Namespace = "WorkflowRoleMappingService";
            collaborationUsersByTagFunc.Returns<CollaborationUserModel>();
            collaborationUsersByTagFunc.Parameter<string>("mappingName");
            collaborationUsersByTagFunc.Parameter<Guid>("workflowInstanceId");
            collaborationUsersByTagFunc.Parameter<string>("internalActivityId");

            FunctionConfiguration collaborationUsersByRoleFunc = builder
                  .EntityType<WorkflowRoleMapping>().Collection
                  .Function("GetCollaborationUsers");

            collaborationUsersByRoleFunc.Namespace = "WorkflowRoleMappingService";
            collaborationUsersByRoleFunc.Returns<CollaborationUserModel>();
            collaborationUsersByRoleFunc.Parameter<int?>("roleId");

            FunctionConfiguration collaborationUsersByAccountFunc = builder
                  .EntityType<WorkflowRoleMapping>().Collection
                  .Function("GetCollaborationUsers");

            collaborationUsersByAccountFunc.Namespace = "WorkflowRoleMappingService";
            collaborationUsersByAccountFunc.Returns<CollaborationUserModel>();
            collaborationUsersByAccountFunc.Parameter<string>("domain");
            collaborationUsersByAccountFunc.Parameter<string>("account");

            FunctionConfiguration collaborationSignsByTagFunc = builder
                  .EntityType<WorkflowRoleMapping>().Collection
                  .Function("GetCollaborationSigns");

            collaborationSignsByTagFunc.Namespace = "WorkflowRoleMappingService";
            collaborationSignsByTagFunc.Returns<CollaborationSign>();
            collaborationSignsByTagFunc.Parameter<string>("mappingName");
            collaborationSignsByTagFunc.Parameter<Guid>("workflowInstanceId");
            collaborationSignsByTagFunc.Parameter<string>("internalActivityId");

            FunctionConfiguration collaborationSignsByRoleFunc = builder
                  .EntityType<WorkflowRoleMapping>().Collection
                  .Function("GetCollaborationSigns");

            collaborationSignsByRoleFunc.Namespace = "WorkflowRoleMappingService";
            collaborationSignsByRoleFunc.Returns<CollaborationSign>();
            collaborationSignsByRoleFunc.Parameter<int?>("roleId");

            FunctionConfiguration collaborationSignsByAccountFunc = builder
                  .EntityType<WorkflowRoleMapping>().Collection
                  .Function("GetCollaborationSigns");

            collaborationSignsByAccountFunc.Namespace = "WorkflowRoleMappingService";
            collaborationSignsByAccountFunc.Returns<CollaborationSign>();
            collaborationSignsByAccountFunc.Parameter<string>("domain");
            collaborationSignsByAccountFunc.Parameter<string>("account");

            FunctionConfiguration getAuthorizedActiveWorkflowRepositories = builder
                .EntityType<WorkflowRepository>().Collection
                .Function("GetAuthorizedActiveWorkflowRepositories");

            getAuthorizedActiveWorkflowRepositories.Namespace = "WorkflowRepositoryService";
            getAuthorizedActiveWorkflowRepositories.Returns<WorkflowRepository>();
            getAuthorizedActiveWorkflowRepositories.Parameter<int>("environment");
            getAuthorizedActiveWorkflowRepositories.Parameter<bool>("anyEnv");
            getAuthorizedActiveWorkflowRepositories.Parameter<bool>("documentRequired");
            getAuthorizedActiveWorkflowRepositories.Parameter<bool>("documentUnitRequired");
            getAuthorizedActiveWorkflowRepositories.Parameter<bool>("showOnlyNoInstanceWorkflows");
            getAuthorizedActiveWorkflowRepositories.Parameter<bool>("showOnlyHasIsFascicleClosedRequired");

            FunctionConfiguration hasAuthorizedWorkflowRepositories = builder
                .EntityType<WorkflowRepository>().Collection
                .Function("HasAuthorizedWorkflowRepositories");

            hasAuthorizedWorkflowRepositories.Namespace = "WorkflowRepositoryService";
            hasAuthorizedWorkflowRepositories.Returns<bool>();
            hasAuthorizedWorkflowRepositories.Parameter<int>("environment");
            hasAuthorizedWorkflowRepositories.Parameter<bool>("anyEnv");

            FunctionConfiguration getByWorkflowActivityId = builder
               .EntityType<WorkflowRepository>().Collection
               .Function("GetByWorkflowActivityId");

            getByWorkflowActivityId.Namespace = "WorkflowRepositoryService";
            getByWorkflowActivityId.Returns<WorkflowRepository>();
            getByWorkflowActivityId.Parameter<Guid>("workflowActivityId");

            FunctionConfiguration hasHandler = builder
                .EntityType<WorkflowActivity>().Collection
                .Function("HasHandler");

            hasHandler.Namespace = "WorkflowActivityService";
            hasHandler.Returns<bool>();
            hasHandler.Parameter<Guid>("workflowActivityId");

            FunctionConfiguration isWorkflowActivityHandler = builder
                .EntityType<WorkflowActivity>().Collection
                .Function("IsWorkflowActivityHandler");

            isWorkflowActivityHandler.Namespace = "WorkflowActivityService";
            isWorkflowActivityHandler.Returns<bool>();
            isWorkflowActivityHandler.Parameter<Guid>("workflowActivityId");


            FunctionConfiguration getActiveActivitiesByReferenceIdAndEnvironment = builder
                .EntityType<WorkflowActivity>().Collection
                .Function("GetActiveActivitiesByReferenceIdAndEnvironment");

            getActiveActivitiesByReferenceIdAndEnvironment.Namespace = "WorkflowActivityService";
            getActiveActivitiesByReferenceIdAndEnvironment.Returns<WorkflowActivity>();
            getActiveActivitiesByReferenceIdAndEnvironment.Parameter<Guid>("referenceId");
            getActiveActivitiesByReferenceIdAndEnvironment.Parameter<DocSuiteWeb.Entity.Commons.DSWEnvironmentType>("type");

            FunctionConfiguration isAuthorized = builder
                .EntityType<WorkflowActivity>().Collection
                .Function("IsAuthorized");

            isAuthorized.Namespace = "WorkflowActivityService";
            isAuthorized.Returns<bool>();
            isAuthorized.Parameter<Guid>("workflowActivityId");
            isAuthorized.Parameter<string>("username");
            isAuthorized.Parameter<string>("domain");
            #endregion

            #region [ Navigation Properties ]

            builder
                .EntitySet<WorkflowActivity>("WorkflowActivities")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<WorkflowActivityLog>("WorkflowActivityLogs")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<WorkflowAuthorization>("WorkflowAuthorizations")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<WorkflowInstance>("WorkflowInstances")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<WorkflowProperty>("WorkflowProperties")
                .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<WorkflowRoleMapping>("WorkflowRoleMappings")
               .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<WorkflowInstanceLog>("WorkflowInstanceLogs")
              .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<WorkflowInstanceRole>("WorkflowInstanceRoles")
              .EntityType.HasKey(p => p.UniqueId);

            builder
              .EntitySet<WorkflowEvaluationProperty>("WorkflowEvaluationProperties")
              .EntityType.HasKey(p => p.UniqueId);

            #endregion

        }

        private static void MapOChartOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<OChart>("OCharts")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Navigation Properties ]

            builder
                .EntitySet<OChartItem>("OChartItems")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<OChartItemContainer>("OChartItemContainers")
                .EntityType.HasKey(p => p.UniqueId);
            #endregion
        }

        private static void MapMessageOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Message>("Messages")
                .EntityType.HasKey(p => p.EntityId);

            #region [ Functions ]
            FunctionConfiguration messageContactsByTagFunc = builder
                  .EntityType<MessageContact>().Collection
                  .Function("GetMessageRecipients");

            messageContactsByTagFunc.Namespace = "MessageContactService";
            messageContactsByTagFunc.Returns<MessageContactModel>();
            messageContactsByTagFunc.Parameter<string>("mappingName");
            messageContactsByTagFunc.Parameter<Guid>("workflowInstanceId");
            messageContactsByTagFunc.Parameter<string>("internalActivityId");
            #endregion

            #region [ Navigation Properties ]
            builder
                   .EntitySet<MessageAttachment>("MessageAttachments")
                   .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<MessageContact>("MessageContacts")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<MessageContactEmail>("MessageContactEmails")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<MessageEmail>("MessageEmails")
                .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<MessageLog>("MessageLogs")
                .EntityType.HasKey(p => p.EntityId);
            #endregion
        }

        private static void MapDocumentArchiveOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<DocumentSeries>("DocumentSeries")
                .EntityType.HasKey(p => p.EntityId);
            builder
                .EntitySet<MonitoringSeriesSectionModel>("MonitoringSeriesSectionModel")
                .EntityType.HasKey(p => p.UniqueId);
            builder
                .EntitySet<MonitoringQualitySummaryModel>("MonitoringQualitySummaryModel")
                .EntityType.HasKey(p => p.UniqueId);
            builder
                .EntitySet<MonitoringQualityDetailsModel>("MonitoringQualityDetailsModel")
                .EntityType.HasKey(p => p.UniqueId);
            builder
                .EntitySet<MonitoringSeriesRoleModel>("MonitoringSeriesRoleModel")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration monitoringSeriesBySection = builder
                .EntityType<DocumentSeries>().Collection
                .Function("GetMonitoringSeriesBySection");
            monitoringSeriesBySection.Namespace = "DocumentSeriesService";
            monitoringSeriesBySection.Returns<MonitoringSeriesSectionModel>();
            monitoringSeriesBySection.Parameter<string>("dateFrom");
            monitoringSeriesBySection.Parameter<string>("dateTo");

            FunctionConfiguration monitoringQualitySummary = builder
                .EntityType<DocumentSeries>().Collection
                .Function("GetMonitoringQualitySummary");
            monitoringQualitySummary.Namespace = "DocumentSeriesService";
            monitoringQualitySummary.Returns<MonitoringQualitySummaryModel>();
            monitoringQualitySummary.Parameter<string>("dateFrom");
            monitoringQualitySummary.Parameter<string>("dateTo");
            monitoringQualitySummary.Parameter<int>("idDocumentSeries").Optional();

            FunctionConfiguration monitoringQualityDetails = builder
                .EntityType<DocumentSeries>().Collection
                .Function("GetMonitoringQualityDetails");
            monitoringQualityDetails.Namespace = "DocumentSeriesService";
            monitoringQualityDetails.Returns<MonitoringQualityDetailsModel>();
            monitoringQualityDetails.Parameter<int>("idDocumentSeries");
            monitoringQualityDetails.Parameter<int?>("idRole");
            monitoringQualityDetails.Parameter<string>("dateFrom");
            monitoringQualityDetails.Parameter<string>("dateTo");

            FunctionConfiguration monitoringSeriesRole = builder
                .EntityType<DocumentSeries>().Collection
                .Function("GetMonitoringSeriesByRole");
            monitoringSeriesRole.Namespace = "DocumentSeriesService";
            monitoringSeriesRole.Returns<MonitoringSeriesRoleModel>();
            monitoringSeriesRole.Parameter<string>("dateFrom");
            monitoringSeriesRole.Parameter<string>("dateTo");
            #endregion

            #region [ Navigation Properties ]
            builder
                  .EntitySet<DocumentSeriesItem>("DocumentSeriesItems")
                  .EntityType.HasKey(p => p.UniqueId);

            builder
                  .EntitySet<DocumentSeriesItemRole>("DocumentSeriesItemRoles")
                  .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<DocumentSeriesItemLog>("DocumentSeriesItemLogs")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DocumentSeriesConstraint>("DocumentSeriesConstraints")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DocumentSeriesItemLink>("DocumentSeriesItemLinks")
                .EntityType.HasKey(p => p.EntityId);
            #endregion
        }

        private static void MapDeskOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Desk>("Desks")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Navigation Properties ]
            builder
                   .EntitySet<DeskMessage>("DeskMessages")
                   .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DeskCollaboration>("DeskCollaborations")
                .EntityType.HasKey(p => p.UniqueId);

            builder.
                EntitySet<DeskDocument>("DeskDocuments")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DeskDocumentVersion>("DeskDocumentVersions")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DeskRoleUser>("DeskRoleUsers")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DeskDocumentEndorsement>("DeskDocumentEndorsements")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DeskStoryBoard>("DeskStoryBoards")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DeskLog>("DeskLogs")
                .EntityType.HasKey(p => p.UniqueId);
            #endregion
        }

        private static void MapFascicleOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Fascicle>("Fascicles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<FascicleLog>("FascicleLogs")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]     

            FunctionConfiguration fascicleDocumentUnitsFunc = builder
                .EntityType<FascicleDocumentUnit>().Collection
                .Function("FascicleDocumentUnits");
            fascicleDocumentUnitsFunc.Namespace = "FascicleDocumentUnitService";
            fascicleDocumentUnitsFunc.ReturnsCollection<FascicleDocumentUnitModel>();
            fascicleDocumentUnitsFunc.Parameter<Guid>("idFascicle");
            fascicleDocumentUnitsFunc.Parameter<Guid?>("idFascicleFolder");
            fascicleDocumentUnitsFunc.Parameter<Guid>("idTenantAOO");

            FunctionConfiguration availableFasciclesFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("AvailableFascicles");
            availableFasciclesFunc.Namespace = "FascicleService";
            availableFasciclesFunc.Returns<FascicleModel>()
                .Parameter<Guid>("uniqueId");

            FunctionConfiguration periodicFasciclesFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("PeriodicFascicles");
            periodicFasciclesFunc.Namespace = "FascicleService";
            periodicFasciclesFunc.Returns<FascicleModel>()
                .Parameter<Guid>("uniqueId");

            FunctionConfiguration documentUnitAssociatedFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("DocumentUnitAssociated");
            documentUnitAssociatedFunc.Namespace = "FascicleService";
            documentUnitAssociatedFunc.Returns<FascicleModel>()
                .Parameter<Guid>("uniqueId");

            FunctionConfiguration notLinkedFasciclesFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("NotLinkedFascicles");

            notLinkedFasciclesFunc.Namespace = "FascicleService";
            notLinkedFasciclesFunc.Returns<FascicleModel>();
            notLinkedFasciclesFunc.Parameter<Guid>("idFascicle");
            notLinkedFasciclesFunc.Parameter<int>("idCategory");

            FunctionConfiguration hasViewableDocumentFunc = builder
                    .EntityType<Fascicle>().Collection
                    .Function("HasViewableDocument");

            hasViewableDocumentFunc.Namespace = "FascicleService";
            hasViewableDocumentFunc.Returns<bool>();
            hasViewableDocumentFunc.Parameter<Guid>("idFascicle");
            hasViewableDocumentFunc.Parameter<string>("username");
            hasViewableDocumentFunc.Parameter<string>("domain");

            FunctionConfiguration getFascicleByCategoty = builder
                .EntityType<Fascicle>().Collection
                .Function("GetFasciclesByCategory");

            getFascicleByCategoty.Namespace = "FascicleService";
            getFascicleByCategoty.Returns<FascicleModel>();
            getFascicleByCategoty.Parameter<int>("idCategory");
            getFascicleByCategoty.Parameter<string>("name");
            getFascicleByCategoty.Parameter<bool?>("hasProcess");

            FunctionConfiguration hasFascicolatedDocumentUnits = builder
                .EntityType<Fascicle>().Collection
                .Function("HasFascicolatedDocumentUnits");

            hasFascicolatedDocumentUnits.Namespace = "FascicleService";
            hasFascicolatedDocumentUnits.Returns<bool>();
            hasFascicolatedDocumentUnits.Parameter<Guid>("idFascicle");

            FunctionConfiguration hasInsertRight = builder
                .EntityType<Fascicle>().Collection
                .Function("HasInsertRight");

            hasInsertRight.Namespace = "FascicleService";
            hasInsertRight.Returns<bool>();
            hasInsertRight.Parameter<DocSuiteWeb.Entity.Fascicles.FascicleType>("fascicleType");

            FunctionConfiguration hasViewableRight = builder
                .EntityType<Fascicle>().Collection
                .Function("HasViewableRight");

            hasViewableRight.Namespace = "FascicleService";
            hasViewableRight.Returns<bool>();
            hasViewableRight.Parameter<Guid>("idFascicle");

            FunctionConfiguration hasManageableRight = builder
                .EntityType<Fascicle>().Collection
                .Function("HasManageableRight");

            hasManageableRight.Namespace = "FascicleService";
            hasManageableRight.Returns<bool>();
            hasManageableRight.Parameter<Guid>("idFascicle");

            ActionConfiguration authorizedFascicles = builder
                .EntityType<Fascicle>().Collection
                .Action("AuthorizedFascicles");

            authorizedFascicles.Namespace = "FascicleService";
            authorizedFascicles.Returns<FascicleModel>();
            authorizedFascicles.Parameter<FascicleFinderModel>("finder");

            ActionConfiguration countAuthorizedFascicles = builder
                .EntityType<Fascicle>().Collection
                .Action("CountAuthorizedFascicles");

            countAuthorizedFascicles.Namespace = "FascicleService";
            countAuthorizedFascicles.Returns<int>();
            countAuthorizedFascicles.Parameter<FascicleFinderModel>("finder");

            FunctionConfiguration isManager = builder
                .EntityType<Fascicle>().Collection
                .Function("IsManager");

            isManager.Namespace = "FascicleService";
            isManager.Returns<bool>();
            isManager.Parameter<Guid>("idFascicle");

            FunctionConfiguration hasProcedureDistributionInsertRight = builder
                .EntityType<Fascicle>().Collection
                .Function("HasProcedureDistributionInsertRight");

            hasProcedureDistributionInsertRight.Namespace = "FascicleService";
            hasProcedureDistributionInsertRight.Returns<bool>();
            hasProcedureDistributionInsertRight.Parameter<short>("idCategory");

            FunctionConfiguration authorizedFasciclesFromDocumentUnitFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("AuthorizedFasciclesFromDocumentUnit");

            authorizedFasciclesFromDocumentUnitFunc.Namespace = "FascicleService";
            authorizedFasciclesFromDocumentUnitFunc.Returns<FascicleModel>().Parameter<Guid>("uniqueIdDocumentUnit");

            FunctionConfiguration countAuthorizedFasciclesFromDocumentUnitFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("CountAuthorizedFasciclesFromDocumentUnit");
            countAuthorizedFasciclesFromDocumentUnitFunc.Namespace = "FascicleService";
            countAuthorizedFasciclesFromDocumentUnitFunc.Returns<int>().Parameter<Guid>("uniqueIdDocumentUnit");

            FunctionConfiguration findAuthorizedCategoryFascicles = builder
                .EntityType<Fascicle>().Collection
                .Function("AuthorizedCategoryFascicles");

            findAuthorizedCategoryFascicles.Namespace = "FascicleService";
            findAuthorizedCategoryFascicles.Returns<FascicleModel>();
            findAuthorizedCategoryFascicles.Parameter<short>("idCategory");
            findAuthorizedCategoryFascicles.Parameter<int>("skip");
            findAuthorizedCategoryFascicles.Parameter<int>("top");

            FunctionConfiguration countAuthorizedCategoryFascicles = builder
                .EntityType<Fascicle>().Collection
                .Function("CountAuthorizedCategoryFascicles");

            countAuthorizedCategoryFascicles.Namespace = "FascicleService";
            countAuthorizedCategoryFascicles.Returns<int>();
            countAuthorizedCategoryFascicles.Parameter<short>("idCategory");

            FunctionConfiguration fascicleFromDocumentUnitFunc = builder
                .EntityType<Fascicle>().Collection
                .Function("FasciclesFromDocumentUnit");

            fascicleFromDocumentUnitFunc.Namespace = "FascicleService";
            fascicleFromDocumentUnitFunc.Returns<FascicleModel>();
            fascicleFromDocumentUnitFunc.Parameter<Guid>("idDocumentUnit");
            #endregion

            #region [ Navigation Properties ]



            builder
                .EntitySet<FasciclePeriod>("FasciclePeriods")
                .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<FascicleLog>("FascicleLogs")
               .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<FascicleLink>("FascicleLinks")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<FascicleDocument>("FascicleDocuments")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<FascicleRole>("FascicleRoles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<FascicleDocumentUnit>("FascicleDocumentUnits")
                .EntityType.HasKey(x => x.UniqueId);

            #endregion

        }

        private static void MapTemplateDocumentRepositoryOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<TemplateDocumentRepository>("TemplateDocumentRepositories")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration getTagsFunc = builder
                .EntityType<TemplateDocumentRepository>().Collection
                .Function("GetTags");
            getTagsFunc.Namespace = "TemplateDocumentRepositoryService";
            getTagsFunc.Returns<string>();
            #endregion

            #region [ Navigation Properties ]



            #endregion
        }

        private static void MapTemplateCollaborationOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<TemplateCollaboration>("TemplateCollaborations")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration getAuthorizedTemplates = builder
                .EntityType<TemplateCollaboration>().Collection
                .Function("GetAuthorizedTemplates");

            getAuthorizedTemplates.Namespace = "TemplateCollaborationService";
            getAuthorizedTemplates.Returns<TemplateCollaborationModel>()
                .Parameter<string>("username");
            getAuthorizedTemplates.Parameter<string>("domain");


            FunctionConfiguration getInvalidatingTemplatesByRoleUserAccount = builder
                .EntityType<TemplateCollaboration>().Collection
                .Function("GetInvalidatingTemplatesByRoleUserAccount");

            getInvalidatingTemplatesByRoleUserAccount.Namespace = "TemplateCollaborationService";
            getInvalidatingTemplatesByRoleUserAccount.Returns<TemplateCollaborationModel>()
                .Parameter<string>("username");
            getInvalidatingTemplatesByRoleUserAccount.Parameter<string>("domain");
            getInvalidatingTemplatesByRoleUserAccount.Parameter<int>("idRole");

            FunctionConfiguration getAllParentsOfTemplate = builder
                .EntityType<TemplateCollaboration>().Collection
                .Function("GetAllParentsOfTemplate");

            getAllParentsOfTemplate.Namespace = "TemplateCollaborationService";
            getAllParentsOfTemplate.Returns<TemplateCollaborationModel>();
            getAllParentsOfTemplate.Parameter<Guid>("templateId");

            FunctionConfiguration getChildren = builder
                .EntityType<TemplateCollaboration>().Collection
                .Function("GetChildren");

            getChildren.Namespace = "TemplateCollaborationService";
            getChildren.Returns<TemplateCollaborationModel>();
            getChildren.Parameter<Guid>("idParent");
            getChildren.Parameter<short?>("status");
            #endregion

            #region [ Navigation Properties ]

            builder
                .EntitySet<TemplateCollaborationUser>("TemplateCollaborationUsers")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<TemplateCollaborationDocumentRepository>("TemplateCollaborationDocumentRepositories")
                .EntityType.HasKey(p => p.UniqueId);

            #endregion
        }

        private static void MapTemplateReportOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<TemplateReport>("TemplateReports")
               .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            #endregion


            #region [ Navigation Properties ]
            #endregion
        }

        private static void MapCollaborationOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Collaboration>("Collaborations")
                .EntityType.HasKey(p => p.EntityId);


            builder
                .EntitySet<CollaborationDraft>("CollaborationDrafts")
                .EntityType.HasKey(p => p.UniqueId);
            #region [ Functions ]

            //GetToVisionSignCollaborations
            ActionConfiguration toActiveVisionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetToVisionSignCollaborations");

            toActiveVisionFunc.Namespace = "CollaborationService";
            toActiveVisionFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountToVisionSignCollaborations
            ActionConfiguration countToActiveVisionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountToVisionSignCollaborations");

            countToActiveVisionFunc.Namespace = "CollaborationService";
            countToActiveVisionFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetToVisionDelegateSignCollaborations
            ActionConfiguration toActiveDelegateVisionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetToVisionDelegateSignCollaborations");

            toActiveDelegateVisionFunc.Namespace = "CollaborationService";
            toActiveDelegateVisionFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountToVisionDelegateSignCollaborations
            ActionConfiguration countToActiveDelegateVisionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountToVisionDelegateSignCollaborations");

            countToActiveDelegateVisionFunc.Namespace = "CollaborationService";
            countToActiveDelegateVisionFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetAtVisionSignCollaborations
            ActionConfiguration atVisionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetAtVisionSignCollaborations");

            atVisionFunc.Namespace = "CollaborationService";
            atVisionFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountAtVisionSignCollaborations
            ActionConfiguration countAtVisionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountAtVisionSignCollaborations");

            countAtVisionFunc.Namespace = "CollaborationService";
            countAtVisionFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetAtProtocolAdmissionCollaborations
            ActionConfiguration atProtocolAdmissionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetAtProtocolAdmissionCollaborations");

            atProtocolAdmissionFunc.Namespace = "CollaborationService";
            atProtocolAdmissionFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountAtProtocolAdmissionCollaborations
            ActionConfiguration countAtProtocolAdmissionFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountAtProtocolAdmissionCollaborations");

            countAtProtocolAdmissionFunc.Namespace = "CollaborationService";
            countAtProtocolAdmissionFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetCurrentActivitiesAllCollaborations
            ActionConfiguration currentActivitiesFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetCurrentActivitiesAllCollaborations");

            currentActivitiesFunc.Namespace = "CollaborationService";
            currentActivitiesFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountCurrentActivitiesAllCollaborations
            ActionConfiguration countCurrentActivitiesFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountCurrentActivitiesAllCollaborations");

            countCurrentActivitiesFunc.Namespace = "CollaborationService";
            countCurrentActivitiesFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetCurrentActivitiesActiveCollaborations
            ActionConfiguration currentActivitiesActiveFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetCurrentActivitiesActiveCollaborations");

            currentActivitiesActiveFunc.Namespace = "CollaborationService";
            currentActivitiesActiveFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountCurrentActivitiesActiveCollaborations
            ActionConfiguration countCurrentActivitiesActiveFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountCurrentActivitiesActiveCollaborations");

            countCurrentActivitiesActiveFunc.Namespace = "CollaborationService";
            countCurrentActivitiesActiveFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetCurrentActivitiesPastCollaborations
            ActionConfiguration currentActivitiesPastFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetCurrentActivitiesPastCollaborations");

            currentActivitiesPastFunc.Namespace = "CollaborationService";
            currentActivitiesPastFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountCurrentActivitiesPastCollaborations
            ActionConfiguration countCurrentActivitiesPastFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountCurrentActivitiesPastCollaborations");

            countCurrentActivitiesPastFunc.Namespace = "CollaborationService";
            countCurrentActivitiesPastFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetToManageCollaborations
            ActionConfiguration toManageFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetToManageCollaborations");

            toManageFunc.Namespace = "CollaborationService";
            toManageFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountToManageCollaborations
            ActionConfiguration countToManageFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountToManageCollaborations");

            countToManageFunc.Namespace = "CollaborationService";
            countToManageFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetRegisteredCollaborations
            ActionConfiguration registeredFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetRegisteredCollaborations");

            registeredFunc.Namespace = "CollaborationService";
            registeredFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountRegisteredCollaborations
            ActionConfiguration countRegisteredFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountRegisteredCollaborations");

            countRegisteredFunc.Namespace = "CollaborationService";
            countRegisteredFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            //GetMyCheckedOutCollaborations
            ActionConfiguration myCheckOutyFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("GetMyCheckedOutCollaborations");

            myCheckOutyFunc.Namespace = "CollaborationService";
            myCheckOutyFunc.Returns<CollaborationModel>()
                .Parameter<CollaborationFinderModel>("finder");

            //CountMyCheckedOutCollaborations
            ActionConfiguration countMyCheckOutyFunc = builder
                .EntityType<Collaboration>().Collection
                .Action("CountMyCheckedOutCollaborations");

            countMyCheckOutyFunc.Namespace = "CollaborationService";
            countMyCheckOutyFunc.Returns<int>()
                .Parameter<CollaborationFinderModel>("finder");

            FunctionConfiguration hasViewableRightFunc = builder
                .EntityType<Collaboration>().Collection
                .Function("HasViewableRight");

            hasViewableRightFunc.Namespace = "CollaborationService";
            hasViewableRightFunc.Parameter<int>("idCollaboration");
            hasViewableRightFunc.Returns<bool>();

            #endregion

            #region [ Navigation Properties ]
            builder
                    .EntitySet<CollaborationLog>("CollaborationLogs")
                    .EntityType.HasKey(p => p.EntityId);

            builder
                .EntitySet<CollaborationAggregate>("CollaborationAggregates")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<CollaborationSign>("CollaborationSigns")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<CollaborationUser>("CollaborationUsers")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<CollaborationVersioning>("CollaborationVersionings")
                .EntityType.HasKey(p => p.UniqueId);
            #endregion
        }

        private static void MapUDSOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<UDSRepository>("UDSRepositories")
                .EntityType.HasKey(p => p.UniqueId);

            builder
               .EntitySet<UDSFieldList>("UDSFieldLists")
               .EntityType.HasKey(p => p.UniqueId);
            #region [ Functions ]

            FunctionConfiguration viewableFunc = builder
                .EntityType<UDSRepository>().Collection
                .Function("GetViewableRepositoriesByTypology");

            viewableFunc.Namespace = "UDSRepositoryService";
            viewableFunc.Returns<UDSRepository>();
            viewableFunc.Parameter<Guid?>("idUDSTypology");
            viewableFunc.Parameter<bool>("pecAnnexedEnabled");

            FunctionConfiguration insertableFunc = builder
               .EntityType<UDSRepository>().Collection
               .Function("GetInsertableRepositoriesByTypology");

            insertableFunc.Namespace = "UDSRepositoryService";
            insertableFunc.Returns<UDSRepository>();
            insertableFunc.Parameter<string>("username");
            insertableFunc.Parameter<string>("domain");
            insertableFunc.Parameter<Guid?>("idUDSTypology");
            insertableFunc.Parameter<bool>("pecAnnexedEnabled");

            FunctionConfiguration availableCQRSPublishedUDSRepositories = builder
                .EntityType<UDSRepository>().Collection
                .Function("GetAvailableCQRSPublishedUDSRepositories");

            availableCQRSPublishedUDSRepositories.Namespace = "UDSRepositoryService";
            availableCQRSPublishedUDSRepositories.Returns<UDSRepository>();
            availableCQRSPublishedUDSRepositories.Parameter<Guid?>("idUDSTypology");
            availableCQRSPublishedUDSRepositories.Parameter<string>("name");
            availableCQRSPublishedUDSRepositories.Parameter<string>("alias");
            availableCQRSPublishedUDSRepositories.Parameter<short?>("idContainer");

            FunctionConfiguration tenantUDSRepositories = builder
                .EntityType<UDSRepository>().Collection
                .Function("GetTenantUDSRepositories");

            tenantUDSRepositories.Namespace = "UDSRepositoryService";
            tenantUDSRepositories.Returns<UDSRepository>();
            tenantUDSRepositories.Parameter<string>("tenantName");
            tenantUDSRepositories.Parameter<string>("udsName");

            FunctionConfiguration isUserAuthorized = builder
               .EntityType<UDSUser>().Collection
               .Function("IsUserAuthorized");

            isUserAuthorized.Namespace = "UDSUserService";
            isUserAuthorized.Returns<UDSUser>();
            isUserAuthorized.Parameter<Guid>("idUDS");
            isUserAuthorized.Parameter<string>("domain");
            isUserAuthorized.Parameter<string>("username");


            FunctionConfiguration GetMyLogs = builder
                .EntityType<UDSLog>().Collection
                .Function("GetMyLogs");

            GetMyLogs.Namespace = "UDSLogService";
            GetMyLogs.Returns<UDSLog>();
            GetMyLogs.Parameter<Guid>("idUDS");
            GetMyLogs.Parameter<int>("skip");
            GetMyLogs.Parameter<int>("top");

            FunctionConfiguration getChildrenByParent = builder
                .EntityType<UDSFieldList>().Collection
                .Function("GetChildrenByParent");
            getChildrenByParent.Namespace = "UDSFieldListService";
            getChildrenByParent.Returns<UDSFieldListTableValuedModel>();
            getChildrenByParent.Parameter<Guid>("parentId");

            FunctionConfiguration getParent = builder
                .EntityType<UDSFieldList>().Collection
                .Function("GetParent");
            getParent.Namespace = "UDSFieldListService";
            getParent.Returns<UDSFieldListTableValuedModel>();
            getParent.Parameter<Guid>("childId");

            FunctionConfiguration getAllParents = builder
                .EntityType<UDSFieldList>().Collection
                .Function("GetAllParents");
            getAllParents.Namespace = "UDSFieldListService";
            getAllParents.Returns<UDSFieldListTableValuedModel>();
            getAllParents.Parameter<Guid>("childId");
            #endregion

            #region [ Navigation Properties ]
            builder
                   .EntitySet<UDSSchemaRepository>("UDSSchemaRepositories")
                   .EntityType.HasKey(p => p.UniqueId);

            builder
                  .EntitySet<UDSTypology>("UDSTypologies")
                  .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSLog>("UDSLogs")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSRole>("UDSRoles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSFieldList>("UDSFieldLists")
                .EntityType.HasKey(p => p.UniqueId);
            builder
                .EntitySet<UDSUser>("UDSUsers")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSContact>("UDSContacts")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSMessage>("UDSMessages")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSDocumentUnit>("UDSDocumentUnits")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSPECMail>("UDSPECMails")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<UDSCollaboration>("UDSCollaborations")
                .EntityType.HasKey(p => p.UniqueId);

            #endregion
        }

        private static void MapMassimarioScartoOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<MassimarioScarto>("MassimariScarto")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            FunctionConfiguration allChildrenFunc = builder
                .EntityType<MassimarioScarto>().Collection
                .Function("GetAllChildren");

            allChildrenFunc.Namespace = "MassimariScartoService";
            allChildrenFunc.Returns<MassimarioScartoModel>();
            allChildrenFunc.Parameter<Guid?>("parentId");
            allChildrenFunc.Parameter<bool>("includeLogicalDelete");

            FunctionConfiguration getMassimariFunc = builder
                .EntityType<MassimarioScarto>().Collection
                .Function("GetMassimari");

            getMassimariFunc.Namespace = "MassimariScartoService";
            getMassimariFunc.Returns<MassimarioScartoModel>();
            getMassimariFunc.Parameter<string>("name");
            getMassimariFunc.Parameter<bool>("includeLogicalDelete");

            FunctionConfiguration getMassimariFuncFull = builder
                .EntityType<MassimarioScarto>().Collection
                .Function("GetMassimari");

            getMassimariFuncFull.Namespace = "MassimariScartoService";
            getMassimariFuncFull.Returns<MassimarioScartoModel>();
            getMassimariFuncFull.Parameter<string>("name");
            getMassimariFuncFull.Parameter<string>("fullCode");
            getMassimariFuncFull.Parameter<bool>("includeLogicalDelete");

            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapDossierOdata(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Dossier>("Dossiers")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            ActionConfiguration authorizedDossiers = builder
               .EntityType<Dossier>().Collection
               .Action("GetAuthorizedDossiers");

            authorizedDossiers.Namespace = "DossierService";
            authorizedDossiers.Returns<DossierModel>();
            authorizedDossiers.Parameter<DossierFinderModel>("finder");

            ActionConfiguration countDossiers = builder
              .EntityType<Dossier>().Collection
              .Action("CountAuthorizedDossiers");

            countDossiers.Namespace = "DossierService";
            countDossiers.Returns<int>();
            countDossiers.Parameter<DossierFinderModel>("finder");

            FunctionConfiguration getDossier = builder
                .EntityType<Dossier>().Collection
                .Function("GetDossier");

            getDossier.Namespace = "DossierService";
            getDossier.Returns<DossierModel>();
            getDossier.Parameter<Guid>("uniqueId");


            FunctionConfiguration getCompleteDossier = builder
                .EntityType<Dossier>().Collection
                .Function("GetCompleteDossier");

            getCompleteDossier.Namespace = "DossierService";
            getCompleteDossier.Returns<DossierModel>();
            getCompleteDossier.Parameter<Guid>("uniqueId");

            FunctionConfiguration getDossierContacts = builder
               .EntityType<Dossier>().Collection
               .Function("GetDossierContacts");

            getDossierContacts.Namespace = "DossierService";
            getDossierContacts.Returns<ContactTableValuedModel>();
            getDossierContacts.Parameter<Guid>("uniqueId");


            FunctionConfiguration getDossierRoles = builder
                .EntityType<Dossier>().Collection
                .Function("GetDossierRoles");

            getDossierRoles.Namespace = "DossierService";
            getDossierRoles.Returns<DossierRoleModel>();
            getDossierRoles.Parameter<Guid>("idDossier");

            FunctionConfiguration isViewableDossier = builder
                   .EntityType<Dossier>().Collection
                   .Function("IsViewableDossier");

            isViewableDossier.Namespace = "DossierService";
            isViewableDossier.Returns<bool>();
            isViewableDossier.Parameter<Guid>("idDossier");

            FunctionConfiguration isManageableDossier = builder
                   .EntityType<Dossier>().Collection
                   .Function("IsManageableDossier");

            isManageableDossier.Namespace = "DossierService";
            isManageableDossier.Returns<bool>();
            isManageableDossier.Parameter<Guid>("idDossier");

            FunctionConfiguration hasInsertRight = builder
                 .EntityType<Dossier>().Collection
                 .Function("HasInsertRight");

            hasInsertRight.Namespace = "DossierService";
            hasInsertRight.Returns<bool>();

            FunctionConfiguration hasModifyRight = builder
               .EntityType<Dossier>().Collection
               .Function("HasModifyRight");

            hasModifyRight.Namespace = "DossierService";
            hasModifyRight.Parameter<Guid>("idDossier");
            hasModifyRight.Returns<bool>();

            FunctionConfiguration hasRootNode = builder
              .EntityType<Dossier>().Collection
              .Function("HasRootNode");

            hasRootNode.Namespace = "DossierService";
            hasRootNode.Parameter<Guid>("idDossier");
            hasRootNode.Returns<bool>();

            FunctionConfiguration allFasciclesAreClosed = builder
              .EntityType<Dossier>().Collection
              .Function("AllFasciclesAreClosed");

            allFasciclesAreClosed.Namespace = "DossierService";
            allFasciclesAreClosed.Parameter<Guid>("idDossier");
            allFasciclesAreClosed.Returns<bool>();
            #endregion

            #region [ Navigation Properties ]
            builder
                .EntitySet<DossierLog>("DossierLogs")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DossierDocument>("DossierDocuments")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DossierRole>("DossierRoles")
                .EntityType.HasKey(p => p.UniqueId);


            builder
                .EntitySet<DossierComment>("DossierComments")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DossierDocument>("DossierDocuments")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DossierFolderRole>("DossierFolderRoles")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                .EntitySet<DossierLink>("DossierLinks")
                .EntityType.HasKey(p => p.UniqueId);
            #endregion
        }

        private static void MapDossierFolderOdata(ODataModelBuilder builder)
        {
            builder
                .EntitySet<DossierFolder>("DossierFolderss")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            FunctionConfiguration getRootDossierFolders = builder
               .EntityType<DossierFolder>().Collection
               .Function("GetRootDossierFolders");

            getRootDossierFolders.Namespace = "DossierFolderService";
            getRootDossierFolders.Returns<DossierFolderModel>();
            getRootDossierFolders.Parameter<Guid>("idDossier");
            getRootDossierFolders.Parameter<short?>("status");

            FunctionConfiguration getChildrenByParent = builder
               .EntityType<DossierFolder>().Collection
               .Function("GetChildrenByParent");

            getChildrenByParent.Namespace = "DossierFolderService";
            getChildrenByParent.Returns<DossierFolderModel>();
            getChildrenByParent.Parameter<Guid>("idDossierFolder");
            getChildrenByParent.Parameter<short?>("status");

            FunctionConfiguration getProcessFolders = builder
               .EntityType<DossierFolder>().Collection
               .Function("GetProcessFolders");

            getProcessFolders.Namespace = "DossierFolderService";
            getProcessFolders.Returns<DossierFolderModel>();
            getProcessFolders.Parameter<string>("name");
            getProcessFolders.Parameter<Guid>("idProcess");
            getProcessFolders.Parameter<bool>("loadOnlyActive");
            getProcessFolders.Parameter<bool>("loadOnlyMy");

            FunctionConfiguration getChildrenByParentProcess = builder
               .EntityType<DossierFolder>().Collection
               .Function("GetChildrenByParentProcess");

            getChildrenByParentProcess.Namespace = "DossierFolderService";
            getChildrenByParentProcess.Returns<DossierFolderModel>();
            getChildrenByParentProcess.Parameter<Guid>("idProcess");
            getChildrenByParentProcess.Parameter<bool>("loadOnlyActive");
            getChildrenByParentProcess.Parameter<bool>("loadOnlyMy");

            FunctionConfiguration getAllParentsOfFascicle = builder
               .EntityType<DossierFolder>().Collection
               .Function("GetAllParentsOfFascicle");

            getAllParentsOfFascicle.Namespace = "DossierFolderService";
            getAllParentsOfFascicle.Returns<DossierFolderModel>();
            getAllParentsOfFascicle.Parameter<Guid>("idDossier");
            getAllParentsOfFascicle.Parameter<Guid?>("idFascicle");

            FunctionConfiguration hasAssociatedFascicles = builder
                .EntityType<DossierFolder>().Collection
                .Function("HasAssociatedFascicles");

            hasAssociatedFascicles.Namespace = "DossierFolderService";
            hasAssociatedFascicles.Returns<bool>();
            hasAssociatedFascicles.Parameter<Guid>("idDossier");

            FunctionConfiguration getParent = builder
                .EntityType<DossierFolder>().Collection
                .Function("GetParent");

            getParent.Namespace = "DossierFolderService";
            getParent.Returns<DossierFolderModel>();
            getParent.Parameter<Guid>("idDossierFolder");

            FunctionConfiguration countChildren = builder
                .EntityType<DossierFolder>().Collection
                .Function("CountDossierFolderChildren");

            countChildren.Namespace = "DossierFolderService";
            countChildren.Returns<int>();
            countChildren.Parameter<Guid>("idDossierFolder");
            countChildren.Parameter<bool?>("loadOnlyFolders");

            FunctionConfiguration getChildren = builder
               .EntityType<DossierFolder>().Collection
               .Function("GetChildren");

            getChildren.Namespace = "DossierFolderService";
            getChildren.Returns<DossierFolderModel>();
            getChildren.Parameter<Guid>("idDossierFolder");
            getChildren.Parameter<int>("skip");
            getChildren.Parameter<int>("top");
            getChildren.Parameter<bool?>("loadOnlyFolders");
            #endregion

            #region [ Navigation Properties ]
            builder
                .EntitySet<ProcessFascicleTemplate>("ProcessFascicleTemplates")
                .EntityType.HasKey(p => p.UniqueId);
            #endregion
        }

        private static void MapParametersOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<ODataParameterModel>("Parameters")
                .EntityType.HasKey(p => p.Key);

            builder
                .EntitySet<Parameter>("IncrementalParameters")
                .EntityType.HasKey(p => p.UniqueId);



            #region [ Functions ]
            #endregion

            #region [ Navigation Properties ]
            #endregion
        }

        private static void MapFascicleFolderOdata(ODataModelBuilder builder)
        {
            builder
                .EntitySet<FascicleFolder>("FascicleFolderss")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration getChildrenByParent = builder
              .EntityType<FascicleFolder>().Collection
              .Function("GetChildrenByParent");

            getChildrenByParent.Namespace = "FascicleFolderService";
            getChildrenByParent.Returns<FascicleFolderModel>();
            getChildrenByParent.Parameter<Guid>("idFascicleFolder");

            FunctionConfiguration getByCategoryAndFascicle = builder
             .EntityType<FascicleFolder>().Collection
             .Function("GetByCategoryAndFascicle");

            getByCategoryAndFascicle.Namespace = "FascicleFolderService";
            getByCategoryAndFascicle.Returns<FascicleFolderModel>();
            getByCategoryAndFascicle.Parameter<Guid>("idFascicle");
            getByCategoryAndFascicle.Parameter<short>("idCategory");

            FunctionConfiguration getChildren = builder
               .EntityType<FascicleFolder>().Collection
               .Function("GetChildren");

            getChildren.Namespace = "FascicleFolderService";
            getChildren.Returns<FascicleFolderModel>();
            getChildren.Parameter<Guid>("idFascicleFolder");
            getChildren.Parameter<int>("skip");
            getChildren.Parameter<int>("top");

            FunctionConfiguration countChildren = builder
                .EntityType<FascicleFolder>().Collection
                .Function("CountFascicleFolderChildren");

            countChildren.Namespace = "FascicleFolderService";
            countChildren.Returns<int>();
            countChildren.Parameter<Guid>("idFascicleFolder");
            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapConservationOdata(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Conservation>("Conservations")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration getAvailableProtocolLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailableProtocolLogs");

            getAvailableProtocolLogs.Namespace = "ConservationService";
            getAvailableProtocolLogs.Returns<ConservationLogModel>();
            getAvailableProtocolLogs.Parameter<int>("skip");
            getAvailableProtocolLogs.Parameter<int>("top");

            FunctionConfiguration countAvailableProtocolLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailableProtocolLogs");

            countAvailableProtocolLogs.Namespace = "ConservationService";
            countAvailableProtocolLogs.Returns<int>();

            FunctionConfiguration getAvailableDocumentSeriesItemLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailableDocumentSeriesItemLogs");

            getAvailableDocumentSeriesItemLogs.Namespace = "ConservationService";
            getAvailableDocumentSeriesItemLogs.Returns<ConservationLogModel>();
            getAvailableDocumentSeriesItemLogs.Parameter<int>("skip");
            getAvailableDocumentSeriesItemLogs.Parameter<int>("top");

            FunctionConfiguration countAvailableDocumentSeriesItemLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailableDocumentSeriesItemLogs");

            countAvailableDocumentSeriesItemLogs.Namespace = "ConservationService";
            countAvailableDocumentSeriesItemLogs.Returns<int>();

            FunctionConfiguration getAvailableUDSLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailableUDSLogs");

            getAvailableUDSLogs.Namespace = "ConservationService";
            getAvailableUDSLogs.Returns<ConservationLogModel>();
            getAvailableUDSLogs.Parameter<int>("skip");
            getAvailableUDSLogs.Parameter<int>("top");

            FunctionConfiguration countAvailableUDSLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailableUDSLogs");

            countAvailableUDSLogs.Namespace = "ConservationService";
            countAvailableUDSLogs.Returns<int>();

            FunctionConfiguration getAvailableDossierLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailableDossierLogs");

            getAvailableDossierLogs.Namespace = "ConservationService";
            getAvailableDossierLogs.Returns<ConservationLogModel>();
            getAvailableDossierLogs.Parameter<int>("skip");
            getAvailableDossierLogs.Parameter<int>("top");

            FunctionConfiguration countAvailableDossierLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailableDossierLogs");

            countAvailableDossierLogs.Namespace = "ConservationService";
            countAvailableDossierLogs.Returns<int>();

            FunctionConfiguration getAvailableFascicleLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailableFascicleLogs");

            getAvailableFascicleLogs.Namespace = "ConservationService";
            getAvailableFascicleLogs.Returns<ConservationLogModel>();
            getAvailableFascicleLogs.Parameter<int>("skip");
            getAvailableFascicleLogs.Parameter<int>("top");

            FunctionConfiguration countAvailableFascicleLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailableFascicleLogs");

            countAvailableFascicleLogs.Namespace = "ConservationService";
            countAvailableFascicleLogs.Returns<int>();

            FunctionConfiguration getAvailablePECMailLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailablePECMailLogs");

            getAvailablePECMailLogs.Namespace = "ConservationService";
            getAvailablePECMailLogs.Returns<ConservationLogModel>();
            getAvailablePECMailLogs.Parameter<int>("skip");
            getAvailablePECMailLogs.Parameter<int>("top");

            FunctionConfiguration countAvailablePECMailLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailablePECMailLogs");

            countAvailablePECMailLogs.Namespace = "ConservationService";
            countAvailablePECMailLogs.Returns<int>();

            FunctionConfiguration getAvailableTableLogs = builder
              .EntityType<Conservation>().Collection
              .Function("GetAvailableTableLogs");

            getAvailableTableLogs.Namespace = "ConservationService";
            getAvailableTableLogs.Returns<ConservationLogModel>();
            getAvailableTableLogs.Parameter<int>("skip");
            getAvailableTableLogs.Parameter<int>("top");

            FunctionConfiguration countAvailableTableLogs = builder
              .EntityType<Conservation>().Collection
              .Function("CountAvailableTableLogs");

            countAvailableTableLogs.Namespace = "ConservationService";
            countAvailableTableLogs.Returns<int>();
            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapTransparentAdministrationMonitorLogsOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<TransparentAdministrationMonitorLog>("TransparentAdministrationMonitorLogs")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            FunctionConfiguration transparentAdministrationMonitorsByDateIntervalFunc = builder
                .EntityType<TransparentAdministrationMonitorLog>().Collection
                .Function("GetByDateInterval");
            transparentAdministrationMonitorsByDateIntervalFunc.Namespace = "TransparentAdministrationMonitorLogService";
            transparentAdministrationMonitorsByDateIntervalFunc.Returns<TransparentAdministrationMonitorLogModel>();
            transparentAdministrationMonitorsByDateIntervalFunc.Parameter<string>("dateFrom");
            transparentAdministrationMonitorsByDateIntervalFunc.Parameter<string>("dateTo");
            transparentAdministrationMonitorsByDateIntervalFunc.Parameter<string>("userName");
            transparentAdministrationMonitorsByDateIntervalFunc.Parameter<short?>("idContainer");
            transparentAdministrationMonitorsByDateIntervalFunc.Parameter<int?>("environment");
            #endregion

            #region [ Navigation Properties ]

            #endregion
        }

        private static void MapServiceBusTopicsOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<ServiceBusTopicMessage>("ServiceBusTopics")
                .EntityType.HasKey(p => p.MessageId);

            #region [ Functions ]
            #endregion

            #region [ Navigation Properties ]
            #endregion
        }

        private static void MapJeepServiceHostsOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<JeepServiceHost>("JeepServiceHosts")
                .EntityType.HasKey(p => p.UniqueId);
        }

        private static void MapPosteWebOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<PosteOnLineRequest>("POLRequests")
                .EntityType.HasKey(p => p.UniqueId);

            FunctionConfiguration polRequest = builder
                .EntityType<PosteOnLineRequest>().Collection
                .Function("GetPOLRequest");

            polRequest.Namespace = "POLRequestService";
            polRequest.Parameter<Guid>("uniqueId");
            polRequest.Returns<PosteOnLineRequest>();

            FunctionConfiguration polRequestsByDocumentUnitId = builder
               .EntityType<PosteOnLineRequest>().Collection
               .Function("GetPOLRequestsByDocumentUnitId");

            polRequestsByDocumentUnitId.Namespace = "POLRequestService";
            polRequestsByDocumentUnitId.Parameter<Guid>("documentUnitId");
            polRequestsByDocumentUnitId.Returns<PosteOnLineRequest>();
        }

        private static void MapTenantsOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<Tenant>("Tenants")
                .EntityType.HasKey(p => p.UniqueId);

            builder
                  .EntitySet<TenantAOO>("TenantsAOO")
                  .EntityType.HasKey(p => p.UniqueId);

            FunctionConfiguration userTenants = builder
                .EntityType<Tenant>().Collection
                .Function("GetUserTenants");
            userTenants.Namespace = "TenantService";
            userTenants.Returns<Tenant>();

            #region [ Navigation Properties ]

            builder
                .EntitySet<TenantConfiguration>("TenantConfigurations")
                .EntityType.HasKey(p => p.UniqueId);
            builder
                .EntitySet<TenantWorkflowRepository>("TenantWorkflowRepositories")
                .EntityType.HasKey(p => p.UniqueId);

            #endregion
        }

        private static void MapReportsOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<ReportCategoryModel>("ReportServices")
                .EntityType.HasKey(p => p.Id);
            builder
                .EntitySet<ReportModel>("ReportModel")
                .EntityType.HasKey(p => p.Id);
            builder
                .EntitySet<ReportOutputFormatModel>("ReportOutputFormatModel")
                .EntityType.HasKey(p => p.Name);
            builder
                .EntitySet<ReportParameterModel>("ReportParameterModel")
                .EntityType.HasKey(p => p.Name);

            #region [ Functions ]

            FunctionConfiguration category = builder
                .EntityType<ReportCategoryModel>().Collection
                .Function("GetCategories");
            category.Namespace = "ReportServices";
            category.Returns<ReportCategoryModel>();

            FunctionConfiguration report = builder
                .EntityType<ReportCategoryModel>().Collection
                .Function("GetReports");
            report.Namespace = "ReportServices";
            report.Returns<ReportModel>();
            report.Parameter<string>("documentId");

            FunctionConfiguration format = builder
                .EntityType<ReportCategoryModel>().Collection
                .Function("GetOutputTypes");
            format.Namespace = "ReportServices";
            format.Returns<ReportOutputFormatModel>();

            FunctionConfiguration parameters = builder
                .EntityType<ReportCategoryModel>().Collection
                .Function("GetParameters");
            parameters.Namespace = "ReportServices";
            parameters.Returns<ReportParameterModel>();
            parameters.Parameter<string>("reportId");

            #endregion

            #region [ Navigation Properties ]
            #endregion
        }

        private static void MapProcessesOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<Process>("Processes")
               .EntityType.HasKey(p => p.UniqueId);
            builder
               .EntitySet<ProcessFascicleTemplate>("ProcessFascicleTemplates")
               .EntityType.HasKey(p => p.UniqueId);
            builder
               .EntitySet<ProcessFascicleWorkflowRepository>("ProcessFascicleWorkflowRepositories")
               .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            FunctionConfiguration process = builder
                .EntityType<Process>().Collection
                .Function("AvailableProcesses");
            process.Namespace = "ProcessService";
            process.Returns<ProcessModel>();
            process.Parameter<string>("name");
            process.Parameter<short?>("categoryId");
            process.Parameter<Guid?>("dossierId");
            process.Parameter<bool>("loadOnlyMy");
            process.Parameter<bool>("isProcessActive");

            FunctionConfiguration categoryProcesses = builder
                .EntityType<Process>().Collection
                .Function("CategoryProcesses");
            categoryProcesses.Namespace = "ProcessService";
            categoryProcesses.Returns<ProcessModel>();
            categoryProcesses.Parameter<short>("categoryId");
            categoryProcesses.Parameter<bool>("loadOnlyMy");
            categoryProcesses.Parameter<int>("skip");
            categoryProcesses.Parameter<int>("top");

            FunctionConfiguration countCategoryProcesses = builder
                .EntityType<Process>().Collection
                .Function("CountCategoryProcesses");
            countCategoryProcesses.Namespace = "ProcessService";
            countCategoryProcesses.Returns<int>();
            countCategoryProcesses.Parameter<short>("categoryId");
            countCategoryProcesses.Parameter<bool>("loadOnlyMy");

            #endregion

            #region [ Navigation Properties ]
            builder
                 .EntitySet<Category>("Categories")
                 .EntityType.HasKey(p => p.EntityShortId);
            #endregion
        }

        private static void MapTasksOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<TaskHeader>("TaskHeaders")
               .EntityType.HasKey(p => p.EntityId);

            #region [ Functions ]

            #endregion

            #region [ Navigation Properties ]
            builder
                 .EntitySet<TaskHeaderProtocol>("TaskHeaderProtocols")
                 .EntityType.HasKey(p => p.EntityId);
            #endregion
        }

        private static void MapTendersOData(ODataModelBuilder builder)
        {
            builder
               .EntitySet<TenderHeader>("TenderHeaders")
               .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]

            #endregion

            #region [ Navigation Properties ]
            builder
                 .EntitySet<TenderLot>("TenderLots")
                 .EntityType.HasKey(p => p.UniqueId);

            builder
                 .EntitySet<TenderLotPayment>("TenderLotPayments")
                 .EntityType.HasKey(p => p.UniqueId);
            #endregion
        }

        private static void MapUserLogOData(ODataModelBuilder builder)
        {
            builder
                .EntitySet<UserLog>("UserLogs")
                .EntityType.HasKey(p => p.UniqueId);

            #region [ Functions ]
            FunctionConfiguration userLog = builder
                .EntityType<UserLog>().Collection
                .Function("GetCurrentUserLogSecure");
            userLog.Namespace = "UserLogService";
            userLog.ReturnsFromEntitySet<UserLog>("UserLogs");
            #endregion

            #region [ Navigation Properties ]
            #endregion
        }
    }
}
