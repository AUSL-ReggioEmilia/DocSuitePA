using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.IO;
using System.Configuration;


namespace BiblosDS.WindowsService.StampaConforme.Office.Converter
{
    [RunInstaller(true)]
    public partial class OfficeConverterInstaller : System.Configuration.Install.Installer
    {
        public OfficeConverterInstaller()
        {            
            InitializeComponent();
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            string baseDir = System.IO.Path.GetDirectoryName(a.Location);

            serviceInstaller.DisplayName = "BiblosDS_StampaConformeServiceConverter_Office";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "BiblosDS_StampaConformeServiceConverter_Office";
            serviceInstaller.Description = "BiblosDS Server di conversione di stampa conforme (VecompSoftware)";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
