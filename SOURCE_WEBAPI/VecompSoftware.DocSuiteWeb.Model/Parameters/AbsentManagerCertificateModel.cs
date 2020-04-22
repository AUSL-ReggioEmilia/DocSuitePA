using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters
{
    public class AbsentManagerCertificateModel
    {
        public IDictionary<string, short> Roles { get; set; }
        public IDictionary<string, CollaborationManagerModel> Managers { get; set; }
    }
}
