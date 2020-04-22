using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Entity;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;
using VecompSoftware.DocSuiteWeb.Model.Securities;
using VecompSoftware.DocSuiteWeb.Model.Workflow;
using VecompSoftware.Services.Command.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.ServiceBus.WebAPI
{
    public interface IWebAPIClient
    {
        Task<bool> PushEventAsync<TEvent>(TEvent evt)
            where TEvent : IEvent;

        Task<bool> PushCommandAsync<TCommand>(TCommand command)
            where TCommand : ICommand;

        Task<TEntity> PostEntityAsync<TEntity>(TEntity entity, string actionType = "")
            where TEntity : DSWBaseEntity;

        Task<TEntity> PutEntityAsync<TEntity>(TEntity entity, string actionType = "")
            where TEntity : DSWBaseEntity;

        Task<TEntity> DeleteEntityAsync<TEntity>(TEntity model, string actionType = "")
            where TEntity : DSWBaseEntity;

        Task<UDSRepository> GetCurrentUDSRepositoryAsync(string udsRepositoryName);

        Task<UDSSchemaRepository> GetCurrentUDSSchemaRepositoryAsync();

        Task<Fascicle> GetFascicleAsync(Guid documentUnitId);

        Task<Location> GetLocationAsync(short idLocation);

        Task<Category> GetCategoryAsync(int idCategory);

        Task<Container> GetContainerAsync(int idContainer);

        Task<Container> CreateContainerAsync(Container container);

        Task<DocumentUnit> GetDocumentUnitAsync(DocumentUnit documentUnit);

        Task<DocumentUnit> PutDocumentUnitAsync(DocumentUnit documentUnit);

        Task<DocumentUnit> PostDocumentUnitAsync(DocumentUnit documentUnit);        

        Task<TFascicolable> PostFascicolableEntityAsync<TFascicolable, TEntity>(TFascicolable fascicolableEntity)
            where TFascicolable : DSWBaseEntity, IDSWEntityFascicolable<TEntity>
            where TEntity : DSWBaseEntity;

        Task<TFascicolable> PutFascicolableEntityAsync<TFascicolable, TEntity>(TFascicolable fascicolableEntity)
            where TFascicolable : DSWBaseEntity, IDSWEntityFascicolable<TEntity>
            where TEntity : DSWBaseEntity;

        Task<PECMail> GetPECMailAsync(int idPECMail);

        Task<Collaboration> GetCollaborationAsync(int idCollaboration);

        Task<ICollection<Fascicle>> GetPeriodicFasciclesAsync();

        Task<Fascicle> GetAvailablePeriodicFascicleByDocumentUnitAsync(DocumentUnit documentUnit);

        Task<Role> GetRoleAsync(RoleModel roleModel);

        Task<DocumentSeriesItem> GetDocumentSeriesItemAsync(Guid uniqueId);

        Task<Resolution> GetResolutionAsync(Guid uniqueId);

        Task<Protocol> GetProtocolAsync(Guid uniqueId);

        Task<UDSRepository> GetUDSRepositoryAsync(Guid uniqueId);

        Task<WorkflowResult> StartWorkflowAsync(WorkflowStart entity);

        Task<DomainUserModel> GetSignerInformationAsync(string username, string domain);

        Task<CategoryFascicle> GetPeriodicCategoryFascicleByEnvironmentAsync(short idCategory, int environment);

        Task<CategoryFascicle> GetDefaultCategoryFascicleAsync(short idCategory);

        Task<DocumentUnitFascicleCategory> GetDocumentUnitFascicleCategoryAsync(Guid idDocumentUnit, short idCategory, Guid idFascicle);

        Task<bool> GetAutomaticSecurityGroupsEnabledAsync();

        Task<bool> GetSecureDocumentSignatureEnabledAsync();

        Task<BuildActionModel> PostBuilderAsync(BuildActionModel entity, Guid IdRepository);

        Task<ICollection<ProtocolLog>> GetProtocolLogAsync(string odataFilter);

        Task<FascicleFolder> GetDefaultFascicleFolderAsync(Guid idFascicle);

        Task<int> GetParameterSecurePaperServiceIdAsync();

        Task<string> GetParameterSecurePaperCertificateThumbprintAsync();

        Task<string> GetParameterSecurePaperServiceUrlAsync();

        Task<short> GetParameterSignatureProtocolTypeAsync();

        Task<string> GetParameterSignatureProtocolStringAsync();

        Task<string> GetParameterSignatureProtocolMainFormatAsync();

        Task<string> GetParameterSignatureProtocolAttachmentFormatAsync();

        Task<string> GetParameterSignatureProtocolAnnexedFormatAsync();

        Task<string> GetParameterCorporateAcronymAsync();

        Task<string> GetParameterCorporateNameAsync();
        Task<ICollection<UDSDocumentUnit>> GetUDSDocumentUnitRelationByID(Guid IdUDS);
    }
}