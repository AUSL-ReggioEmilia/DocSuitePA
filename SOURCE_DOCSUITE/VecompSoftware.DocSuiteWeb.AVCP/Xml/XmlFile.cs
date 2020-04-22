using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public class XmlFile<T>
  {
    public static T Load(string sFilePath, string sRoot)
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
      {
        return null;
      }

      var serializer = new XmlSerializer(typeof(T));
      var settings = new XmlWriterSettings();
      settings.Encoding = Encoding.UTF8;
      settings.Indent = true;
      settings.OmitXmlDeclaration = false;

      using (var textWriter = new VecompSoftware.DocSuiteWeb.AVCP.XmlUtil.StringWriterWithEncoding(Encoding.UTF8))
      {
        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
        {
          serializer.Serialize(xmlWriter, value);
        }
        return textWriter.ToString();
      }
    }


    private static T Deserialize(XmlDocument doc, string sRoot)
    {
      XmlNode node = doc.ChildNodes[1];
      if (sRoot != "")
        node = doc.SelectSingleNode("//" + sRoot);

      var reader = new StringReader(node.OuterXml);
      var x = new XmlSerializer(typeof(T));
      var obj = (T)x.Deserialize(reader);
      return obj;
    }
  }
}