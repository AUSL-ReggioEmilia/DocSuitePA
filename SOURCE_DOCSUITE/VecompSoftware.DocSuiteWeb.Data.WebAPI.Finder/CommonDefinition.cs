using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder
{
    public static class CommonDefinition
    {
        public static class OData
        {
            public static class DocumentUnitService
            {
                public const string FX_GetFascicolableDocuments = "DocumentUnitService.GetFascicolableDocumentUnits(dateFrom='{0}',dateTo='{1}',includeThreshold={2},threshold='{3}',idTenantAOO={4},excludeLinked={5})";
                public const string FX_GetAutorizedDocuments = "DocumentUnitService.GetAuthorizedDocuments(dateFrom='{0}',dateTo='{1}',idTenantAOO={2})";
            }

            public static class ProtocolService
            {
                public const string FX_GetAuthorizedProtocols = "ProtocolService.GetAuthorizedProtocols(username='{0}',domain='{1}',dateFrom='{2}',dateTo='{3}')";
            }

            public static class CollaborationService
            {
                public const string FX_GetAtVisionSignCollaborations = "CollaborationService.GetAtVisionSignCollaborations";
                public const string FX_GetToVisionSignCollaborations = "CollaborationService.GetToVisionSignCollaborations";
                public const string FX_GetAtProtocolAdmissionCollaborations = "CollaborationService.GetAtProtocolAdmissionCollaborations";
                public const string FX_GetCurrentActivitiesAllCollaborations = "CollaborationService.GetCurrentActivitiesAllCollaborations";
                public const string FX_GetCurrentActivitiesActiveCollaborations = "CollaborationService.GetCurrentActivitiesActiveCollaborations";
                public const string FX_GetCurrentActivitiesPastCollaborations = "CollaborationService.GetCurrentActivitiesPastCollaborations";
                public const string FX_GetToManageCollaborations = "CollaborationService.GetToManageCollaborations";
                public const string FX_GetRegisteredCollaborations = "CollaborationService.GetRegisteredCollaborations";
                public const string FX_GetMyCheckedOutCollaborations = "CollaborationService.GetMyCheckedOutCollaborations";
                public const string FX_GetToVisionDelegateSignCollaborations = "CollaborationService.GetToVisionDelegateSignCollaborations";

                public const string FX_CountAtVisionSignCollaborations = "CollaborationService.CountAtVisionSignCollaborations";
                public const string FX_CountToVisionSignCollaborations = "CollaborationService.CountToVisionSignCollaborations";
                public const string FX_CountAtProtocolAdmissionCollaborations = "CollaborationService.CountAtProtocolAdmissionCollaborations";
                public const string FX_CountCurrentActivitiesAllCollaborations = "CollaborationService.CountCurrentActivitiesAllCollaborations";
                public const string FX_CountCurrentActivitiesActiveCollaborations = "CollaborationService.CountCurrentActivitiesActiveCollaborations";
                public const string FX_CountCurrentActivitiesPastCollaborations = "CollaborationService.CountCurrentActivitiesPastCollaborations";
                public const string FX_CountToManageCollaborations = "CollaborationService.CountToManageCollaborations";
                public const string FX_CountRegisteredCollaborations = "CollaborationService.CountRegisteredCollaborations";
                public const string FX_CountMyCheckedOutCollaborations = "CollaborationService.CountMyCheckedOutCollaborations";
                public const string FX_CountToVisionDelegateSignCollaborations = "CollaborationService.CountToVisionDelegateSignCollaborations";
            }

            public static class TemplateCollaborationService
            {
                public const string FX_GetAuthorizedTemplates = "TemplateCollaborationService.GetAuthorizedTemplates(username='{0}',domain='{1}')";
                public const string FX_GetInvalidatingTemplatesByRoleUserAccount = "TemplateCollaborationService.GetInvalidatingTemplatesByRoleUserAccount(username='{0}',domain='{1}',idRole={2})";
            }
            
            public static class CategoryFascicleService
            {
                public const string FX_GetAuthorizedTemplates = "CategoryFascicleService.GetParentWithCategoryFascicle(idCategory={0},environment={1})";
            }

            public static class UDSRepositoryService
            {
                public const string FX_GetViewableRepositoriesByTypology = "UDSRepositoryService.GetViewableRepositoriesByTypology(idUDSTypology={0},pecAnnexedEnabled={1})";
                public const string FX_GetInsertableRepositoriesByTypology = "UDSRepositoryService.GetInsertableRepositoriesByTypology(username='{0}',domain='{1}',idUDSTypology={2},pecAnnexedEnabled={3})";
                public const string FX_IsUserAuthorized = "UDSUserService.IsUserAuthorized(idUDS={0},domain='{1}',username='{2}')";
            }

            public static class WorkflowActivityService
            {
                public const string FX_IsAuthorized = "WorkflowActivityService.IsAuthorized(workflowActivityId={0},username='{1}',domain='{2}')";
            }

            public static class FascicleService
            {
                public const string FX_GetChildrenByParent = "FascicleFolderService.GetChildrenByParent(idFascicleFolder={0})";
                public const string FX_GetAuthorizedFascicles = @"FascicleService.AuthorizedFascicles";
                public const string FX_GetCountAuthorizedFascicles = @"FascicleService.CountAuthorizedFascicles";
            }

            public static class DocumentSeriesService
            {
                public const string FX_GetMonitoringQualitySummary = "DocumentSeriesService.GetMonitoringQualitySummary(dateFrom='{0}',dateTo='{1}',idDocumentSeries={2})";
                public const string FX_GetMonitoringQualityDetails = "DocumentSeriesService.GetMonitoringQualityDetails(idDocumentSeries={0},idRole={1},dateFrom='{2}',dateTo='{3}')";
                public const string FX_GetMonitoringSeriesRole = "DocumentSeriesService.GetMonitoringSeriesByRole(dateFrom='{0}',dateTo='{1}')";
                public const string FX_GetMonitoringSeriesSection = "DocumentSeriesService.GetMonitoringSeriesBySection(dateFrom='{0}',dateTo='{1}')";
            }

            public static class TenantService
            {
                public const string FX_GetUserTenants = "TenantService.GetUserTenants()";
            }
            public static class DomainUser
            {
                public const string FX_GetCurrentRights = "DomainUserService.GetCurrentRights()";
            }

            public static class DossierService
            { 
                public const string FX_GetAuthorizedDossiers = @"DossierService.GetAuthorizedDossiers";
            }

            public static class RoleUserService
            {
                //(finder=@p0)?@p0={0}
                public const string FX_AllSecretariesFromDossier = "RoleUserService.GetRoleUsersFromDossier(finder=@p0)?@p0={0}";
            }
        }

    }
}
