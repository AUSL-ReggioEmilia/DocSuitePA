using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;
using Microsoft.Web.Administration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace BiblosDS.WebRole
{
    public class WebRole : RoleEntryPoint
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
                    _logger = LogManager.GetLogger(typeof(WebRole));
                return _logger;
            }
        }

        #endregion

        public override bool OnStart()
        {
            Initialize();
            if (!LogManager.GetRepository().Configured)
            {
                var configuration = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                XmlConfigurator.Configure(new Uri(configuration));
            }

            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    string appPoolName =
                    serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_Web"]
                    .Applications.First().ApplicationPoolName;

                    var appPool = serverManager.ApplicationPools[appPoolName];

                    appPool.ProcessModel.UserName = Environment.MachineName + "\\" + RoleEnvironment.GetConfigurationSettingValue("BiblosDS_AppPoolUser");

                    appPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;

                    appPool.ProcessModel.Password = RoleEnvironment.GetConfigurationSettingValue("BiblosDS_AppPoolPsw");

                    serverManager.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            //CloudDriveManager.MountAllDrives(DRIVE_SETTINGS, DCACHE_NAME);
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.            
            return base.OnStart();
        }

        void Initialize()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            });
            logger.InfoFormat("DeploymentId {0}", RoleEnvironment.DeploymentId);
        }

        
    }
}
