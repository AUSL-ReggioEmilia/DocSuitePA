using Newtonsoft.Json;
using System;
using VecompSoftware.Helpers.UDS;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    [Serializable]
    public class UDSEntityPECMailDto
    {
        public Guid? UDSPecMailId { get; set; }

        public int? IdPECMail { get; set; }

        public Guid? UniqueId { get; set; }

        public DateTimeOffset? RegistrationDate { get; set; }

    }
}
