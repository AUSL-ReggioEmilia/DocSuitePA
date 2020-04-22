using System;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.Services.Command;

namespace VecompSoftware.JeepService.Pec
{
    public class EventHelper
    {
        public Guid? ProtocolUniqueId { get; set; } = null;
        public short? ProtocolYear { get; set; } = null;
        public int? ProtocolNumber { get; set; } = null;
        public Guid? CollaborationUniqueId { get; set; } = null;
        public int? CollaborationId { get; set; } = null;
        public string CollaborationTemplateName { get; set; } = string.Empty;
        public PECMail PECMail { get; set; } = null;
        public PECMailReceipt PECMailReceipt { get; set; } = null;
        public IIdentityContext Identity { get; set; } = null;
    }
}
