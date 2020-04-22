using System;
using System.Web;
using VecompSoftware.Services.Logging;
using AmministrazioneTrasparente.Code;
using System.Collections.Generic;
using AmministrazioneTrasparente.WSSeries;

namespace AmministrazioneTrasparente
{
    public class Global : HttpApplication
    {
        public const string LoggerName = "Application";

        protected void Application_Start(object sender, EventArgs e)
        {
            FileLogger.Initialize();
            FileLogger.Debug(LoggerName, "Application Start");
            new SqLiteBase().Initialize();
            //Inizializzo la cache al ClientBase
            WSSeriesClient.CacheSetting = System.ServiceModel.CacheSetting.AlwaysOn;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            ICollection<DocumentSeriesHeader> headerItems = Singleton.Instance.DocumentSeriesHeaders;
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
            var ex = Server.GetLastError();
            FileLogger.Error(LoggerName, "Errore applicativo non previsto.", ex);
        }
    }
}