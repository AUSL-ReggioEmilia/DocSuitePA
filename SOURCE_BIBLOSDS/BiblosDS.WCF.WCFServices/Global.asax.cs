using System;
using log4net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using log4net.Config;
using System.Configuration; 

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

            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    //(CDLTLL) Configuration for Windows Azure settings 
                    //CloudStorageAccount.SetConfigurationSettingPublisher(
                    //    (configName, configSettingPublisher) =>
                    //    {
                    //        var connectionString =
                    //            RoleEnvironment.GetConfigurationSettingValue(configName);
                    //        configSettingPublisher(connectionString);
                    //    }
                    //);

                    // piccoli 20130422 se il role enviroment non è disponibile o l'accesso è in eccezione, legge dal config 
                    CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                    {
                        bool isRoleEnviromentAvailable = false;
                        try
                        {
                            isRoleEnviromentAvailable = RoleEnvironment.IsAvailable;
                        }
                        catch
                        {
                            isRoleEnviromentAvailable = false;
                        }

                        string value = "";
                        if (isRoleEnviromentAvailable)
                            value = RoleEnvironment.GetConfigurationSettingValue(configName);
                        else
                            value = ConfigurationManager.AppSettings[configName];
                        configSetter(value);
                    });

                    logger.InfoFormat("DeploymentId {0}", RoleEnvironment.DeploymentId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);                
            }            
              
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
