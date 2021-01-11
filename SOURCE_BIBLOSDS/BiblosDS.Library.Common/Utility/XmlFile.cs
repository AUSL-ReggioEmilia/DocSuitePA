using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace BiblosDS.Library.Common.Utility
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


    public static string Serialize(T value, Action<XmlWriter> writerCallBack = null)
    {
      if (value == null)
        return null;

      var serializer = new XmlSerializer(typeof(T));
      var settings = new XmlWriterSettings();
      settings.Encoding = Encoding.UTF8;
      settings.Indent = true;
      settings.OmitXmlDeclaration = false;

      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add(string.Empty, string.Empty);

      using (var textWriter = new StringWriterWithEncoding(Encoding.UTF8))
      {
        using (XmlWriter writer = XmlWriter.Create(textWriter, settings))
        {
          serializer.Serialize(writer, value, namespaces);

          if (writerCallBack != null)
            writerCallBack(writer);
        }
        return textWriter.ToString();
      }
    }


    public static void Serialize(T value, string filename, Action<XmlWriter> writerCallBack = null)
    {
      if (value == null)
        return;

      var settings = new XmlWriterSettings();
      settings.Encoding = Encoding.UTF8;
      settings.Indent = true;
      settings.OmitXmlDeclaration = false;

      XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
      namespaces.Add(string.Empty, string.Empty);

      using (XmlWriter writer = XmlWriter.Create(filename, settings))
      {
        var serializer = new XmlSerializer(typeof(T));

        if (writerCallBack != null)
          writerCallBack(writer);

        serializer.Serialize(writer, value, namespaces);
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

  public class CData : IXmlSerializable
  {
    private string _value;

    /// <summary>
    /// Allow direct assignment from string:
    /// CData cdata = "abc";
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator CData(string value)
    {
      return new CData(value);
    }

    /// <summary>
    /// Allow direct assigment to string
    /// string str = cdata;
    /// </summary>
    /// <param name="cdata"></param>
    /// <returns></returns>
    public static implicit operator string(CData cdata)
    {
      return cdata._value;
    }

    public CData()
      : this(string.Empty)
    {
    }

    public CData(string value)
    {
      _value = value;
    }

    public override string ToString()
    {
      return _value;
    }

    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(System.Xml.XmlReader reader)
    {
      _value = reader.ReadElementString();
    }

    public void WriteXml(System.Xml.XmlWriter writer)
    {
      //_value = _value.Replace(Convert.ToChar((byte)0x1F), ' ');
      writer.WriteCData(SanitizeXmlString(_value));
    }

    /// <summary>
    /// Remove illegal XML characters from a string.
    /// </summary>
    private string SanitizeXmlString(string xml)
    {
      if (xml == null)
      {
        throw new ArgumentNullException("xml");
      }

      StringBuilder buffer = new StringBuilder(xml.Length);

      foreach (char c in xml)
      {
        if (IsLegalXmlChar(c))
        {
          buffer.Append(c);
        }
      }

      return buffer.ToString();
    }

    /// <summary>
    /// Whether a given character is allowed by XML 1.0.
    /// </summary>
    private bool IsLegalXmlChar(int character)
    {
      return
      (
           character == 0x9 /* == '\t' == 9   */          ||
           character == 0xA /* == '\n' == 10  */          ||
           character == 0xD /* == '\r' == 13  */          ||
          (character >= 0x20 && character <= 0xD7FF) ||
          (character >= 0xE000 && character <= 0xFFFD) ||
          (character >= 0x10000 && character <= 0x10FFFF)
      );
    }

  }

}