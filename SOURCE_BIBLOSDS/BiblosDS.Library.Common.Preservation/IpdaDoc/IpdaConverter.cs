using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BiblosDS.Library.Common.Preservation.IpdaDoc
{
  public class IpdaConverter
  {
    private string closeFilepath = "";
    private string indexFilepath = "";
    private string xmlIndexFilepath = "";
    private string xslIndexFilepath = "";
    
    
    public Ipda ConvertCloseFile(string closeFilepath, string fieldId)
    {
      if (!File.Exists(closeFilepath))
        return null;

      this.closeFilepath = closeFilepath;
      Ipda ipda = new Ipda();

      string closeContents = "";
      using (System.IO.StreamReader sr = new System.IO.StreamReader(closeFilepath))
      {
        closeContents = sr.ReadToEnd();
      }

      string[] lines = closeContents.Split(new string[] {"\r\n"}, StringSplitOptions.None);
      this.indexFilepath = GetString(lines, "Nome File Indice:");
      this.xmlIndexFilepath = indexFilepath.Replace(".txt", ".xml");
      this.xslIndexFilepath = indexFilepath.Replace(".txt", ".xsl");

      string dataChiusura = GetString(lines, "Generato il ");

      ipda.descGenerale = new DescGenerale
      {
        id = GetString(lines, "File di chiusura dei documenti elencati nel percorso:"),
        applicazione = new DescApplicazione
        {
          nome = "Biblos Document Server",
          produttore = "Dgroove Srl",
          versione = "2015.0"
        }
      };

      ipda.descGenerale.extraInfo = new DgExtraInfo
      {
        metadatiIntegrati = ParseMetadatiIntegrati(lines),
        metadatiEsterniList = ParseMedatiEsterni(lines)
      };

      ipda.pda = new Pda
      {
        id = Path.GetFileNameWithoutExtension(this.indexFilepath),
        extraInfo = new PdaExtraInfo
        {
          metadati = new PdaMetadati
          {
            attributoList = ParseAttributi(lines).Select(p => new PdaAttributo { nome = PatchAttributeName(p) }).ToArray()
          }
        }
      };

      ipda.xmlFieldId = fieldId;

      FileGruppoFile[] files = ParseIndexFile(ipda);
      ipda.fileGruppo = new FileGruppo
      {
        files = files
      };

      //processo
      ipda.processo = new Processo
      {
        soggetto = new Soggetto
        {
          soggettoID = GetString(lines, "Responsabile Conservazione:")
        },
        tempo = new Tempo
        {
          rifTemporale = GetString(lines, "Generato il").Replace(" alle", "")
        },
        rifNormativo = ""
      };

      return ipda;
    }


    private DgMetadatiIntegrati ParseMetadatiIntegrati(string[] lines)
    {
      DgMetadatiIntegrati meta = null;

      try
      {
        List<DgmiBlocco> blocchiList = new List<DgmiBlocco>();

        string[] groups = GetLinesAfter(lines, "I documenti sono così suddivisi");
        foreach (var group in groups)
        {
          var tokens = Regex.Match(group, @"L'elenco  - gruppo (\S+) comprende (\d+) documenti dal numero (\d+) al numero (\d+)").Groups;
          blocchiList.Add(new DgmiBlocco
          {
            id = tokens[1].Value,
            documenti = tokens[2].Value,
            dal = tokens[3].Value,
            al = tokens[4].Value
          });
        }

        meta = new DgMetadatiIntegrati
        {
          metadati = new DgmiMetadati
          {
            blocchi = new DgmiBlocchi
            {
              bloccoList = blocchiList.ToArray()
            },
            documenti = new DgmiDocumenti
            {
              tipologia = GetString(lines, "Tipo Documenti:"),
              numero = GetString(lines, "Numero Documenti:"),
              dal = GetString(lines, "Data Primo Documento:"),
              al = GetString(lines, "Data Ultimo Documento:")
            }
          }
        };

        return meta;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Errore in decodifica MetadatiIntegrati:" + ex.Message);
      }
    }

    private DgMetadatiEsterni[] ParseMedatiEsterni(string[] lines)
    {
      try
      {

        List<DgMetadatiEsterni> list = new List<DgMetadatiEsterni>();

        list.Add(new DgMetadatiEsterni
        {
          id = "CHIUSURA_TXT",
          indirizzo = Path.GetFileName(this.closeFilepath),
          impronta = UtilityService.GetHash(this.closeFilepath, true)
        });

        list.Add(new DgMetadatiEsterni
        {
          id = "INDICE_TXT",
          indirizzo = Path.GetFileName(this.indexFilepath),
          impronta = GetString(lines, "Evidenza informatica file indice").Split(':')[1].Trim()
        });

        list.Add(new DgMetadatiEsterni
        {
          id = "INDICE_XML",
          indirizzo = Path.GetFileName(this.xmlIndexFilepath),
          impronta = GetString(lines, "Evidenza informatica file indice XML").Split(':')[1].Trim()
        });

        list.Add(new DgMetadatiEsterni
        {
          id = "INDICE_XSL",
          indirizzo = Path.GetFileName(this.xslIndexFilepath),
          impronta = GetString(lines, "Evidenza informatica file foglio di stile").Split(':')[1].Trim()
        });

        return list.ToArray();
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Errore in decodifica ParseMedatiEsterni:" + ex.Message);
      }
    }


    private string[] ParseAttributi(string[] lines)
    {
      try
      {
        string[] attributes = GetLinesAfter(lines, "Il file Indice esplicita i seguenti campi:");

        List<string> attrsList = new List<string>();
        foreach (var attr in attributes)
        {
          var tokens = Regex.Match(attr, @"- (.+)").Groups;
          attrsList.Add(tokens[1].Value.Trim());
        }

        return attrsList.ToArray();
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Errore in decodifica ParseAttributi:" + ex.Message);
      }
    }


    private string GetString(string[] lines, string startWith)
    {
      string str = lines.Where(s => s.ToLower().StartsWith(startWith.ToLower())).FirstOrDefault();
      if (str == null)
        throw new InvalidOperationException(String.Format("Nessuna linea che inizia con '{0}' trovata.", startWith));

      return str.Substring(startWith.Length).Trim();
    }


    private string[] GetLinesAfter(string[] lines, string afterStartWith)
    {
      afterStartWith = afterStartWith.ToLower();
      int idx = 0;

      for (idx = 0; idx < lines.Count(); idx++)
      {
        if (lines[idx].ToLower().StartsWith(afterStartWith))
          break;
      }

      idx++;

      if (idx >= lines.Count())
        throw new InvalidOperationException(String.Format("Nessuna linea che inizia con '{0}' trovata.", afterStartWith));

      List<string> list = new List<string>();
      while (idx < lines.Count())
      {
        string str = lines[idx].Trim();
        if (str == "")
          break;

        list.Add(str);
        idx++;
      }

      return list.ToArray();
    }

    private FileGruppoFile[] ParseIndexFile(Ipda ipda)
    {
      PdaAttributo[] attrs = ipda.pda.extraInfo.metadati.attributoList;

      XmlDocument indiceXml = new XmlDocument();
      List<FileGruppoFile> files = new List<FileGruppoFile>();

      indiceXml.Load(Path.Combine(Path.GetDirectoryName(this.closeFilepath), this.xmlIndexFilepath));

      XmlNodeList nList = indiceXml.SelectNodes("//File");
      foreach (XmlNode node in nList)
      {
        string id = GetNodeAttribute(node, ipda.xmlFieldId);

        string fileHash = "";
        fileHash = GetNodeAttribute(node, "ImprontaFileSHA1");
        if (fileHash == "")
          fileHash = GetNodeAttribute(node, "ImprontaFileSHA256");

        List<FgAttributo> attrList = attrs.Select(p => new FgAttributo
        {
          nome = p.nome,
          valore = GetNodeAttribute(node, PatchAttributeName(p.nome))
        }).ToList();

        files.Add(new FileGruppoFile
        {
          id = id,
          impronta = fileHash,
          extraInfo = new FgExtraInfo
          {
            metadati = new FgMetadati
            {
              attributoList = attrList.ToArray()
            }
          }
        });
      }

      return files.ToArray();
    }


    private string PatchAttributeName(string attr)
    {
      string _attr = attr.ToLower();

      if (_attr == "nomefile")
        return "NomeFileInArchivio";

      if (_attr == "impronta sha 256 (formato hex)")
        return "ImprontaFileSHA256";

      if (_attr == "impronta sha 1 (formato hex)")
        return "ImprontaFileSHA1";

      return attr;
    }


    private string GetNodeAttribute(XmlNode node, string name)
    {
      string qry = String.Format("Attributo[@Nome = \"{0}\"]", name);
      var val = node.SelectSingleNode(qry);
      if (val == null)
        return "";

      return val.InnerText;
    }


    public IpdaDocument[] GenerateDocuments(Ipda ipda)
    {
      List<IpdaDocument> docList = new List<IpdaDocument>();
      CultureInfo provider = CultureInfo.CreateSpecificCulture("it-IT");
      XmlDocument indiceXml = new XmlDocument();

      indiceXml.Load(this.xmlIndexFilepath);

      string[] soggettoFields = ipda.processo.soggetto.soggettoID.ToString().Split(new string[] { "C.F." }, StringSplitOptions.None);

      XmlNodeList nList = indiceXml.SelectNodes("//File");
      foreach (XmlNode node in nList)
      {
        IpdaDocument doc = new IpdaDocument();

        doc.iddocumento = GetNodeAttribute(node, ipda.xmlFieldId);
        doc.datachiusura = DateTime.Parse(ipda.processo.tempo.rifTemporale, provider);
        doc.oggettodocumento = doc.iddocumento;

        doc.soggettoproduttore = new SoggettoProduttore
        {
          nome = "",
          cognome = soggettoFields[0].Trim(),
          codicefiscale = soggettoFields[1].Trim()
        };

        doc.destinatario = new Destinatario
        {
          nome = "",
          cognome = GetNodeAttribute(node, "Denominazione"),
          codicefiscale = GetNodeAttribute(node, "PIVA")
        };

        docList.Add(doc);
      }

      return docList.ToArray();
    }

  }
}
