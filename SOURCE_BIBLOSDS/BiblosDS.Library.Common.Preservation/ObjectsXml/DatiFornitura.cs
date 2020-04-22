using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiFornitura : IXmlBasicInterface
    {
        public string CodiceFornitura { get; set; }
        public int PeriodoImposta { get; set; }
        public TipoComunicazioneEnum TipoComunicazione { get; set; }
        public string Protocollo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSerializedForm()
        {
            /*
              Sezione 3.1: dati fornitura.
              Sono 4 campi che identificano :
               - Il codice della Fornitura: valore predefinito imp00
               - Il periodo di imposta:
               - Anno di imposta a cui si riferiscono i documenti di cui si trasmette l’impronta.
               - Si presume che in tutti quei casi in cui l’anno d’imposta non coincide con l’anno solare, vada indicato l’anno iniziale Es. dal 01/07/2008 al 30/06/2009 indicare l’anno 2008
               - Il tipo di comunicazione che può assumere 3 valori,
               - Ordinaria,
               - Sostitutiva Correttiva (corregge una comunicazione già inviata),
               - Sostitutiva Riversamento (sostituisce una comunicazione già inviata)
               - L’eventuale protocollo da sostituire
               - dato obbligatorio nel caso in cui si stia compilando una comunicazione sostitutiva: indicare il protocollo della comunicazione precedentemente inviata.
               - In caso di comunicazione sostitutiva correttiva, indicare il protocollo della comunicazione ordinaria da sostituire.
             */
            var doc = new XmlDocument();

            XmlElement nodoDatiFornitura = doc.CreateElement(GetType().Name);

            XmlElement elementoCodice = doc.CreateElement("CodiceFornitura");
            elementoCodice.InnerXml = (CodiceFornitura ?? string.Empty).ToUpper();

            XmlElement elementoPeriodo = doc.CreateElement("PeriodoImposta");
            elementoPeriodo.InnerXml = PeriodoImposta.ToString();

            XmlElement elementoTipoComunicazione = doc.CreateElement("TipoComunicazione");
            elementoTipoComunicazione.InnerXml = ((int)TipoComunicazione).ToString();

            if (TipoComunicazione == TipoComunicazioneEnum.Sostitutiva ||
                TipoComunicazione == TipoComunicazioneEnum.Correttiva)
            {
                XmlElement elementoProtocollo = doc.CreateElement("Protocollo");
                elementoProtocollo.InnerXml = (Protocollo ?? string.Empty).ToUpper();
                //
                elementoTipoComunicazione.AppendChild(elementoProtocollo);
            }

            nodoDatiFornitura.AppendChild(elementoCodice);
            nodoDatiFornitura.AppendChild(elementoPeriodo);
            nodoDatiFornitura.AppendChild(elementoTipoComunicazione);

            doc.AppendChild(nodoDatiFornitura);

            return doc.OuterXml;
        }

    }
}