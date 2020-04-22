using System;
using System.ComponentModel.DataAnnotations;
//TODO: Non rimuovere lo using
using System.ComponentModel.DataAnnotations.Schema;

namespace VecompSoftware.DocSuiteWeb.UDS.Entities.Base
{
    public abstract class UDSDocumentBase
    {
        [Key]
        public Guid UDSDocumentId { get; set; }
        public Guid IdDocument { get; set; }
        public short DocumentType { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string DocumentName { get; set; }
        public Guid UDSId { get; set; }
        public string DocumentLabel { get; set; }
    }
}
