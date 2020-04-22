using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiImpronta : IXmlBasicInterface
    {
        public string Hash { get; set; }
        public string MarcaTemporale { get; set; }

        public static string GetNodoNullo()
        {
            return string.Format("<{0}></{0}>", "Impronta");
        }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            XmlElement nodoImp = doc.CreateElement("Impronta");
            nodoImp.InnerXml = Hash ?? string.Empty;

            XmlElement nodoMarca = doc.CreateElement("MarcaTemporale");
            nodoMarca.InnerXml = MarcaTemporale ?? string.Empty;

            return nodoImp.OuterXml + nodoMarca.OuterXml;
        }
    }
}