using System;
using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class Documento : XmlBasic, IXmlBasicInterface
    {
        public TipoDocumento TipoDocumento { get; set; }
        public int Numero { get; set; }
        public DataImpegno DataInizioVal { get; set; }
        public DataImpegno DataFineVal { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();

            var nodoDoc = doc.CreateElement(this.GetType().Name);
            var attributoTipo = doc.CreateAttribute("TipoDocumento");

            attributoTipo.Value = TipoDocumento.ToString();

            nodoDoc.Attributes.Append(attributoTipo);

            var elemNumero = doc.CreateElement("Numero");
            elemNumero.InnerXml = Numero.ToString();

            var dtIni = DataInizioVal == null ? CreaNodoNullo(typeof(DataImpegno)) : DataInizioVal.GetSerializedForm();
            dtIni = dtIni.Replace(string.Format("<{0}>", typeof(DataImpegno).Name), "<DataInizioVal>");
            dtIni = dtIni.Replace(string.Format("</{0}>", typeof(DataImpegno).Name), "</DataInizioVal>");

            var dtFin = DataFineVal == null ? CreaNodoNullo(typeof(DataImpegno)) : DataFineVal.GetSerializedForm();
            dtFin = dtFin.Replace(string.Format("<{0}>", typeof(DataImpegno).Name), "<DataFineVal>");
            dtFin = dtFin.Replace(string.Format("</{0}>", typeof(DataImpegno).Name), "</DataFineVal>");

            nodoDoc.AppendChild(elemNumero);
            nodoDoc.InnerXml += dtIni + dtFin;

            doc.AppendChild(nodoDoc);

            return doc.OuterXml;
        }
    }
}