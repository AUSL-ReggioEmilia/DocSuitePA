using VecompSoftware.DocSuiteWeb.DTO.Commons;

namespace VecompSoftware.DocSuiteWeb.DTO.Workflows
{
    public class RequestStatementResult
    {
        #region [ Properties ]
        public DocumentModel Document { get; set; }
        public bool IsSeletable => !IsLocked && !HasSecureDocumentReference && !HasSecurePaperReference;
        public bool IsLocked { get; set; }
        public bool HasSecureDocumentReference { get; set; }
        public bool HasSecurePaperReference { get; set; }
        #endregion
    }
}
