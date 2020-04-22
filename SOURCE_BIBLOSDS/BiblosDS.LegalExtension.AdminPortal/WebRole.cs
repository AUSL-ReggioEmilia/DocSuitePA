using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using log4net;
using Microsoft.WindowsAzure;

namespace BiblosDS.LegalExtension.AdminPortal
{
    public class WebRole : RoleEntryPoint
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(WebRole));

        public override bool OnStart()
        {
            Initialize();
            if (!log4net.LogManager.GetRepository().Configured)
                log4net.Config.XmlConfigurator.Configure();
            logger.Info("OnStart");
            logger.InfoFormat("DeploymentId {0}", RoleEnvironment.DeploymentId);
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
        }


    }
}