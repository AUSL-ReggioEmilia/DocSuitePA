using System.Collections.Generic;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public class UDSRelations
    {
        public IEnumerable<UDSDocument> Documents { get; set; }
        public IEnumerable<UDSAuthorization> Authorizations { get; set; }
        public IEnumerable<UDSContact> Contacts { get; set; }
        public IEnumerable<UDSMessage> Messages { get; set; }
        public IEnumerable<UDSPECMail> PECMails { get; set; }
        public IEnumerable<UDSProtocol> Protocols { get; set; }
        public IEnumerable<UDSResolution> Resolutions { get; set; }
        public IEnumerable<UDSCollaboration> Collaborations { get; set; }
    }
}
