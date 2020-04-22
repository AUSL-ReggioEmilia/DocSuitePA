using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public interface IParameterEnvService : IModelService<ParameterEnv>
    {
        short CollaborationLocationId { get; }
        short WorkflowLocationId { get; }
        short MessageLocationId { get; }
        short UDSLocationId { get; }
        short TemplateReportLocationId { get; }
        short ProcessContainerId { get; }
        short ProcessRoleId { get; }
        string CurrentTenantName { get; }
        Guid CurrentTenantId { get; }
        TenantModel CurrentTenantModel { get; }
        IList<TenantModel> TenantModels { get; }
        AbsentManagerCertificateModel AbsentManagersCertificates { get; }
        bool ArchiveSecurityGroupsGenerationEnabled { get; }
        bool SecureDocumentSignatureEnabled { get; }
        short SecurePaperServiceId { get; }
        string SecurePaperCertificateThumbprint { get; }
        string SecurePaperServiceUrl { get; }
        short SignatureProtocolType { get; }
        string SignatureProtocolString { get; }
        string SignatureProtocolMainFormat { get; }
        string SignatureProtocolAttachmentFormat { get; }
        string SignatureProtocolAnnexedFormat { get; }
        string CorporateAcronym { get; }
        string CorporateName { get; }
        bool FascicleContainerEnabled { get; }
        bool RoleContactEnabled { get; }
        string GroupTblContact { get; }
        bool ProcessEnabled { get; }
    }
}