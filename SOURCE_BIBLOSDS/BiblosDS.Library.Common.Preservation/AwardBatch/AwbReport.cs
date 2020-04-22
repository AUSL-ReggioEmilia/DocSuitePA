using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BiblosDS.Library.Common.Preservation.AwardBatch
{
  [XmlRootAttribute(ElementName = "AwardBatchReport", IsNullable = false)]
  public class AwbReport
  {
    private string sourceFile = "";

    [XmlElementAttribute("Lotto")]
    public AwbBatch[] Batches { get; set; }

    public static AwbReport Load(string fileName)
    {
      var res = XmlFile<AwbReport>.Load(fileName, "");
      res.sourceFile = fileName;
      return res;
    }

    public void Save(string xslFilename)
    {
      XmlFile<AwbReport>.Serialize(this, this.sourceFile, (w => {
        w.WriteProcessingInstruction("xml-stylesheet", String.Format("type=\"text/xsl\" href=\"{0}\"",
          Path.GetFileName(xslFilename)));
      }) );
    }

    public void SaveAs(string fileName, string xslFilename)
    {
      this.sourceFile = fileName;
      XmlFile<AwbReport>.Serialize(this, fileName, (w =>
      {
        w.WriteProcessingInstruction("xml-stylesheet", String.Format("type=\"text/xsl\" href=\"{0}\"",
          Path.GetFileName(xslFilename) ));
      }));
    }

  }
  

  public class AwbBatch
  {
    public string Descrizione { get; set; }

    [XmlElementAttribute("File")]
    public AwbBatchFile[] Files { get; set; }
  }


  public class AwbBatchFile
  {
    [XmlElementAttribute("Attributo")]
    public AwbBatchFileAttribute[] Attributes { get; set; }
  }
  

  public class AwbBatchFileAttribute
  {
    public string Nome { get; set; }
    public CData Valore { get; set; }
  }
}
