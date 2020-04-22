using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data.SqlClient;
using System.IO;
using System.ServiceProcess;
using System.Xml;


namespace BiblosDS.WindowsService.WCFHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {

        public ProjectInstaller()
        {
            InitializeComponent();
            this.BeforeInstall += new InstallEventHandler(ProjectInstaller_BeforeInstall);
        }

        #region [ Const ]

        const string DescriptionKey = "Description";
        const string DisplayNameKey = "DisplayName";
        const string ServiceNameKey = "ServiceName";

        #endregion

        #region [ Fields ]

        private StringDictionary _defaultParameters;

        #endregion

        #region [ Properties ]

        private StringDictionary defaultParameters
        {
            get
            {
                if (_defaultParameters == null)
                    initializeDefaultValues();
                return _defaultParameters;
            }
        }
        private StringDictionary contextParameters
        {
            get
            {
                if (this.Context != null && this.Context.Parameters != null && this.Context.Parameters.Count > 0)
                    return this.Context.Parameters;
                return null;
            }
        }

        #endregion

        private void initializeDefaultValues()
        {
            _defaultParameters = new StringDictionary
            {
                { DescriptionKey, "Dgroove Srl, BiblosDS WCFHost." },
                { DisplayNameKey, "BiblosDS2010" },
                { ServiceNameKey, "BiblosDS2010" }
            };

            // Se ho specificato solo ServiceName, inizializzo il default di DisplayName con lo stesso valore.
            if (contextParameters != null && !contextParameters.ContainsKey(DisplayNameKey) && contextParameters.ContainsKey(ServiceNameKey))
            {
                _defaultParameters[DisplayNameKey] = contextParameters[ServiceNameKey];
                Console.WriteLine("Parametro: {0}, Reimpostato valore di default: {1}", DisplayNameKey, defaultParameters[DisplayNameKey]);
            }

        }
        private string getValueOrDefaultParameter(string key)
        {
            if (contextParameters != null && contextParameters.ContainsKey(key))
            {
                Console.WriteLine("Parametro: {0}, Valore specificato: {1}", key, contextParameters[key]);
                return contextParameters[key];
            }
            if (defaultParameters.ContainsKey(key))
            {
                Console.WriteLine("Parametro: {0}, Valore di default: {1}", key, defaultParameters[key]);
                return defaultParameters[key];
            }
            Console.WriteLine("Parametro: {0} mancante o non valido", key);
            return string.Empty;
        }

        private void ProjectInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            var processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem,
                Password = null,
                Username = null
            };
            Installers.Add(processInstaller);

            var installer = new ServiceInstaller
            {
                Description = getValueOrDefaultParameter(DescriptionKey),
                DisplayName = getValueOrDefaultParameter(DisplayNameKey),
                ServiceName = getValueOrDefaultParameter(ServiceNameKey),
                StartType = ServiceStartMode.Automatic
            };
            Installers.Add(installer);
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
                        string original = child.Attributes["connectionString"].Value;
                        string toMatch = original.ToLower();
                        int index = toMatch.LastIndexOf("provider connection string");
                        int index2 = toMatch.LastIndexOf('"');
                        int lenght = index2 - index;
                        string result = toMatch.Substring(index, lenght);

                        int idx = result.IndexOf("data source=") + "data source=".Length, idx2 = result.IndexOf(";", idx), len = idx2 - idx;
                        result = result.Remove(idx, len).Insert(idx, this.Context.Parameters["server"]);
                        idx = result.IndexOf("user id=") + "User ID=".Length;
                        idx2 = result.IndexOf(";", idx);
                        len = idx2 - idx;
                        result = result.Remove(idx, len).Insert(idx, this.Context.Parameters["userid"]);
                        idx = result.IndexOf("password=") + "Password=".Length;
                        idx2 = result.IndexOf(";", idx);
                        len = idx2 - idx;
                        result = result.Remove(idx, len).Insert(idx, this.Context.Parameters["password"]);
                        idx = result.IndexOf("catalog=") + "catalog=".Length;
                        idx2 = result.IndexOf(";", idx);
                        len = idx2 - idx;
                        result = result.Remove(idx, len).Insert(idx, this.Context.Parameters["catalog"]);

                        original = original.Remove(index, lenght).Insert(index, result);

                        child.Attributes["connectionString"].Value = original;

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
            this.ModifyConfiguredConnectionString("BiblosDS.WindowsService.WCFHost.exe.config", "BiblosDS");
        }

    }
}
