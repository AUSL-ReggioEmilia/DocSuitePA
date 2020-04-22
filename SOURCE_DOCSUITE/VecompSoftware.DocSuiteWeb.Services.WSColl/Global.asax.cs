using System;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.Services.WSColl
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
        }

    }
}