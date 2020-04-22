using System;
using System.ComponentModel;
using System.Configuration.Install;

namespace JeepService.JeepService.DashboardService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {

        public ProjectInstaller()
        {
            InitializeComponent();
            BeforeInstall += ProjectInstallerBeforeInstall;
            BeforeUninstall += ProjectInstallerBeforeUninstall;
        }

        void ProjectInstallerBeforeUninstall(object sender, InstallEventArgs e)
        {
            // Configure Account for Service Process. 
            switch (Context.Parameters["Account"])
            {
                case "LocalService":
                    serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService;
                    break;
                case "LocalSystem":
                    serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
                    break;
                case "NetworkService":
                    serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.NetworkService;
                    break;
                case "User":
                    serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.User;
                    serviceProcessInstaller1.Username = Context.Parameters["UserName"];
                    serviceProcessInstaller1.Password = Context.Parameters["Password"];
                    break;
            }
            // Configure ServiceName 
            if (!String.IsNullOrEmpty(Context.Parameters["ServiceName"]))
            {
                serviceInstaller1.ServiceName = Context.Parameters["ServiceName"];
                serviceInstaller1.DisplayName = Context.Parameters["ServiceName"];
            }
        }

        void ProjectInstallerBeforeInstall(object sender, InstallEventArgs e)
        {
            if (!String.IsNullOrEmpty(Context.Parameters["ServiceName"]))
            {
                serviceInstaller1.ServiceName = Context.Parameters["ServiceName"];
            }
        }
    }
}
