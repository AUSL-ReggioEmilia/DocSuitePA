using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public class XmlValidator
    {
        private bool res = false;
        public List<string> Errors { get; set; }

        public bool ValidateXml(string xml, string sSchemaFile, string sTargetNS)
        {
            this.res = true;
            Errors = new List<string>();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(sTargetNS, sSchemaFile);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
            while (reader.Read()) { }
            if (reader != null)
                reader.Close();

            return this.res;
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            this.res = false;
            this.Errors.Add(args.Message);
        }

    }
}
