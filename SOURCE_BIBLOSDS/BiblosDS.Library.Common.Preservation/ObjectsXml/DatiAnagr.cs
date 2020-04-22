using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiAnagr : IXmlBasicInterface
    {
        public string CodFisc { get; set; }
        public string Denominazione { get; set; }
        public DomFiscale SedeLegale { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            XmlElement nodoDati = doc.CreateElement(GetType().Name);

            XmlElement elementoCodFisc = doc.CreateElement("CodFisc");
            elementoCodFisc.InnerXml = (CodFisc ?? string.Empty).ToUpper();

            XmlElement elementoDenom = doc.CreateElement("Denominazione");
            elementoDenom.InnerXml = (Denominazione ?? string.Empty).ToUpper();

            nodoDati.AppendChild(elementoCodFisc);
            nodoDati.AppendChild(elementoDenom);

            nodoDati.InnerXml += SedeLegale.GetSerializedForm();

            doc.AppendChild(nodoDati);

            return doc.OuterXml;
        }
    }
}