using System.Collections.Generic;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.Collaborations
{
    public class CollaborationInitializer
    {
        public string Subject { get; set; }
        public DocumentInfo MainDocument { get; set; }
        public IList<DocumentInfo> Attachments { get; set; }
        public IList<DocumentInfo> Annexed { get; set; }
    }
}
