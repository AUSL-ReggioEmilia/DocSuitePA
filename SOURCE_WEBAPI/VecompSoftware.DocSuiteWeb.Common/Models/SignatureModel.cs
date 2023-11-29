using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Common.Models
{
    public class SignatureModel
    {
        public SignatureModel()
        {
            RoleServiceCodes = new List<string>();
            CorporateAcronym = string.Empty;
            CorporateName = string.Empty;
            ContainerName = string.Empty;
            ContainerNote = string.Empty;
            DocumentType = SignatureDocumentType.None;
            RegistrationDate = DateTimeOffset.MinValue;
            Typology = ProtocolTypology.Inbound;
        }

        public string CorporateAcronym { get; set; }
        public string CorporateName { get; set; }
        public SignatureDocumentType DocumentType { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public string ContainerName { get; set; }
        public short ContainerId { get; set; }
        public string ContainerNote { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public ProtocolTypology Typology { get; set; }
        public List<string> RoleServiceCodes { get; set; }
        public int? AttachmentsCount { get; set; }
        public int? DocumentNumber { get; set; }
    }
}
