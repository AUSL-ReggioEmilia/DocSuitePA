using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiIntermediarioTrasmissione : XmlBasic, IXmlBasicInterface
    {
        private string _numIscrizioneAlboCaf;
        public string CodFisc { get; set; }

        public string NumIscrizioneAlboCaf
        {
            get { return _numIscrizioneAlboCaf ?? "00000"; }
            set
            {
                int valore;
                string toParse = value ?? string.Empty;
                _numIscrizioneAlboCaf = int.TryParse(toParse, out valore)
                                           ? valore.ToString().PadLeft(5, '0').Substring(0, 5)
                                           : "00000";
            }
        }

        public ImpegnoTrasmissione ImpegnoTrasmissione { get; set; }
        public DataImpegno DataImpegno { get; set; }

        public string GetSerializedForm()
        {
            /*
             Sezione 3.5. L’intermediario
                Come accennato la spedizione può essere effettuata dall’azienda tramite il servizio FiscoOnline o il servizio Entratel se rientra nella categoria delle autorizzate o esser affidata ad un intermediario autorizzato
                L’identificazione dell’intermediario avviene attraverso il codice fiscale, va anche indicata la data dell’Impegno a trasmettere e il tipo di Impegno .
                Il Codice CAF va indicato solo nel caso sia un CAF a trasmettere, ma vi è una particolarità nel controllo, se il campo non è valorizzato con 5 cifre viene generato errore. Per cui inserire forzatamente (è un campo numerico) 00000 in tutti i casi in cui a trasmettere non è un CAF.
                Per il Tipo di impegno si presume che la logica sia
                    1 Trasmettere comunicazione predisposta dal contribuente
                    2 Formare e trasmettere.
             */

            var doc = new XmlDocument();
            XmlElement nodoDati = doc.CreateElement(GetType().Name);

            XmlElement elemCodFisc = doc.CreateElement("CodFisc");
            elemCodFisc.InnerXml = (CodFisc ?? string.Empty).ToUpper();

            XmlElement elemCaf = doc.CreateElement("NumIscrizioneAlboCaf");
            elemCaf.InnerXml = NumIscrizioneAlboCaf ?? string.Empty;

            XmlElement elemImp = doc.CreateElement("ImpegnoTrasmissione");
            elemImp.InnerXml = ((int)ImpegnoTrasmissione).ToString();

            nodoDati.AppendChild(elemCodFisc);
            nodoDati.AppendChild(elemCaf);
            nodoDati.AppendChild(elemImp);

            nodoDati.InnerXml += (DataImpegno == null) ? CreaNodoNullo(typeof(DataImpegno)) : DataImpegno.GetSerializedForm();

            doc.AppendChild(nodoDati);

            return doc.OuterXml;
        }
    }
}