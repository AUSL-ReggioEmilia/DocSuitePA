using System.Collections.Generic;
using System;
using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class IndiceDocumentiInArchivio : XmlBasic, IXmlBasicInterface
    {
        public int NumElemIndice { get; set; }
        public List<Documento> Documenti { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            var nodoIndice = doc.CreateElement(this.GetType().Name);

            var elemNum = doc.CreateElement("NumElemIndice");
            elemNum.InnerXml = NumElemIndice.ToString();

            nodoIndice.AppendChild(elemNum);

            var documenti = (Documenti == null || NumElemIndice < 1) ? CreaNodoNullo(typeof(Documento)) : string.Empty;

            if (Documenti != null)
            {
                Documenti.ForEach(x => documenti += x.GetSerializedForm());
            }

            nodoIndice.InnerXml += documenti;

            doc.AppendChild(nodoIndice);

            return doc.OuterXml;
        }
    }
}