using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiTitolareContabilita : IXmlBasicInterface
    {
        public DatiAnagr DatiAnagr { get; set; }
        public bool FatturazioneElettronica { get; set; }

        public string GetSerializedForm()
        {
            /*
             Sezione 3.2: i dati del titolare della contabilità
                Sono i dati anagrafici del soggetto fiscale a cui si riferiscono i documenti, strutturati secondo la classica logica fiscale che distingue persone fisiche e giuridiche, indicando per l’una il domicilio fiscale e per l’altra la sede legale.
                Forse è importante precisare che occorre indicare il CODICE FISCALE (non la Partita Iva). L’indicazione di un codice fiscale non presente in Anagrafe Tributaria comporta lo scarto. Questo si ritiene che sia valido per tutti i soggetti riportati.
              
             *** INDIRIZZI ***
             Gli indirizzi sono soggetti a continui cambiamenti non annotabili nei supporti già generati. Questo crea problemi soprattutto per la spedizione delle impronte relative agli anni precedenti.
              
             *** FATTURAZIONE ELETTRONICA ***
             Questo flag è da attivare se il soggetto effettua invio/ricezione di fatture elettroniche non è da attivare se effettua soltanto la conservazione elettronica di fatture.
             */
            var doc = new XmlDocument();
            XmlElement nodoTitolare = doc.CreateElement(GetType().Name);
            string anagr = string.Format("<{0}></{0}>", typeof (DatiAnagr).Name);

            if (DatiAnagr != null)
            {
                anagr = DatiAnagr.GetSerializedForm();
                anagr = anagr.Replace("<DomFiscale>", "<DomFiscaleSedeLegale>");
                anagr = anagr.Replace("</DomFiscale>", "</DomFiscaleSedeLegale>");
            }

            XmlElement elementoFatt = doc.CreateElement("FatturazioneElettronica");
            elementoFatt.InnerXml = FatturazioneElettronica ? "Si" : "No";

            nodoTitolare.InnerXml += anagr;
            nodoTitolare.AppendChild(elementoFatt);

            doc.AppendChild(nodoTitolare);

            return doc.OuterXml;
        }
    }
}