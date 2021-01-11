using System.Linq;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class APIService : IAPIService
    {
        public bool IsAlive()
        {
            return true;
        }

        public string GetAvailable()
        {
            try
            {
                var available = FacadeFactory.Instance.APIProviderFacade.GetAll().Where(p => p.IsEnabled);
                return JsonConvert.SerializeObject(available).ToBase64();
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }
    }
}
