using System.Collections.Generic;
using VecompSoftware.BPM.Integrations.Services.SignServices.ArubaService.Models;
using VecompSoftware.Helpers.EInvoice.Models;

namespace VecompSoftware.BPM.Integrations.Modules.TECHNOSKY.SAP.Invoice.Models
{
    public class InvoiceConfiguration
    {
        public string WorkflowRepositoryName { get; set; }
        public string UDSRepositoryName { get; set; }
        public short ProtocolContainerId { get; set; }
        public short ProtocolCategoryId { get; set; }
        public int ContactB2BParent { get; set; }
        public int ContactPAParent { get; set; }
        public short PECMailBoxId { get; set; }
        public string MailRecipients { get; set; }
        public SignInvoiceType SignInvoiceType { get; set; }
        public ArubaSignModel SignerParameter { get; set; }
        public List<short> AuthorizationRoles { get; set; }
        public Dictionary<string, InvoiceFilePersistance> InvoicePersistanceConfigurations { get; set; }
    }
}
