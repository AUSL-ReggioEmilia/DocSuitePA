using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;

namespace BiblosDs.Document.AdminCentral.UControls
{
    public partial class UcConfigView : UserControl
    {
        public UcConfigView()
        {
            InitializeComponent();
            this.Load += new EventHandler(UcConfigView_Load);
        }

        void UcConfigView_Load(object sender, EventArgs e)
        {
            label1.Text = ConfigSettings.GetEndpointAddress();
        }
    }

    public class ConfigSettings
    {

        private static string NodePath = "//system.serviceModel//client//endpoint";
        private ConfigSettings() { }

        public static string GetEndpointAddress()
        {
            // load config document for current assembly
            XmlDocument doc = loadConfigDocument();

            // retrieve appSettings node
            var node = doc.SelectNodes(NodePath);

            if (node == null)
                return "Error. Could not find endpoint node in config file.";

            string result = "";
            foreach (XmlNode item in node)
            {
                result += item.Attributes["name"].Value + ": " + item.Attributes["address"].Value + Environment.NewLine;
            }

            return result;
        }       

        public static XmlDocument loadConfigDocument()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(getConfigFilePath());
                return doc;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
        }

        private static string getConfigFilePath()
        {
            return Assembly.GetExecutingAssembly().Location + ".config";
        }
    }
}
