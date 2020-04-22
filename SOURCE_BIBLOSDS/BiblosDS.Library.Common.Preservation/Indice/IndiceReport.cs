using BiblosDS.Library.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BiblosDS.Library.Common.Preservation.Indice
{
    [XmlRoot(ElementName = "Indice", IsNullable = false)]
    public class IndiceReport
    {
        [XmlElement("File")]
        public IndiceFile[] File { get; set; }

        public void SaveTo(string destination, string xslFileName)
        {
            XmlFile<IndiceReport>.Serialize(this, destination, (w => {
                w.WriteProcessingInstruction("xml-stylesheet", String.Format("type=\"text/xsl\" href=\"{0}\"",
                  Path.GetFileName(xslFileName)));
            }));
        }
    }

    public class IndiceFile
    {
        [XmlElement("Attributo")]
        public IndiceFileAttributo[] Attributo { get; set; }

        [XmlAttribute]
        public long Progressivo { get; set; }
    }

    public class IndiceFileAttributo
    {
        [XmlAttribute]
        public string Nome { get; set; }

        public CData Value { get; set; }
    }
}
