using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace BiblosDS.WindowsService.WCFHost_DigitalSign
{
    [RunInstaller(true)]
    public partial class WCFHost_DigitalSign_Installer : System.Configuration.Install.Installer
    {
        public WCFHost_DigitalSign_Installer()
        {
            InitializeComponent();
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.DisplayName = "BiblosDS2010 DigitalSign";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "BiblosDS2010_DigitalSign";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
