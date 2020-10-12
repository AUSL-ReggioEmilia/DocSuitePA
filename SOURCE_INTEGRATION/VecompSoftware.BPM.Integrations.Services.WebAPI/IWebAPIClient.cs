using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.WebAPI.Models;
using VecompSoftware.Core.Command.CQRS.Commands;
using VecompSoftware.Core.Command.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.ServiceBus;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Services.Command;

namespace VecompSoftware.BPM.Integrations.Services.WebAPI
{
    public interface IWebAPIClient
    {
        void SetUDSEndpoints(string endpointName, string controllerName);
        Task PushCorrelatedNotificationAsync(string message, string moduleName, Guid tenantId, Guid tenantAOOId, string tenantName, Guid? correlationId, IIdentityContext identity, NotificationType notificationType);
        Task<DocumentUnit> GetDocumentUnitAsync(Guid uniqueId);
        Task<ICollection<Location>> GetLocationAsync(int id);
        Task<DocumentUnit> GetDocumentUnitAsync(short year, int number, bool expandChains = false);
        Task<TemplateDocumentRepository> GetTemplateDocumentRepositoryAsync(Guid uniqueId);
        Task<ICollection<DocumentUnitChain>> GetDocumentUnitChainsAsync(Guid uniqueId);
        Task<ICollection<WorkflowProperty>> GetWorkflowPropertiesAsync(Guid uniqueId);
        Task<ICollection<WorkflowRoleMapping>> GetWorkflowRoleMappingAsync(string odataQuery);
        Task<Fascicle> GetFascicleAsync(string odataQuery);
        Task<FascicleFolder> GetDefaultFascicleFolderAsync(Guid idFascicle);
        Task<ICollection<Fascicle>> GetFasciclesAsync(string odataQuery);
        Task<TenantAOO> GetTenantAOOAsync(Guid uniqueId);
        Task<ICollection<TenantAOO>> GetTenantsAOOAsync(string odataQuery);
        Task<ICollection<Tenant>> GetTenantsAsync();
        Task<Tenant> GetTenantAsync(Guid uniqueId, string optionalFilter = null);
        Task<ICollection<Tenant>> GetTenantsAsync(string odataQuery);
        Task<WorkflowActivity> GetWorkflowActivityAsync(Guid uniqueId, string optionalFilter = null);
        Task<ICollection<FascicleDocumentUnit>> GetFascicleDocumentUnitAsync(string odataQuery);
        Task<Role> GetRoleAsync(short entityShortId);
        Task<ICollection<Category>> GetCategoryAsync(string odataQuery);
        Task<ICollection<Contact>> GetContactAsync(string odataQuery);
        Task<ICollection<Container>> GetContainerAsync(short containerId);
        Task<ICollection<PECMailBox>> GetPECMailBoxAsync(short pecMailBoxId);
        Task<ICollection<Protocol>> GetProtocolAsync(string odataQuery);
        /// <summary>
        /// <seealso cref="ComunicationType"/>
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="comunicationType"><see cref="ComunicationType"/></param>
        /// <returns></returns>
        Task<ICollection<ProtocolContact>> GetProtocolContactsAsync(Guid uniqueId, string comunicationType);
        /// <summary>
        /// <seealso cref="ComunicationType"/>
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="comunicationType"><see cref="Use helper ComunicationType"/></param>
        /// <returns></returns>
        Task<ICollection<ProtocolContactManual>> GetProtocolContactManualsAsync(Guid uniqueId, string comunicationType);
        Task<ICollection<ProtocolLog>> GetProtocolLogAsync(string odataQuery);
        Task<ICollection<ResolutionLog>> GetResolutionLogAsync(string odataQuery);
        Task<ICollection<DocumentSeriesItemLog>> GetDocumentSeriesItemLogAsync(string odataQuery);
        Task<ICollection<MetadataRepository>> GetMetadataRepositoryAsync(string odataQuery);
        Task<ICollection<UDSRepository>> GetUDSRepository(string name);
        Task<ICollection<UDSRepository>> GetUDSRepository(Guid uniqueId);
        Task<ICollection<UDSDocumentUnit>> GetUDSDocumentUnits(Guid idUDS, bool expandFascicle, bool expandUDSRepository);
        Task<ICollection<UDSRole>> GetUDSRoles(Guid idUDS);
        Task<ICollection<UDSContact>> GetUDSContacts(Guid idUDS);
        Task<ICollection<PECMail>> GetPECMailFromProtocol(Guid uniqueIdProtocol);
        Task<ICollection<PECMail>> GetPECMailFromReceiptIdentification(string referenceToPECMessageId);
        Task<ICollection<PECMailAttachment>> GetPECMailAttachmentFromPECMailId(int entityId, string attachmentName);
        Task<ICollection<UDSPECMail>> GetUDSPECMails(Guid idUDS);
        Task<ICollection<UDSMessage>> GetUDSMessages(Guid idUDS);
        Task<Dictionary<string, object>> GetUDSByInvoiceFilename(string controllerName, string invoiceFilename, string invoiceNumber, bool onlyValidInvoiceState, Dictionary<int, Guid> documents);
        Task<Dictionary<string, object>> GetRejectedUDSByInvoiceFilename(string controllerName, string invoiceFilename, Dictionary<int, Guid> documents);
        Task<Dictionary<string, object>> GetUDSByInvoiceFilename(string controllerName, string invoiceFilename, bool onlyValidInvoiceState, Dictionary<int, Guid> documents);
        Task<Dictionary<string, object>> GetUDSByDocumentFilename(string controllerName, string documentName, Dictionary<int, Guid> documents);
        Task<ICollection<UDSDocumentUnit>> GetUDSDocumentUnitFromDocumentUnitId(Guid idDocumentUnit);
        Task<Dictionary<string, object>> GetUDS(string controllerName, Guid idUds, Dictionary<int, Guid> documents);
        Task<ICollection<Collaboration>> GetCollaborationAggregatesAsync(int collaborationId);
        Task<Collaboration> GetCollaborationAsync(int collaborationId, bool expandWorkflowActivities = false);
        Task<ICollection<WorkflowActivity>> GetWorkflowAuthorizedActivitiesByDocumentUnitAsync(Guid documentUnitId, string account, string workflowName);
        Task<ICollection<WorkflowActivity>> GetWorkflowActivitiesByPropertyAsync(string propertyName, int value);
        Task<ICollection<WorkflowActivity>> GetWorkflowActivitiesByPropertyAsync(string propertyName, string value);
        Task<ICollection<WorkflowActivity>> GetWorkflowActivitiesByPropertyAsync(string propertyName, Guid value);
        Task<WorkflowResult> StartWorkflow(WorkflowStart workflowStart);
        Task<WorkflowResult> WorkflowNotify(WorkflowNotify workflowNotify);
        Task<DomainUserModel> GetUserAsync(string user);
        Task<ICollection<Contact>> GetProtocolContactSendersAsync(Protocol protocol);
        Task<ICollection<ProtocolContactManual>> GetProtocolContactManualSendersAsync(Protocol protocol);
        Task<ODataModel<ProtocolContactManual>> GetProtocolManualContactsWithFiscalCodeAsync(short containerId, DateTimeOffset fromDate, short docTypeCode, int protocolType, string comunicationType, int skip, int top);
        Task<ODataModel<Protocol>> GetProtocolsByManualContactWithFiscalCodeAsync(string fiscalCode, short containerId, DateTimeOffset fromDate, short doctypeCode, int protocolType, string comunicationType, int skip, int top);
        Task<TaskHeaderProtocol> GetTaskHeaderProtocolAsync(Protocol protocol);
        Task<ICollection<T>> GetAsync<T>()
            where T : class;
        Task<ICollection<T>> GetAsync<T>(string odataQuery)
            where T : class;
        Task<TResult> GetAsync<T, TResult>(string controllerName, string odataQuery)
            where T : class
            where TResult : class;
        Task<T> PostAsync<T>(T model, InsertActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class;
        Task<TResult> PostAsync<T, TResult>(T model, bool retryPolicyEnabled = false)
            where T : class
            where TResult : class;
        Task SendCommandAsync<T>(T model) where T : Command;
        Task<ServiceBusMessage> SendEventAsync<T>(T model) where T : Event;
        Task<T> PutAsync<T>(T model, UpdateActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class;
        Task<T> DeleteAsync<T>(T model, DeleteActionType? actionType = null, bool retryPolicyEnabled = false)
            where T : class;
        Task<Category> GetCategoryAsync(int idCategory);
        Task<int?> GetParameterWorkflowLocationIdAsync();
        Task<int?> GetParameterContactAOOParentIdAsync();
        Task<string> GetParameterSignatureTemplate();
        Task<int?> GetParameterFascicleAutoCloseThresholdDaysAsync();
        Task<Fascicle> FindFascicle(FascicleFinderModel finderModel);
        Task<Dossier> FindDossier(DossierFinderModel finderModel);
        Task<Dossier> GetDossierAsync(Guid uniqueId, string optionalFilter = null);
        Task<ICollection<ProtocolContactManual>> GetProtocolContactManualsAsync(string odataQuery);
    }
}
