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
                public const string FX_GetFascicolableDocuments = "DocumentUnitService.GetFascicolableDocumentUnits(username='{0}',domain='{1}',dateFrom='{2}',dateTo='{3}',includeThreshold={4},threshold='{5}',excludeLinked={6})";
                public const string FX_GetAutorizedDocuments = "DocumentUnitService.GetAuthorizedDocuments(username='{0}',domain='{1}',dateFrom='{2}',dateTo='{3}',isSecurityEnabled={4})";
            }

            public static class ProtocolService
            {
                public const string FX_GetAuthorizedProtocols = "ProtocolService.GetAuthorizedProtocols(username='{0}',domain='{1}',dateFrom='{2}',dateTo='{3}')";
            }

            public static class CollaborationService
            {
                public const string FX_GetAtVisionSignCollaborations = "CollaborationService.GetAtVisionSignCollaborations(username='{0}',domain='{1}')";
                public const string FX_GetToVisionSignCollaborations = "CollaborationService.GetToVisionSignCollaborations(username='{0}',domain='{1}',isRequired=null)";
                public const string FX_GetToVisionSignRequiredCollaborations = "CollaborationService.GetToVisionSignCollaborations(username='{0}',domain='{1}',isRequired=true)";
                public const string FX_GetToVisionSignNoRequiredCollaborations = "CollaborationService.GetToVisionSignCollaborations(username='{0}',domain='{1}',isRequired=false)";
                public const string FX_GetAtProtocolAdmissionCollaborations = "CollaborationService.GetAtProtocolAdmissionCollaborations(username='{0}',domain='{1}')";
                public const string FX_GetCurrentActivitiesAllCollaborations = "CollaborationService.GetCurrentActivitiesAllCollaborations(username='{0}',domain='{1}')";
                public const string FX_GetCurrentActivitiesActiveCollaborations = "CollaborationService.GetCurrentActivitiesActiveCollaborations(username='{0}',domain='{1}')";
                public const string FX_GetCurrentActivitiesPastCollaborations = "CollaborationService.GetCurrentActivitiesPastCollaborations(username='{0}',domain='{1}')";
                public const string FX_GetToManageCollaborations = "CollaborationService.GetToManageCollaborations(username='{0}',domain='{1}')";
                public const string FX_GetRegisteredCollaborations = "CollaborationService.GetRegisteredCollaborations(username='{0}',domain='{1}',dateFrom='{2}',dateTo='{3}')";
                public const string FX_GetMyCheckedOutCollaborations = "CollaborationService.GetMyCheckedOutCollaborations(username='{0}',domain='{1}')";
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
                public const string FX_GetAuthorizedFascicles = @"FascicleService.AuthorizedFascicles(finder=@p0)?@p0={0}";
                public const string FX_GetCountAuthorizedFascicles = @"FascicleService.CountAuthorizedFascicles(finder=@p0)?@p0={0}";
            }

            public static class DocumentSeriesService
            {
                public const string FX_GetMonitoringQualitySummary = "DocumentSeriesService.GetMonitoringQualitySummary(dateFrom='{0}',dateTo='{1}',idDocumentSeries={2})";
                public const string FX_GetMonitoringQualityDetails = "DocumentSeriesService.GetMonitoringQualityDetails(idDocumentSeries={0},idRole={1},dateFrom='{2}',dateTo='{3}')";
                public const string FX_GetMonitoringSeriesRole = "DocumentSeriesService.GetMonitoringSeriesByRole(dateFrom='{0}',dateTo='{1}')";
                public const string FX_GetMonitoringSeriesSection = "DocumentSeriesService.GetMonitoringSeriesBySection(dateFrom='{0}',dateTo='{1}')";
            }

            public static class PECMailService
            {
                public const string FX_GetEventPECSummaryError = "?topicName={0}&subscriptionName={1}";
                public const string FX_GetEventPECStreamError = "?topicName={0}&subscriptionName={1}&correlationId={2}";
                
            }

            public static class TenantService
            {
                public const string FX_GetUserTenants = "TenantService.GetUserTenants()";
            }
        }

    }
}
