using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityDto
    {
        public UDSEntityDto()
        {
            Authorizations = new List<UDSEntityRoleDto>();
            Contacts = new List<UDSEntityContactDto>();
            Documents = new List<UDSEntityDocumentDto>();
            Messages = new List<UDSEntityMessageDto>();
            PecMails = new List<UDSEntityPECMailDto>();
            DocumentUnits = new List<UDSEntityDocumentUnitDto>();
        }

        [JsonProperty("UDSId")]
        public Guid? Id { get; set; }

        [JsonProperty("_year")]
        public int? Year { get; set; }

        [JsonProperty("_number")]
        public int? Number { get; set; }

        [JsonProperty("_status")]
        public short Status { get; set; }

        [JsonProperty("_cancelMotivation")]
        public string CancelMotivation { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public UDSEntityRepositoryDto UDSRepository { get; set; }

        [JsonProperty("_subject")]
        public string Subject { get; set; }

        public int? IdCategory { get; set; }

        public UDSEntityCategoryDto Category { get; set; }

        public UDSEntityCQRSDocumentUnitDto DocumentUnit { get; set; }

        public IList<UDSEntityDocumentDto> Documents { get; set; }

        public IList<UDSEntityContactDto> Contacts { get; set; }

        public IList<UDSEntityRoleDto> Authorizations { get; set; }

        public IList<UDSEntityMessageDto> Messages { get; set; }

        public IList<UDSEntityPECMailDto> PecMails { get; set; }

        public IList<UDSEntityDocumentUnitDto> DocumentUnits { get; set; }
    }    
}
