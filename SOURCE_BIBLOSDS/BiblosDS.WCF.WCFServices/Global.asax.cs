using log4net;
using log4net.Config;
using System;
using System.Configuration;
using System.IO;

namespace BiblosDS.WCF.WCFServices
{
    public class Global : System.Web.HttpApplication
    {

        #region [ Fields ]

        ILog _logger;

        #endregion

        #region [ Properties ]

        private ILog logger
        {
            get
            {
                if (_logger == null)
                    _logger = LogManager.GetLogger(typeof(Global));
                return _logger;
            }
        }

        #endregion


        protected void Application_Start(object sender, EventArgs e)
        {            
            var configuration = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            XmlConfigurator.Configure(new Uri(configuration));

            logger.Info("Application_Start");
        }      

        protected void Session_Start(object sender, EventArgs e)
        {
            string sessionId = Session.SessionID;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            log4net.ILog logger = log4net.LogManager.GetLogger(GetType());
            logger.Fatal(Server.GetLastError());
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

    }
}
