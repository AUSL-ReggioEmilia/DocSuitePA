using System;
using System.Configuration;

namespace VecompSoftware.NHibernateManager.Config
{
    public class SessionFactoryElement : ConfigurationElement
    {
        public SessionFactoryElement()
        {
        }

        public SessionFactoryElement(string name, string configPath)
        {
            Name = name;
            FactoryConfigPath = configPath;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true, DefaultValue = "Not Supplied")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("factoryConfigPath", IsRequired = true, DefaultValue = "Not Supplied")]
        public string FactoryConfigPath
        {
            get { return (string)this["factoryConfigPath"]; }
            set { this["factoryConfigPath"] = value; }
        }

        [ConfigurationProperty("isTransactional", IsRequired = false, DefaultValue = false)]
        public bool IsTransactional
        {
            get { return Convert.ToBoolean(this["isTransactional"]); }
            set { this["isTransactional"] = value; }
        }

        [ConfigurationProperty("cfg", IsRequired = false)]
        public string Configurator
        {
            get { return (string)this["cfg"]; }
            set { this["cfg"] = value; }
        }

    }
}
