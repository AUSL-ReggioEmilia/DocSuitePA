using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.AVCP
{

    public class XmlUtil
  {
    public static string Serialize<T>(T value, XmlSerializerNamespaces namespaces, string defaultNamespace)
    {
      if (value == null)
      {
        return null;
      }

      XmlSerializer serializer = new XmlSerializer(typeof(T), defaultNamespace);
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Encoding = Encoding.UTF8;
      settings.Indent = false;
      settings.OmitXmlDeclaration = false;

      using (StringWriter textWriter = new StringWriterWithEncoding(Encoding.UTF8))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
        {
          serializer.Serialize(xmlWriter, value, namespaces);
        }
        return textWriter.ToString();
      }
    }


    public class StringWriterWithEncoding : StringWriter
    {
      private Encoding myEncoding;
      public override Encoding Encoding
      {
        get
        {
          return myEncoding;
        }
      }
      public StringWriterWithEncoding(Encoding encoding)
        : base()
      {
        myEncoding = encoding;
      }
    }

    public static T Deserialize<T>(string xml)
    {
      if (string.IsNullOrEmpty(xml))
      {
        return default(T);
      }

      XmlSerializer serializer = new XmlSerializer(typeof(T));
      XmlReaderSettings settings = new XmlReaderSettings();
      using (StringReader textReader = new StringReader(xml))
      {
        using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
        {
          return (T)serializer.Deserialize(xmlReader);
        }
      }
    }

  }
}
