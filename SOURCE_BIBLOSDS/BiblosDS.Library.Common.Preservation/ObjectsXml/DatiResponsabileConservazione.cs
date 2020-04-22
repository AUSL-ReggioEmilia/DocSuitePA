using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiResponsabileConservazione : IXmlBasicInterface
    {
        public DatiAnagr DatiAnagr { get; set; }

        public string GetSerializedForm()
        {
            /*
             Sezione 3.3: i dati del Responsabile
                Il responsabile è uno, tutti gli altri soggetti coinvolti nel processo possono essere inseriti nei delegati (fino a 20) la logica dei dati è la stessa del titolare.
                I responsabili “non residenti” prima dell’invio dell’impronta dovranno richiedere codice fiscale, nei dati Residenza/Sede dovrà essere indicato il domicilio in Italia.
             */
            var doc = new XmlDocument();
            XmlElement nodoResp = doc.CreateElement(GetType().Name);
            string anagr = string.Format("<{0}></{0}>", typeof(DatiAnagr).Name);

            if (DatiAnagr != null)
            {
                anagr = DatiAnagr.GetSerializedForm();
                anagr = anagr.Replace("<DomFiscale>", "<DomFiscaleSedeLegale>");
                anagr = anagr.Replace("</DomFiscale>", "</DomFiscaleSedeLegale>");
                anagr = anagr.Replace(string.Format("<{0}>", typeof(DatiAnagr).Name), null);
                anagr = anagr.Replace(string.Format("</{0}>", typeof(DatiAnagr).Name), null);
            }

            nodoResp.InnerXml += anagr;

            doc.AppendChild(nodoResp);

            return doc.OuterXml;
        }
    }
}