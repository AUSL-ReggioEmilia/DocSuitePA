using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.IStorage;
using System.IO;
using System.Xml;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.Library.Storage.BDSCom
{
    public class BDSComStorage : StorageBase
    {
        protected override string GetFileName(Common.Objects.Document Document)
        {
            return string.Format("{1}_{0}", Document.IdBiblos.ToString().PadLeft(8, '0'), Document.DocumentParent.IdBiblos.ToString().PadLeft(8, '0'));
        }

        protected override long WriteFile(string saveFileName, Common.Objects.DocumentStorage storage, Common.Objects.DocumentStorageArea storageArea, Common.Objects.Document document, Common.Objects.DocumentContent content)
        {
            string storagePath = Path.Combine(storage.MainPath, storageArea.Path);
            string saveFilePathName = Path.Combine(storagePath, saveFileName);
            File.WriteAllBytes(saveFilePathName, content.Blob);                        
            return content.Blob.Length;
        }      

        protected override long SaveDocument(string LocalFilePath, BiblosDS.Library.Common.Objects.DocumentStorage Storage, BiblosDS.Library.Common.Objects.DocumentStorageArea StorageArea, BiblosDS.Library.Common.Objects.Document Document, System.ComponentModel.BindingList<BiblosDS.Library.Common.Objects.DocumentAttributeValue> attributeValue)
        {
            string storage = Path.Combine(Storage.MainPath, StorageArea.Path);
            if (!Directory.Exists(storage))
                Directory.CreateDirectory(storage);
            string fileName = GetFileName(Document);          
            if (Path.GetExtension(Document.Name) != string.Empty)
                fileName = GetFileName(Document) + Path.GetExtension(Document.Name);          
            string saveFileName =  Path.Combine(storage, fileName);
            File.Copy(LocalFilePath, saveFileName, true);
            FileInfo fInfo = new FileInfo(saveFileName);           
            //Write attributes on file system
            WriteAttributes(Document);
            if (!fInfo.Exists)
                throw new Exception("Error on save document");

            return fInfo.Length;
        }

        protected override void SaveAttributes(BiblosDS.Library.Common.Objects.Document Document)
        {
            //Piccoli 20140902 Fix per problema file con estensione xml. 
            // se il documento originario è .xml, di fatto il salvataggio degli attributi sovrascrive il documento originario !!
            // per evitare la sovrascrittura comunque antepone il carattere "_" all'estensione del file
            
            //Write the attribute
            try
            {
                string storage = Path.Combine(Document.Storage.MainPath, Document.StorageArea.Path);

                string fill = "";
                if (Path.GetExtension(Document.Name) == "")
                    fill = "_"; 

                File.Delete(Path.Combine(storage, GetFileName(Document) + Path.GetExtension(Document.Name) + fill + ".xml"));
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(Document.AttributeValues.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlDocument doc = new XmlDocument();
                    x.Serialize(stream, Document.AttributeValues);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.DocumentElement.Attributes.RemoveAll();
                    doc.Save(Path.Combine(storage, Path.Combine(GetFileName(Document) + Path.GetExtension(Document.Name) + fill + ".xml")));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on SaveAttributes", ex) ;
            }
        }

        protected override byte[] LoadDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            string storage = Path.Combine(Document.Storage.MainPath, Document.StorageArea.Path);
            if (Path.GetExtension(Document.Name) != string.Empty)
            {
                var fileName = GetFileName(Document) + Path.GetExtension(Document.Name);
                if (File.Exists(Path.Combine(storage, fileName)))
                    return File.ReadAllBytes(Path.Combine(storage, fileName));
            }
            //Se la ricerca con estensione non va a buon fine si prova a cercare il nome del file
            var files = Directory.GetFiles(storage, GetFileName(Document) + ".*");
            if (files.Length <= 0)
                throw new Exception("File non found.");
            //FIX 
            Document.Name = Path.GetFileName(files.Where(x => Path.GetExtension(x) != ".CRC" && Path.GetExtension(x) != ".xml").First());
            return File.ReadAllBytes(files.Where(x => Path.GetExtension(x) != ".CRC" && Path.GetExtension(x) != ".xml").First());

        }

        protected override System.ComponentModel.BindingList<BiblosDS.Library.Common.Objects.DocumentAttributeValue> LoadAttributes(BiblosDS.Library.Common.Objects.Document Document)
        {
            return new System.ComponentModel.BindingList<Common.Objects.DocumentAttributeValue>();
        }

        protected override void RemoveDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            string storageDir = Path.Combine(Document.Storage.MainPath, Document.StorageArea.Path);
            string saveFileName = Path.Combine(storageDir, string.Concat(GetFileName(Document), Path.GetExtension(Document.Name)));
            string saveFileNameMetadata = Path.Combine(storageDir, string.Concat(GetFileName(Document), Path.GetExtension(Document.Name), ".xml"));
            File.Delete(saveFileName);
            File.Delete(saveFileNameMetadata);
        }

        public override void DeleteDocumentWithoutAttributes(Document document)
        {
            string storageDir = Path.Combine(document.Storage.MainPath, document.StorageArea.Path);
            string saveFileName = Path.Combine(storageDir, string.Concat(GetFileName(document), Path.GetExtension(document.Name)));
            File.Delete(saveFileName);
        }

        protected override void CheckAttribute(System.ComponentModel.BindingList<Common.Objects.DocumentAttributeValue> DbAttributes, System.ComponentModel.BindingList<Common.Objects.DocumentAttributeValue> StorageAttributes)
        {
            
        }

        protected override byte[] LoadAttach(Common.Objects.Document Document, string name)
        {
            string storage = Path.Combine(Document.Storage.MainPath, Document.StorageArea.Path);

            if (File.Exists(Path.Combine(storage, name)))
                return File.ReadAllBytes(Path.Combine(storage, name));
            else
                throw new FileNotFoundException();
        }

        public override void CopyTo(Document document, string destinationPath)
        {
            string storageDir = Path.Combine(document.Storage.MainPath, document.StorageArea.Path);
            string saveFileName = Path.Combine(storageDir, string.Concat(GetFileName(document), Path.GetExtension(document.Name)));
            File.Copy(saveFileName, destinationPath, true);
        }
    }
}
