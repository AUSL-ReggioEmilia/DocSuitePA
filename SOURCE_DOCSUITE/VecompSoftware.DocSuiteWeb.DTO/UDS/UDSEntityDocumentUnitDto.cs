using Newtonsoft.Json;
using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityDocumentUnitDto
    {
        public Guid? UDSDocumentUnitId { get; set; }

        public Guid? UniqueId { get; set; }

        public UDSRelationType RelationType { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }
    }
}
