using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public class AVCPHelper
    {
        public const string AvcpNamespace = "legge190_1_0";
        public const string AvcpNamespacePrefix = "legge190";


        public static string GetDocumentTitle(int AnnoAtto, string CodiceServizio, int NumeroAtto)
        {
            return string.Format("Avcp {0}", GetDocumentCode(
              AnnoAtto,
              CodiceServizio,
              NumeroAtto));
        }


        public static string GetDocumentCode(int AnnoAtto, string CodiceServizio, int NumeroAtto)
        {
            return string.Format("{0}{1}/{2:000000}",
              AnnoAtto,
              string.IsNullOrEmpty(CodiceServizio) ? "" : "/" + CodiceServizio,
              NumeroAtto);
        }


        public static string UpdateXml(string xml, UpdateData data)
        {
            pubblicazione pub = XmlUtil.Deserialize<AVCP.pubblicazione>(xml);

            pub.metadata.titolo = data.titolo;
            pub.metadata.@abstract = data.@abstract;
            pub.metadata.dataPubbicazioneDataset = data.dataPubblicazione;
            pub.metadata.entePubblicatore = data.entePubblicatore;
            pub.metadata.annoRiferimento = data.annoDocumento;
            pub.metadata.urlFile = data.urlFile;
            pub.metadata.licenza = data.licenza;

            return Serialize(pub);
        }

        public static string Serialize(AVCP.pubblicazione pub)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(AvcpNamespacePrefix, AvcpNamespace);

            return XmlUtil.Serialize<AVCP.pubblicazione>(pub, ns, AvcpNamespacePrefix);
        }

        public static string GetIndexFile(string urlFile, string titolo, string @abstract, string entePubblicatore, string licenza, IList<DocumentSeriesItem> list, string urlMask, int? year)
        {
            var dataPubblicazioneIndice = DateTime.MinValue;
            var dataUltimoAggiornamentoIndice = DateTime.MinValue;
            if (list.Count > 0)
            {
                dataPubblicazioneIndice = list.Select(p => p.PublishingDate).Min().GetValueOrDefault(DateTime.MinValue);
                dataUltimoAggiornamentoIndice = DateTime.MinValue;
                DateTime tmpLastUpdate = DateTime.MinValue;
                if (list.Any(p => p.LastChangedDate.HasValue))
                {
                    tmpLastUpdate = list
                        .Where(p => p.LastChangedDate.HasValue)
                        .Select(p => p.LastChangedDate.Value)
                        .Max().ToLocalTime().DateTime;
                }

                DateTime tmpLastPublished = DateTime.MinValue;
                tmpLastPublished = list
                    .Select(s => s.PublishingDate).Max()
                    .GetValueOrDefault(DateTime.MinValue);

                dataUltimoAggiornamentoIndice = tmpLastUpdate < tmpLastPublished ? tmpLastPublished : tmpLastUpdate;
            }

            var indexDoc = new AVCP.indici();
            if (list.Count > 0)
            {
                indexDoc.metadata = new indiciMetadata()
                {
                    titolo = titolo,
                    @abstract = @abstract,
                    dataPubblicazioneIndice = dataPubblicazioneIndice,
                    entePubblicatore = entePubblicatore,
                    dataUltimoAggiornamentoIndice = dataUltimoAggiornamentoIndice,
                    annoRiferimento = year ?? DateTime.Now.Year,
                    urlFile = urlFile,
                    licenza = licenza
                };
            }

            List<indiciDataset> dataset = new List<indiciDataset>();

            int index = 1;
            foreach (var item in list)
            {
                DateTime lastUpdateDate = item.LastChangedDate.HasValue ? item.LastChangedDate.Value.DateTime : item.RegistrationDate.DateTime;
                dataset.Add(new indiciDataset
                {
                    id = "ID_" + index++,
                    dataUltimoAggiornamento = lastUpdateDate,
                    linkDataset = string.Format(urlMask, item.Id)
                });
            }

            indexDoc.indice = dataset.ToArray();
            string xml = XmlUtil.Serialize<AVCP.indici>(indexDoc, null, "");
            return xml;
        }

    }
}
