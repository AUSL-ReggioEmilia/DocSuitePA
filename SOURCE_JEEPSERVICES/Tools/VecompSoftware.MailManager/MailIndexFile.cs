using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace VecompSoftware.MailManager
{
    [XmlRootAttribute(ElementName = "mail")]
    public class MailIndexFile
    {
        [NonSerialized]
        public string sourceFile;

        [XmlAttributeAttribute()]
        public string filename { get; set; }

        [XmlElementAttribute("file")]
        public MailAttachmentFile[] files { get; set; }

        public MailIndexFile()
        {
            sourceFile = String.Empty;
            filename = String.Empty;
            files = null;
        }


        public static MailIndexFile Load(string filename)
        {
            var res = XmlFile<MailIndexFile>.Load(filename, String.Empty);
            res.sourceFile = filename;
            return res;
        }


        public void Save()
        {
            XmlFile<MailIndexFile>.Serialize(this, this.sourceFile);
        }


        public MailAttachmentFile Find(int id)
        {
            return files.Where(p => p.id == id).SingleOrDefault();
        }


        public MailAttachmentFile Find(string filename)
        {
            return files.Where(p => Path.GetFileName(p.Filename).ToLower() == Path.GetFileName(filename).ToLower()).SingleOrDefault();
        }

    }

    public partial class MailAttachmentFile
    {

        [XmlAttributeAttribute()]
        public int idParent { get; set; }

        [XmlAttributeAttribute()]
        public int id { get; set; }

        [XmlAttributeAttribute()]
        public string sourceName { get; set; }

        [XmlAttributeAttribute()]
        public bool isBody;

        [XmlAttributeAttribute()]
        public string mimeType { get; set; }

        [XmlAttributeAttribute()]
        public string mimeSubtype { get; set; }

        [XmlAttributeAttribute()]
        public string idStored { get; set; }

        [XmlTextAttribute()]
        public string Filename { get; set; }

        public bool IsStored()
        {
            return !String.IsNullOrEmpty(idStored);
        }
    }
}
