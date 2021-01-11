using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BiblosDS.Library.Common.Objects
{
    [DataContract(Name = "Certificate", Namespace = "http://BiblosDS/2009/10/Certificate")]
    public partial class DocumentCertificate : BiblosDSObject
    {
        [DataMember]
        public Guid IdCertificate { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public bool IsOnDisk { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string FiscalCode { get; set; }
        [DataMember]
        public string Role { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Issuer { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public DateTime DateExpiration { get; set; }
        [DataMember]
        public DateTime DateValidFrom { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string CertificateVersion { get; set; }
        [DataMember]
        public string HeaderInfo { get; set; }

        public DocumentCertificate()
        {
        }

        public DocumentCertificate(Guid IdCertificate, string Name)
        {
            this.IdCertificate = IdCertificate;
            this.Name = Name;
        }

        public DocumentCertificate(int Level, string Type, string FiscalCode, string Role, string Description, string Issuser, string Email,
            DateTime DateExpiration, DateTime DateValidFrom)
        {
            this.Level = Level;
            this.Type = Type;
            this.FiscalCode = FiscalCode;
            this.Role = Role;
            this.Description = Description;
            this.Issuer = Issuser;
            this.Email = Email;
            this.DateExpiration = DateExpiration;
            this.DateValidFrom = DateValidFrom;
        }

        public DocumentCertificate(Guid IdCertificate)
        {
            this.IdCertificate = IdCertificate;
        }        
    }
}
