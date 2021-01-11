using System;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityCollaborationDto
    {
        public Guid? UDSCollaborationId { get; set; }

        public int? IdCollaboration { get; set; }

        public Guid? UniqueId { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }
    }
}
