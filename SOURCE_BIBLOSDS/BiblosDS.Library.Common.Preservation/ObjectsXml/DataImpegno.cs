using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DataImpegno : IXmlBasicInterface
    {
        public int Giorno { get; set; }
        public int Mese { get; set; }
        public int Anno { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();
            XmlElement nodoData = doc.CreateElement(GetType().Name);

            XmlElement giorno = doc.CreateElement("Giorno");
            giorno.InnerXml = Giorno.ToString();

            XmlElement mese = doc.CreateElement("Mese");
            mese.InnerXml = Mese.ToString();

            XmlElement anno = doc.CreateElement("Anno");
            anno.InnerXml = Anno.ToString();

            nodoData.AppendChild(giorno);
            nodoData.AppendChild(mese);
            nodoData.AppendChild(anno);

            doc.AppendChild(nodoData);

            return doc.OuterXml;
        }
    }
}