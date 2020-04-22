using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace BiblosDS.Library.Common.Utility
{
  public class XslFile
  {
    public static string TransformFile(string xmlFilename, string xslFilename)
    {
      return TransformXml(File.ReadAllText(xmlFilename, Encoding.UTF8), File.ReadAllText(xslFilename, Encoding.UTF8));
    }

    public static string TransformXml(string inputXml, string xsltString)
    {
      XslCompiledTransform transform = new XslCompiledTransform();
      using (XmlReader reader = XmlReader.Create(new StringReader(xsltString)))
      {
        transform.Load(reader);
      }

      using (var results = new StringWriterWithEncoding(Encoding.UTF8))
      {
        using (XmlReader reader = XmlReader.Create(new StringReader(inputXml)))
        {
          transform.Transform(reader, null, results);
        }
        return results.ToString();
      }
    }
  }
}
