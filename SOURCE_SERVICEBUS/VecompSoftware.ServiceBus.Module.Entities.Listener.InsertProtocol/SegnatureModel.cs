using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Protocols;

namespace VecompSoftware.ServiceBus.Module.Entities.Listener.InsertProtocol
{
    public class SegnatureModel
    {
        public SegnatureModel()
        {
            RoleServiceCodes = new List<string>();
            CorporateAcronym = string.Empty;
            CorporateName = string.Empty;
            ContainerName = string.Empty;
            ContainerNote = string.Empty;
            ContainerId = 0;
            DocumentType = SegnatureDocumentType.None;
            Year = 0;
            Number = 0;
            RegistrationDate = DateTimeOffset.MinValue;
            Typology = ProtocolTypology.Inbound;
        }

        public string CorporateAcronym { get; set; }
        public string CorporateName { get; set; }
        public SegnatureDocumentType DocumentType { get; set; }
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
