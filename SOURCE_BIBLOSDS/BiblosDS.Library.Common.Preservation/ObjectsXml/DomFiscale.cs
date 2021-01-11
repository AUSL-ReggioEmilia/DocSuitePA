using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DomFiscale : IXmlBasicInterface
    {
        public string ComuneStato { get; set; }
        public string SiglaProvincia { get; set; }
        public Indirizzo Indirizzo { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            XmlElement nodoDomFiscale = doc.CreateElement(GetType().Name);

            XmlElement elementoStato = doc.CreateElement("ComuneStato");
            elementoStato.InnerXml = (ComuneStato ?? string.Empty).ToUpper();

            XmlElement elementoProv = doc.CreateElement("SiglaProvincia");
            elementoProv.InnerXml = (SiglaProvincia ?? string.Empty).ToUpper();

            nodoDomFiscale.AppendChild(elementoStato);
            nodoDomFiscale.AppendChild(elementoProv);

            nodoDomFiscale.InnerXml += Indirizzo.GetSerializedForm();

            doc.AppendChild(nodoDomFiscale);

            return doc.OuterXml;
        }
    }
}