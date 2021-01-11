using System;
using System.IO; 
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.IStorage;
using System.Security;
using System.Security.AccessControl;
using BiblosDS.Library.Common;
using System.Xml;
using System.Security.Principal;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Xml.Linq;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Services;

namespace BiblosDS.Library.Storage.FileSystem
{
    public class FileSystem : StorageBase
    {
        private string GetStorageDir(DocumentStorage Storage, DocumentStorageArea StorageArea)
        {
            string dir = Path.Combine(Storage.MainPath, Storage.Name);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!string.IsNullOrEmpty(StorageArea.Path))
            {
                dir = Path.Combine(dir, StorageArea.Path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            return dir;
        }

        private new string GetFileName(Document Document)
        {
            return string.Format("{0}{1}", Document.IdDocument, Path.GetExtension(Document.Name));
        }

        /// <summary>
        /// salvataggio del documento nella directory del definitivo
        /// </summary>
        /// <param name="LocalFilePath"></param>
        /// <param name="Storage"></param>
        /// <param name="StorageArea"></param>
        /// <param name="Document"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        /// <remarks>piuttosto di trovarsi in situazioni, che non dovrebbero succedere, di documenti 
        /// nel transito aventi lo stesso nome di documenti nel definitivo e l'impossibilità di sovrascriverli,
        /// viene permesso la sovrascrittura
        /// </remarks>
        protected override long SaveDocument(string LocalFilePath, DocumentStorage Storage, DocumentStorageArea StorageArea, Document Document, System.ComponentModel.BindingList<DocumentAttributeValue> attributeValue)
        {
            string saveFileName = Path.Combine(GetStorageDir(Document.Storage, Document.StorageArea), GetFileName(Document));
            File.Copy(LocalFilePath, saveFileName, true);
            FileInfo fInfo = new FileInfo(saveFileName);
            FileSecurity fSec = null;
            if (Document.Permissions != null)
            {
                fSec = fInfo.GetAccessControl();
                AuthorizationRuleCollection fileRoles = fSec.GetAccessRules(true, true, typeof(NTAccount));                
                foreach (AuthorizationRule item in fileRoles)
                {
                    fSec.RemoveAuditRuleAll(new FileSystemAuditRule(item.IdentityReference.ToString(), FileSystemRights.FullControl, AuditFlags.Success));
                }
                //
                foreach (var item in Document.Permissions)
                {
                    fSec.AddAccessRule(new FileSystemAccessRule(item.Name, FileSystemRights.Read, AccessControlType.Allow));
                } 
                fInfo.SetAccessControl(fSec);              
            }
            //Write attributes on file system
            WriteAttributes(Document);
            if (!fInfo.Exists)
                throw new Exception();
           
            return fInfo.Length;
        }

        protected override void SaveSearchableDocument(Document document)
        {
            //Save searchable document
            string searchableFileName;
            byte[] extractedContent = base.GetDocumentSignedContent(document, document.Content.Blob, out searchableFileName);
            searchableFileName = string.Concat(document.IdDocument, Path.GetExtension(searchableFileName));

            DocumentService.AddSearchableToFileTable(document, searchableFileName, extractedContent, DocumentType.Searchable);
        }

        protected override void SaveSearchableAttributes(Document document)
        {
            using (MemoryStream searchableStream = new MemoryStream())
            {
                XmlDocument doc = new XmlDocument();
                searchableStream.Position = 0;

                XmlElement rootElement = doc.CreateElement("Attributes");

                foreach (DocumentAttributeValue item in document.AttributeValues)
                {
                    XmlElement element = doc.CreateElement(item.Attribute.Name);
                    element.InnerText = item.Value.ToString();
                    rootElement.AppendChild(element);
                }
                doc.AppendChild(rootElement);
                doc.Save(searchableStream);

                //Save searchable attribute document
                string attributesFileName = string.Concat(document.IdDocument, Path.GetExtension(document.Name), ".xml");
                DocumentService.AddSearchableToFileTable(document, attributesFileName, searchableStream.ToArray(), DocumentType.SearchableAttributes);
            }
        }

        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content)
        {
            string saveFilePathName = Path.Combine(GetStorageDir(storage, storageArea), saveFileName);
            File.WriteAllBytes(saveFilePathName, content.Blob);
            return content.Blob.Length;
        }

