using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;
using System.Xml;
using System.Data.SqlClient;


namespace BiblosDs.Document.AdminCentral
{
    [RunInstaller(true)]
    public partial class InstallerClass : Installer
    {
        public InstallerClass()
        {
            InitializeComponent();
        }

        protected void ModifyConfiguredConnectionString(string configFileName, string connStrAttributeKey)
        {
            if (string.IsNullOrEmpty(configFileName) || string.IsNullOrEmpty(connStrAttributeKey))
            {
                throw new Exception("Invalid parameters for connection string configuration");
            }

            string fName = Path.Combine(Path.GetDirectoryName(this.Context.Parameters["assemblypath"]),
                    configFileName);
            string attName = connStrAttributeKey.ToLower();

            if (!File.Exists(fName))
            {
                throw new Exception("Configuration file doesn't exists");
            }
            if (!this.Context.Parameters.ContainsKey("connstr") || string.IsNullOrEmpty(this.Context.Parameters["connstr"])
                || !this.Context.Parameters.ContainsKey("server") || string.IsNullOrEmpty(this.Context.Parameters["server"]))
            {
                throw new Exception("Connection string or server not passed");
            }
            //Write on File.
            XmlDocument doc = new XmlDocument();
            doc.Load(fName);
            var nodes = doc.SelectNodes("configuration/connectionStrings");
            if (nodes == null)
            {
                throw new Exception("No connection string configured");
            }
            bool found = false;
            foreach (XmlNode parent in nodes)
            {
                foreach (XmlNode child in parent.ChildNodes)
                {
                    if (child.Attributes["name"].Value.ToLower() == attName)
                    {
                        child.Attributes["connectionString"].Value = this.Context.Parameters["connstr"];
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            doc.Save(fName);
            //Write on DB.
            using (SqlConnection conn = new SqlConnection(this.Context.Parameters["connstr"]))
            {
                conn.Open();
                try
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        string[] splittedCompName = this.GetType().FullName.Split('.');
                        cmd.CommandText = "INSERT INTO Component(IdComponent, Name, Server, InstallPath, Enable) "
                                        + "VALUES('{0}', '{1}', '{2}', '{3}', 1)";
                        cmd.CommandText = string.Format(cmd.CommandText,
                            Guid.NewGuid(),
                            splittedCompName[splittedCompName.Length - 2],
                            this.Context.Parameters["server"],
                            this.Context.Parameters["assemblypath"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override void Commit(IDictionary savedState)
        {
            //this.ModifyConfiguredConnectionString("BiblosDs.Document.AdminCentral.exe.config",
            //    "DocumentConnectionString");
        }
    }
}
