
using System;
using VecompSoftware.NHibernateManager;

namespace VecompSoftware.DocSuiteWeb.Services.WSSeries
{
    public class Global : System.Web.HttpApplication
    {

        public override void Init()
        {
            base.Init();
            EndRequest += Global_EndRequest;
        }

        void Global_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.CloseTransactionAndSessions();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}