        protected override byte[] LoadDocument(Document Document)
        {            
            string storage = GetStorageDir(Document.Storage, Document.StorageArea);
            string saveFileName = Path.Combine(storage, GetFileName(Document));                        
            return File.ReadAllBytes(saveFileName);
        }

        protected override void RemoveDocument(Document Document)
        {
            string saveFileName = Path.Combine(GetStorageDir(Document.Storage, Document.StorageArea), GetFileName(Document));
            string saveFileNameMetadata = Path.Combine(GetStorageDir(Document.Storage, Document.StorageArea), string.Concat(GetFileName(Document), ".xml"));
            File.Delete(saveFileName);
            File.Delete(saveFileNameMetadata);
        }

        public override void DeleteDocumentWithoutAttributes(Document document)
        {
            string saveFileName = Path.Combine(GetStorageDir(document.Storage, document.StorageArea), GetFileName(document));
            File.Delete(saveFileName);
        }

        protected override void RemoveSearchableDocuments(Document document)
        {
            DocumentService.RemoveSearchableDocumentFromStorage(document);
        }

        protected override void SaveAttributes(Document Document)        
        {
            //Write the attribute
            try
            {
                string storage = GetStorageDir(Document.Storage, Document.StorageArea);
                if (File.Exists(storage))
                    File.Delete(Path.Combine(storage, Document.IdDocument + Path.GetExtension(Document.Name) + ".xml"));
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(Document.AttributeValues.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlDocument doc = new XmlDocument();
                    x.Serialize(stream, Document.AttributeValues);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.DocumentElement.Attributes.RemoveAll();
                    doc.Save(Path.Combine(GetStorageDir(Document.Storage, Document.StorageArea), Document.IdDocument + Path.GetExtension(Document.Name) + ".xml"));
                }
            }
            catch (Exception ex)
            {               
                throw;
            }
        }      

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            string storage = GetStorageDir(Document.Storage, Document.StorageArea);
            try
            {
                BindingList<DocumentAttributeValue> saveAttributes = new BindingList<DocumentAttributeValue>();
                //Gianni: Use linq to Xml because with the object type of value th deserialize fail
                var attr = from c in XElement.Load(Path.Combine(storage, string.Format("{0}{2}.{1}", Document.IdDocument, "xml", Path.GetExtension(Document.Name)))).Elements("DocumentAttributeValue")
                            select c;
                DocumentAttributeValue attributeItem;
                foreach (var item in attr)
                {
                    attributeItem = new DocumentAttributeValue();
                    attributeItem.Value = item.Element("Value").Value.TryConvert(Type.GetType(item.Element("Attribute").Element("AttributeType").Value));
                    attributeItem.Attribute = new DocumentAttribute { 
                        IdAttribute = new Guid(item.Element("Attribute").Element("IdAttribute").Value) ,
                        Name = item.Element("Attribute").Element("Name").Value
                    };
                    saveAttributes.Add(attributeItem);
                }
                //using (StreamReader objStreamReader = new StreamReader(Path.Combine(storage, string.Format("{0}{2}.{1}", Document.IdDocument, "xml", Path.GetExtension(Document.Name)))))
                //{                    
                //    XmlSerializer x = new XmlSerializer(saveAttributes.GetType());                   
                //    saveAttributes = (BindingList<DocumentAttributeValue>)x.Deserialize(objStreamReader);
                //}
                return saveAttributes;
            }            
            catch (Exception ex)
            {
                throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi modificati." + ex.Message);
            }
        }

        protected override byte[] LoadAttach(Document Document, string name)
        {
            string storage = GetStorageDir(Document.Storage, Document.StorageArea);
            string saveFileName = Path.Combine(storage, name);
            return File.ReadAllBytes(saveFileName);
        }

        public override void CopyTo(Document document, string destinationPath)
        {
            string saveFileName = Path.Combine(GetStorageDir(document.Storage, document.StorageArea), GetFileName(document));
            File.Copy(saveFileName, destinationPath, true);
        }
    }
}
