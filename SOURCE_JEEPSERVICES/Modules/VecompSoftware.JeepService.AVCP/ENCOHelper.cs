using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using VecompSoftware.DocSuiteWeb.AVCP;

namespace VecompSoftware.ENCO
{
  public class ENCOHelper
  {
    public static string GetAVCPXml(ENCO.pubblicazione encoDoc)
    {
      XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
      ns.Add(AVCPHelper.AvcpNamespacePrefix, AVCPHelper.AvcpNamespace);

      string xml = XmlUtil.Serialize<ENCO.pubblicazione>(encoDoc, ns, AVCPHelper.AvcpNamespacePrefix);
      return xml;
    }


    public static ENCO.pubblicazione GetTestDocument()
    {
      var encoPub = new ENCO.pubblicazione();
      encoPub.metadata = new ENCO.pubblicazioneMetadata
      {
        annoRiferimento = 2013,
        dataPubbicazioneDataset = DateTime.Today,
        dataUltimoAggiornamentoDataset = DateTime.Today,
        titolo = "test",
        @abstract = "prova",
        entePubblicatore = "",
        urlFile = "www",
        licenza = "lic".ToString()
      };

      List<ENCO.ArrayOfPubblicazioneLottoLotto> listLotti = new List<ENCO.ArrayOfPubblicazioneLottoLotto>();
      listLotti.Add(new ENCO.ArrayOfPubblicazioneLottoLotto
      {
        cig = "",
        sceltaContraente = new ENCO.sceltaContraenteType()
      });
      encoPub.data = listLotti.ToArray();
      return encoPub;
    }
  }


  [System.Xml.Serialization.XmlRootAttribute(Namespace = AVCPHelper.AvcpNamespace, IsNullable = false)]
  public partial class pubblicazione
  {
  }
}
