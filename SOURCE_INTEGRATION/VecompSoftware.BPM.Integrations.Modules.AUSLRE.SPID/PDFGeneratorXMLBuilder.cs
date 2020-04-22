using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID
{
    public class PDFGeneratorXMLBuilder
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public PDFGeneratorXMLBuilder(string xsd, ICollection<IDocumentGeneratorParameter> parameters)
        {
            XSD = xsd;
            Parameters = parameters;
        }
        #endregion

        #region [ Properties ]
        public string XSD { get; private set; }

        public ICollection<IDocumentGeneratorParameter> Parameters { get; private set; }
        #endregion

        #region [ Methods ]
        private KeyValuePair<string, object> ParseParameter(IDocumentGeneratorParameter visitable)
        {
            if (visitable is BooleanParameter)
            {
                BooleanParameter b = visitable as BooleanParameter;
                return new KeyValuePair<string, object>(b.Name, b.Value);
            }
            if (visitable is CharParameter)
            {
                CharParameter ch = visitable as CharParameter;
                return new KeyValuePair<string, object>(ch.Name, ch.Value);
            }
            if (visitable is FloatParameter)
            {
                FloatParameter f = visitable as FloatParameter;
                return new KeyValuePair<string, object>(f.Name, f.Value);
            }
            if (visitable is GuidParameter)
            {
                GuidParameter g = visitable as GuidParameter;
                return new KeyValuePair<string, object>(g.Name, g.Value);
            }
            if (visitable is IntParameter)
            {
                IntParameter i = visitable as IntParameter;
                return new KeyValuePair<string, object>(i.Name, i.Value);
            }
            if (visitable is StringParameter)
            {
                StringParameter s = visitable as StringParameter;
                return new KeyValuePair<string, object>(s.Name, s.Value);
            }

            throw new DSWException(string.Concat("Parameter '", visitable.GetType().Name, "' is not correct"), null, DSWExceptionCode.Invalid);
        }

        public string GetGeneratedXML()
        {
            using (MemoryStream mstream = new MemoryStream(Encoding.UTF8.GetBytes(XSD)))
            using (StreamReader sreader = new StreamReader(mstream))
            using (XmlReader reader = XmlReader.Create(sreader))
            using (DataSet dataSet = new DataSet())
            {
                dataSet.ReadXmlSchema(reader);
                DataRow row;
                foreach (DataTable table in dataSet.Tables)
                {
                    row = table.NewRow();
                    foreach (KeyValuePair<string, object> p in Parameters.Select(s => ParseParameter(s)))
                    {
                        if (table.Columns.Contains(p.Key))
                        {
                            row[p.Key] = p.Value;
                        }
                    }
                    table.Rows.Add(row);
                }

                return dataSet.GetXml();
            }
        }
        #endregion
    }
}
