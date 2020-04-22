using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Data.SqlClient;
using System.IO;


namespace BiblosDS.WindowsService.WCFStampaConformeConverterHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.DisplayName = "BiblosDS_StampaConformeServiceConverter_" + Properties.Settings.Default.ServiceName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "BiblosDS_StampaConformeServiceConverter_" + Properties.Settings.Default.ServiceName;
            serviceInstaller.Description = "BiblosDS Server di conversione di stampa conforme (VecompSoftware)";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);

        }

        public override void Commit(IDictionary savedState)
        {
            //if (!this.Context.Parameters.ContainsKey("connstr") || string.IsNullOrEmpty(this.Context.Parameters["connstr"])
            //    || !this.Context.Parameters.ContainsKey("server") || string.IsNullOrEmpty(this.Context.Parameters["server"]))
            //{
            //    throw new Exception("Connection string or server not passed");
            //}
            ////Write on DB.
            //using (SqlConnection conn = new SqlConnection(this.Context.Parameters["connstr"]))
            //{
            //    conn.Open();
            //    try
            //    {
            //        using (SqlCommand cmd = conn.CreateCommand())
            //        {
            //            string[] splittedCompName = this.GetType().FullName.Split('.');
            //            cmd.CommandText = "INSERT INTO Component(IdComponent, Name, Server, InstallPath, Enable) "
            //                            + "VALUES('{0}', '{1}', '{2}', '{3}', 1)";
            //            cmd.CommandText = string.Format(cmd.CommandText,
            //                Guid.NewGuid(),
            //                splittedCompName[splittedCompName.Length - 2],
            //                this.Context.Parameters["server"],
            //                this.Context.Parameters["assemblypath"]);
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //    finally
            //    {
            //        conn.Close();
            //    }
            //}
        }
    }
}
