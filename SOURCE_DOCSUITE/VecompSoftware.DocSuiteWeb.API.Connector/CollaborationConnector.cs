using System.IO;
using VecompSoftware.DocSuiteWeb.API.Connector.CollaborationService;
using VecompSoftware.Helpers;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class CollaborationConnector : BaseConnector<CollaborationServiceClient>
    {
        #region [ Constructors ]

        public CollaborationConnector(string address) : base("VecompSoftware.DocSuiteWeb.API.CollaborationService", Path.Combine(address, "CollaborationService.svc")) { }

        #endregion

        #region [ Methods ]

        public static CollaborationConnector For(string address)
        {
            return new CollaborationConnector(address);
        }

        public override bool IsAlive()
        {
            return Client.IsAlive();
        }

        protected override void CreateClient()
        {
            Client = new CollaborationServiceClient(ConfigurationName, Address);
        }

        /// <summary>
        /// chiama il servizio WCF e deserializza la risposta
        /// </summary>
        /// <returns></returns>
        public APIResponse<CollaborationDTO[]> GetCollaborationsToAlert(bool checkExpiredCollaborations)
        {
            var json = Client.GetCollaborationsToAlert(checkExpiredCollaborations);
            return json.DeserializeAsResponse<CollaborationDTO[]>();
        }

        #endregion
    }
}
