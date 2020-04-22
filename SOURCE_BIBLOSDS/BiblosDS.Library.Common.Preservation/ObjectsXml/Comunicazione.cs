using System.Xml;
using System.Collections.Generic;
using System.Xml.Schema;

namespace BiblosDS.Library.Common.Preservation.ObjectsXml
{
    public class Comunicazione : XmlBasic, IXmlBasicInterface
    {
        public DatiFornitura Fornitura { get; set; }
        public DatiTitolareContabilita TitolareContabilita { get; set; }
        public DatiResponsabileConservazione ResponsabileConservazione { get; set; }
        public List<DatiDelegatoConservazione> DelegatiConservazione { get; set; }
        public DatiIntermediarioTrasmissione IntermediarioTrasmissione { get; set; }
        public DatiArchivioInformatico ArchivioInformatico { get; set; }

        public string GetSerializedForm()
        {
            var doc = new XmlDocument();
            var nodoComunicazione = doc.CreateElement(this.GetType().Name);
            var attrib = doc.CreateAttribute("xmlns:xsi");

            attrib.Value = @"http://www.w3.org/2001/XMLSchema-instance";
            nodoComunicazione.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("xmlns:xsd");
            attrib.Value = @"http://www.w3.org/2001/XMLSchema";
            nodoComunicazione.Attributes.Append(attrib);

            string datiFornitura = (Fornitura == null) ? CreaNodoNullo(typeof(DatiFornitura)) : Fornitura.GetSerializedForm();
            string titolare = datiFornitura + ((TitolareContabilita == null) ? CreaNodoNullo(typeof(DatiTitolareContabilita)) : TitolareContabilita.GetSerializedForm());
            string responsabile = titolare + ((ResponsabileConservazione == null) ? CreaNodoNullo(typeof(DatiResponsabileConservazione)) : ResponsabileConservazione.GetSerializedForm());
            string delegati = responsabile;

            if (DelegatiConservazione != null)
            {
                DelegatiConservazione.ForEach(x => delegati += x.GetSerializedForm());
            }

            string intermediario = delegati + ((IntermediarioTrasmissione == null) ? CreaNodoNullo(typeof(DatiIntermediarioTrasmissione)) : IntermediarioTrasmissione.GetSerializedForm());
            string archivio = intermediario + ((ArchivioInformatico == null) ? CreaNodoNullo(typeof(DatiArchivioInformatico)) : ArchivioInformatico.GetSerializedForm());

            nodoComunicazione.InnerXml = archivio;

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            doc.AppendChild(nodoComunicazione);

            return doc.OuterXml;
        }
    }
}