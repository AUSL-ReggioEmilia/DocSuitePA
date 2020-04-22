using System;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityDocumentDto
    {
        public Guid? UDSDocumentId { get; set; }

        public UDSDocumentType? DocumentType { get; set; }

        public Guid? IdDocument { get; set; }

        public string DocumentName { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }
    }
}
