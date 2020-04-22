using System;

namespace VecompSoftware.DocSuiteWeb.DTO.Collaborations
{
    public class CollaborationDocument
    {
        public string DocumentName { get; set; }
        public string VersioningDocumentGroup { get; set; }
        public string BiblosSerializeKey { get; set; }
        public Guid? IdBiblosDocument { get; set; }
        public int? IdCollaboration { get; set; }
    }
}
