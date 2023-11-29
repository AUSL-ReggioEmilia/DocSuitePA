using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public interface IDecryptedParameterEnvService : IParameterEnvBaseService
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
        bool ShibbolethEnabled { get; }
        short ContactAOOParentId { get; }
        string SignatureTemplate { get; }
        bool RoleGroupPECRightEnabled { get; }
        bool ForceDescendingOrderElements { get; }
        int FascicleAutoCloseThresholdDays { get; }
        int FascicleContactId { get; }
        int FascicleMiscellaneaLocation { get; }
        bool MultiAOOFascicleEnabled { get; }
        int AVCPDefaultCategoryId { get; }
        string AVCPDatasetUrlMask { get; }
        string AVCPEntePubblicatore { get; }
        string AVCPLicenza { get; }
        int AvcpDocumentSeriesId { get; }
        int AVCPResolutionType { get; }
        string AVCPInclusiveNumberMask { get; }
        ICollection<string> DocSuiteServiceAccounts { get; }
        string BasicPersonSearcherKey { get; }
        bool ForceProsecutableEnabled { get; }
        bool CollaborationMailEnabled { get; }
        int CollaborationSignatureType { get; }
    }
}
