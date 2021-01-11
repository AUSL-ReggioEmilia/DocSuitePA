using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class FileChiusura : XmlBasic, IXmlBasicInterface
    {
        public int NumEle { get; set; }
        public string Nome { get; set; }
        public DatiImpronta ImprontaMarca { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            var nodoFile = doc.CreateElement(this.GetType().Name);

            var elemNum = doc.CreateElement("NumEle");
            elemNum.InnerXml = NumEle.ToString();

            var elemNome = doc.CreateElement("Nome");
            elemNome.InnerXml = (Nome ?? string.Empty).ToUpper();

            nodoFile.AppendChild(elemNum);
            nodoFile.AppendChild(elemNome);
            nodoFile.InnerXml += ImprontaMarca == null ? DatiImpronta.GetNodoNullo() : ImprontaMarca.GetSerializedForm();

            doc.AppendChild(nodoFile);

            return doc.OuterXml;
        }
    }
}
