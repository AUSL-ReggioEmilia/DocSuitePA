using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BiblosDS.Library.Common.Preservation.IpdaDoc
{
  [XmlRootAttribute(ElementName = "IPdA", IsNullable = false)]
  public class Ipda
  {
    private string sourceFile = "";

    [XmlIgnore]
    public string xmlFieldId = "";

    [XmlElementAttribute("DescGenerale")]
    public DescGenerale descGenerale { get; set; }

    [XmlElementAttribute("PdA")]
    public Pda pda { get; set; }

    [XmlElementAttribute("FileGruppo")]
    public FileGruppo fileGruppo { get; set; }

    [XmlElementAttribute("Processo")]
    public Processo processo { get; set; }
    
    public static Ipda Load(string fileName)
    {
      var res = XmlFile<Ipda>.Load(fileName, "");
      res.sourceFile = fileName;
      return res;
    }

    public void Save()
    {
      XmlFile<Ipda>.Serialize(this, this.sourceFile);
    }

    public void SaveAs(string fileName)
    {
      this.sourceFile = fileName;
      XmlFile<Ipda>.Serialize(this, fileName);
    }

  }

  public class DescGenerale
  {
    [XmlElementAttribute("ID")]
    public string id { get; set; }

    [XmlElementAttribute("Applicazione")]
    public DescApplicazione applicazione { get; set; }

    [XmlElementAttribute("ExtraInfo")]
    public DgExtraInfo extraInfo { get; set; }

  }

  public class DescApplicazione
  {
    [XmlElementAttribute("ApplicazioneNome")]
    public string nome { get; set; }

    [XmlElementAttribute("ApplicazioneVersione")]
    public string versione { get; set; }

    [XmlElementAttribute("ApplicazioneProduttore")]
    public string produttore { get; set; }
  }

  public class DgExtraInfo
  {
    [XmlElementAttribute("MetadatiEsterni")]
    public DgMetadatiEsterni[] metadatiEsterniList { get; set; }

    [XmlElementAttribute("MetadatiIntegrati")]
    public DgMetadatiIntegrati metadatiIntegrati { get; set; }
  }

  public class DgMetadatiEsterni 
  {
    [XmlElementAttribute("ID")]
    public string id { get; set; }

    [XmlElementAttribute("Indirizzo")]
    public string indirizzo { get; set; }

    [XmlElementAttribute("Impronta")]
    public string impronta { get; set; }
  }

  public class DgMetadatiIntegrati
  {
    [XmlElementAttribute("Metadati")]
    public DgmiMetadati metadati { get; set; }
  }

  public class DgmiMetadati
  {
    [XmlElementAttribute("Blocchi")]
    public DgmiBlocchi blocchi { get; set; }

    [XmlElementAttribute("Documenti")]
    public DgmiDocumenti documenti { get; set; }
  }

  public class DgmiBlocchi
  {
    [XmlElementAttribute("Blocco")]
    public DgmiBlocco[] bloccoList { get; set; }
  }

  public class DgmiBlocco
  {
    [XmlElementAttribute("ID")]
    public CData id { get; set; }

    [XmlElementAttribute("Documenti")]
    public string documenti { get; set; }

    [XmlElementAttribute("Dal")]
    public string dal { get; set; }

    [XmlElementAttribute("Al")]
    public string al { get; set; }
  }

  public class DgmiDocumenti
  {
    [XmlElementAttribute("Tipologia")]
    public CData tipologia { get; set; }

    [XmlElementAttribute("Numero")]
    public string numero { get; set; }

    [XmlElementAttribute("Dal")]
    public string dal { get; set; }

    [XmlElementAttribute("Al")]
    public string al { get; set; }
  }


  public class Pda
  {
    [XmlElementAttribute("ID")]
    public string id { get; set; }

    [XmlElementAttribute("ExtraInfo")]
    public PdaExtraInfo extraInfo { get; set; }
  }

  public class PdaExtraInfo
  {
    [XmlElementAttribute("Metadati")]
    public PdaMetadati metadati { get; set; }
  }

  public class PdaMetadati
  {
    [XmlElementAttribute("Attributo")]
    public PdaAttributo[] attributoList { get; set; }
  }

  public class PdaAttributo
  {
    [XmlAttributeAttribute("Nome")]
    public string nome { get; set; }
  }

  public class FileGruppo
  {
    [XmlElementAttribute("File")]
    public FileGruppoFile[] files { get; set; }
  }

  public class FileGruppoFile
  {
    [XmlElementAttribute("ID")]
    public string id { get; set; }

    [XmlElementAttribute("Impronta")]
    public string impronta { get; set; }

    [XmlElementAttribute("ExtraInfo")]
    public FgExtraInfo extraInfo { get; set; }
  }

  public class FgExtraInfo
  {
    [XmlElementAttribute("Metadati")]
    public FgMetadati metadati { get; set; }
  }

  
  public class FgMetadati
  {
    [XmlElementAttribute("Attributo")]
    public FgAttributo[] attributoList { get; set; }
  }

  public class FgAttributo
  {
    [XmlAttributeAttribute("Nome")]
    public string nome { get; set; }

    [XmlElementAttribute("Valore")]
    public CData valore { get; set; }
  }

  public class Processo
  {
    [XmlElementAttribute("Soggetto")]
    public Soggetto soggetto { get; set; }

    [XmlElementAttribute("Tempo")]
    public Tempo tempo { get; set; }

    [XmlElementAttribute("RiferimentoNormativo")]
    public string rifNormativo { get; set; }
  }

  public class Soggetto
  {
    [XmlElementAttribute("SoggettoID")]
    public CData soggettoID { get; set; }

    [XmlElementAttribute("SoggettoNome")]
    public SoggettoNome soggettoNome { get; set; }
  }

  public class SoggettoNome
  {
    [XmlElementAttribute("RagioneSociale")]
    public CData ragioneSociale { get; set; }
  }


  public class Tempo
  {
    [XmlElementAttribute("RiferimentoTemporale")]
    public string rifTemporale { get; set; }
  }
}
