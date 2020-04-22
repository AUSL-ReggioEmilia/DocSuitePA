using VecompSoftware.StampConforme.Models.SecureDocument;

namespace VecompSoftware.StampaConforme.Interfaces.Common.Services
{
    public interface ISecureDocumentService
    {
        SecureDocumentModel Create(SecureDocumentModel documentModel);
        SecureDocumentModel Update(SecureDocumentModel documentModel);
        void Delete(SecureDocumentModel documentModel);
        void Upload(SecureDocumentModel documentModel);
    }
}
