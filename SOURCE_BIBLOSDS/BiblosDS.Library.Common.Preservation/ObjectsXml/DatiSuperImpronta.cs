using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiSuperImpronta : XmlBasic, IXmlBasicInterface
    {
        public int NumEle { get; set; }
        public DatiImpronta ImprontaMarca { get; set; }
        public List<FileChiusura> ListaFileChiusura { get; set; }

        public static string GetNodoNullo()
        {
            return string.Format("<{0}></{0}>", "SuperImpronta");
        }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            var nodoSuper = doc.CreateElement("SuperImpronta");

            var elemNum = doc.CreateElement("NumEle");
            elemNum.InnerXml = NumEle.ToString();

            nodoSuper.AppendChild(elemNum);

            nodoSuper.InnerXml += ImprontaMarca == null ? DatiImpronta.GetNodoNullo() : ImprontaMarca.GetSerializedForm();

            if (ListaFileChiusura == null || ListaFileChiusura.Count < 1)
            {
                nodoSuper.InnerXml += CreaNodoNullo(typeof(FileChiusura));
            }
            else
            {
                ListaFileChiusura.ForEach(x => nodoSuper.InnerXml += x.GetSerializedForm());
            }

            doc.AppendChild(nodoSuper);

            return doc.OuterXml;
        }
    }
}
