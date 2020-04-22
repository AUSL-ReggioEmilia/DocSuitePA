using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Documents.Signs
{
    public class UserProfile
    {
        public ProviderSignType DefaultProvider { get; set; }
        public Dictionary<ProviderSignType, RemoteSignProperty> Value { get; set; }

    }
}
