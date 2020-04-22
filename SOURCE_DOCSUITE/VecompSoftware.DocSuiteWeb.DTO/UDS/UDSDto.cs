using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSDto
    {
        public UDSDto()
        {
            Authorizations = new List<UDSEntityRoleDto>();
            Contacts = new List<UDSEntityContactDto>();
            Documents = new List<UDSEntityDocumentDto>();
            Messages = new List<UDSEntityMessageDto>();
            PecMails = new List<UDSEntityPECMailDto>();
            DocumentUnits = new List<UDSEntityDocumentUnitDto>();
        }

        public Guid Id { get; set; }

        public int Year { get; set; }

        public int Number { get; set; }

        public short Status { get; set; }

        public string Subject { get; set; }

        public string CancelMotivation { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public UDSEntityCategoryDto Category { get; set; }

        public UDSEntityRepositoryDto UDSRepository { get; set; }

        public IList<UDSEntityRoleDto> Authorizations { get; set; }

        public IList<UDSEntityContactDto> Contacts { get; set; }

        public IList<UDSEntityDocumentDto> Documents { get; set; }

        public IList<UDSEntityMessageDto> Messages { get; set; }

        public IList<UDSEntityPECMailDto> PecMails { get; set; }

        public IList<UDSEntityDocumentUnitDto> DocumentUnits { get; set; }

        public UDSModel UDSModel { get; set; }

        public string FullNumber
        {
            get
            {
                return String.Format("{0}/{1:0000000}", Year, Number);
            }
        }

        public bool HasProtocol()
        {
            return DocumentUnits != null && DocumentUnits.Any(x => x.RelationType == Model.Entities.UDS.UDSRelationType.Protocol 
            || x.RelationType == Model.Entities.UDS.UDSRelationType.ArchiveProtocol 
            || x.RelationType == Model.Entities.UDS.UDSRelationType.ProtocolArchived);
        }
    }
}
