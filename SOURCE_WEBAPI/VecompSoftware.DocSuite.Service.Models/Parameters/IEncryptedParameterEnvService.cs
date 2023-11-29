namespace VecompSoftware.DocSuite.Service.Models.Parameters
{
    public interface IEncryptedParameterEnvService : IParameterEnvBaseService
    {
        string CollaborationLocationId { get; }
        string WorkflowLocationId { get; }
        string MessageLocationId { get; }
        string UDSLocationId { get; }
        string TemplateReportLocationId { get; }
        string ProcessContainerId { get; }
        string ProcessRoleId { get; }
        string TenantModels { get; }
        string AbsentManagersCertificates { get; }
        string ArchiveSecurityGroupsGenerationEnabled { get; }
        string SecureDocumentSignatureEnabled { get; }
        string SecurePaperServiceId { get; }
        string SignatureProtocolType { get; }
        string SignatureProtocolString { get; }
        string SignatureProtocolMainFormat { get; }
        string SignatureProtocolAttachmentFormat { get; }
        string SignatureProtocolAnnexedFormat { get; }
        string CorporateAcronym { get; }
        string CorporateName { get; }
        string FascicleContainerEnabled { get; }
        string RoleContactEnabled { get; }
        string GroupTblContact { get; }
        string ProcessEnabled { get; }
        string ShibbolethEnabled { get; }
        string ContactAOOParentId { get; }
        string SignatureTemplate { get; }
        string RoleGroupPECRightEnabled { get; }
        string ForceDescendingOrderElements { get; }
        string FascicleAutoCloseThresholdDays { get; }
        string FascicleContactId { get; }
        string FascicleMiscellaneaLocation { get; }
        string MultiAOOFascicleEnabled { get; }
        string AVCPDefaultCategoryId { get; }
        string AVCPDatasetUrlMask { get; }
        string AVCPEntePubblicatore { get; }
        string AVCPLicenza { get; }
        string AvcpDocumentSeriesId { get; }
        string AVCPResolutionType { get; }
        string AVCPInclusiveNumberMask { get; }
        string DocSuiteServiceAccounts { get; }
        string BasicPersonSearcherKey { get; }
        string ForceProsecutableEnabled { get; }
        string CollaborationMailEnabled { get; }
        string CollaborationSignatureType { get; }
    }
}
