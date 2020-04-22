using System.Xml;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class DatiArchivioInformatico : XmlBasic, IXmlBasicInterface
    {
        public LuogoConservazione LuogoConservazione { get; set; }
        public DatiImpronta Impronta { get; set; }
        public IndiceDocumentiInArchivio IndiceDocumentiInArchivio { get; set; }

        public string GetSerializedForm()
        {
            /*
             Sezione 3.6: i dati dell’archivio
             */
            var doc = new XmlDocument();

            var nodoDati = doc.CreateElement(GetType().Name);

            var luogo = LuogoConservazione == null ? CreaNodoNullo(typeof (LuogoConservazione)) : LuogoConservazione.GetSerializedForm();
            var imp = Impronta == null ? CreaNodoNullo(typeof(DatiImpronta)) : Impronta.GetSerializedForm();
            var indice = IndiceDocumentiInArchivio == null ? CreaNodoNullo(typeof(IndiceDocumentiInArchivio)) : IndiceDocumentiInArchivio.GetSerializedForm();

            nodoDati.InnerXml += luogo + imp + indice;

            doc.AppendChild(nodoDati);

            return doc.OuterXml;
        }
    }
}