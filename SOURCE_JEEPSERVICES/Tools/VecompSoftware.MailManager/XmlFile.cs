using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace VecompSoftware.MailManager
{
    public class XmlFile<T>
    {
        public static T Load(string sFilePath, string sRoot = "")
        {
            var doc = new XmlDocument();
            doc.Load(sFilePath);
            return Deserialize(doc, sRoot);
        }

        public static T LoadXml(string xml, string sRoot)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return Deserialize(doc, sRoot);
        }


        public static string Serialize(T value)
        {
            if (value == null)
                return null;

            XmlWriterSettings settings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true, OmitXmlDeclaration = false};

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriterWithEncoding textWriter = new StringWriterWithEncoding(Encoding.UTF8))
            using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(xmlWriter, value, namespaces);
                return textWriter.ToString();
            }
        }


        public static void Serialize(T value, string filename)
        {
            if (value == null)
                return;

            var settings = new XmlWriterSettings {Encoding = Encoding.UTF8, Indent = true, OmitXmlDeclaration = false};

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = XmlWriter.Create(filename, settings))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, value, namespaces);
            }
        }



        private static T Deserialize(XmlDocument doc, string sRoot)
        {
            XmlNode node = doc.ChildNodes[1];
            if (sRoot != String.Empty)
            {
                node = doc.SelectSingleNode("//" + sRoot);
            }

            StringReader reader = new StringReader(node.OuterXml);
            XmlSerializer x = new XmlSerializer(typeof(T));
            T obj = (T)x.Deserialize(reader);
            return obj;
        }
    }
}