using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class Indirizzo : IXmlBasicInterface
    {
        public string FrazioneVia { get; set; }
        public string NumeroCivico { get; set; }
        public string Cap { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            XmlElement nodoInd = doc.CreateElement(GetType().Name);

            XmlElement elementoVia = doc.CreateElement("FrazioneVia");
            elementoVia.InnerXml = (FrazioneVia ?? string.Empty).ToUpper();

            XmlElement elementoCiv = doc.CreateElement("NumeroCivico");
            elementoCiv.InnerXml = (NumeroCivico ?? string.Empty).ToUpper();

            XmlElement elementoCap = doc.CreateElement("Cap");
            elementoCap.InnerXml = (Cap ?? string.Empty).ToUpper();

            nodoInd.AppendChild(elementoVia);
            nodoInd.AppendChild(elementoCiv);
            nodoInd.AppendChild(elementoCap);

            doc.AppendChild(nodoInd);

            return doc.OuterXml;
        }
    }
}