using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BiblosDS.Library.Common.Preservation.IpdaDoc
{

  [Obsolete("Questa classe non viene utilizzata. E' stata create nell'eventualità di dover gestire i metadati minimi di un documento informatico. Utilizzare la classe Ipda")]
  [XmlRootAttribute(ElementName = "documento", IsNullable = false)]
  public class IpdaDocument
  {
    private string sourceFile = "";

    [XmlAttributeAttribute(AttributeName = "IDDocumento")]
    public string iddocumento { get; set; }

    public DateTime datachiusura { get; set; }
    public string oggettodocumento { get; set; }
    public SoggettoProduttore soggettoproduttore { get; set; }
    public Destinatario destinatario { get; set; }

    public static IpdaDocument Load(string fileName)
    {
      var res = XmlFile<IpdaDocument>.Load(fileName, "");
      res.sourceFile = fileName;
      return res;
    }

    public void Save()
    {
      XmlFile<IpdaDocument>.Serialize(this, this.sourceFile);
    }

    public void SaveAs(string fileName)
    {
      this.sourceFile = fileName;
      XmlFile<IpdaDocument>.Serialize(this, fileName);
    }
  }

  public class SoggettoProduttore
  {
    public string nome { get; set; }
    public string cognome { get; set; }
    public string codicefiscale { get; set; }
  }

  public class Destinatario
  {
    public string nome { get; set; }
    public string cognome { get; set; }
    public string codicefiscale { get; set; }
  }
}
