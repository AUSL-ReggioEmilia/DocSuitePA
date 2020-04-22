using BiblosDS.Library.IStorage;
using System;
using System.Linq;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using System.IO;
using BiblosDS.Library.Common.Services;
using System.Xml.Serialization;
using System.Xml;
using BiblosDS.Library.Common.Objects.Enums;
using System.Xml.Linq;

namespace BiblosDS.Library.Storage.SQL
{
    /// <summary>
    /// Classe per la gestione dei documenti in SQL Server tramite FileTable.
    /// Per ogni Storage creato verrano generate automaticamente 4 tabelle che definiscono:
    /// 1) FileTable contenente i documenti originali (ft_<nome storage>_documents)
    /// 2) FileTable contenente i documenti eventualmente estratti (es. file firmati) (ft_<nome storage>_searchable_documents)
    /// 3) FileTable contenente gli xml relativi agli attributi (ft_<nome storage>_attributes)
    /// 4) Tabella ponte contente la mappatura tra le 2 FileTable definite sopra con il relativo IdDocument (Document<nome storage>Maps)
    /// 
    /// Nelle FileTable ft_<nome storage>_searchable_documents e ft_<nome storage>_attributes verrà attivita la fulltextindex.
    /// Per ogni FileTable verrà definito uno specifico FILESTREAM FileGroup con un unico File associato.
    /// A livello DBA è poi possibile associare più File per partizionare il contenuto in più Directory, la procedura di aggiunta dei File nel FileGroup non può essere automatica.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/it-it/sql/relational-databases/blob/filetables-sql-server"/>
    public class SQL2014Storage : StorageBase
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public SQL2014Storage()
        {
        }
        #endregion

        #region [ Methods ]
        protected override string GetFileName(Document Document)
        {
            string ext = Path.GetExtension(Document.Name);
            return string.Concat(Document.IdDocument, ext);
        }

        protected override byte[] LoadAttach(Document Document, string name)
        {
            FileTableModel foundDocument = DocumentService.GetDocumentByNameFromSQLStorage(Document, name);
            if(foundDocument == null)
            {
                throw new Exception(string.Concat("LoadAttach -> document with name '", name, "' not found"));
            }
            return foundDocument.FileStream;
        }

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            try
            {
                BindingList<DocumentAttributeValue> saveAttributes = new BindingList<DocumentAttributeValue>();
                FileTableModel foundAttributeDocument = DocumentService.GetAttributesByDocumentFromSQLStorage(Document);
                if (foundAttributeDocument == null)
                {
                    throw new Exception(string.Concat("LoadAttributes -> attributes for documentid '", Document.IdDocument, "' not found"));
                }

                using (MemoryStream ms = new MemoryStream(foundAttributeDocument.FileStream))
                {
                    var attr = XElement.Load(ms).Elements("DocumentAttributeValue").Select(s => s);
                    DocumentAttributeValue attributeItem;
                    foreach (XElement element in attr)
                    {
                        attributeItem = new DocumentAttributeValue();
                        attributeItem.Value = element.Element("Value").Value.TryConvert(Type.GetType(element.Element("Attribute").Element("AttributeType").Value));
                        attributeItem.Attribute = new DocumentAttribute()
                        {
                            IdAttribute = new Guid(element.Element("Attribute").Element("IdAttribute").Value),
                            Name = element.Element("Attribute").Element("Name").Value
                        };
                        saveAttributes.Add(attributeItem);
                    }
                }
                return saveAttributes;
            }
            catch (Exception ex)
            {
                throw new Common.Exceptions.Attribute_Exception("LoadAttributes -> attributes modified." + ex.Message);
            }
        }

        protected override byte[] LoadDocument(Document Document)
        {
            string fileName = GetFileName(Document);
            FileTableModel foundDocument = DocumentService.GetDocumentFromSQLStorage(Document);
            if (foundDocument == null)
            {
                throw new Exception(string.Concat("LoadDocument -> document with id '", Document.IdDocument, "' not found"));
            }
            return foundDocument.FileStream;
        }

        protected override void RemoveDocument(Document Document)
        {
            DocumentService.RemoveDocumentFromSQLStorage(Document);
        }

        protected override void RemoveSearchableDocuments(Document document)
        {
            DocumentService.RemoveSearchableDocumentFromStorage(document);
        }

        protected override void SaveAttributes(Document Document)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(Document.AttributeValues.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                XmlDocument doc = new XmlDocument();
                xmlSerializer.Serialize(stream, Document.AttributeValues);
                stream.Position = 0;
                doc.Load(stream);
                doc.DocumentElement.Attributes.RemoveAll();
                //Save attribute document
                string attributesFileName = string.Concat(Document.IdDocument, Path.GetExtension(Document.Name), ".xml");
                DocumentService.AddDocumentToSQL(Document, attributesFileName, stream.ToArray(), DocumentType.Attributes);
            }

            using (MemoryStream searchableStream = new MemoryStream())
            {
                XmlDocument doc = new XmlDocument();
                searchableStream.Position = 0;

                XmlElement rootElement = doc.CreateElement("Attributes");

                foreach (DocumentAttributeValue item in Document.AttributeValues)
                {
                    XmlElement element = doc.CreateElement( item.Attribute.Name);
                    element.InnerText = item.Value.ToString();
                    rootElement.AppendChild(element);
                }
                doc.AppendChild(rootElement);
                doc.Save(searchableStream);

                //Save searchable attribute document
                string attributesFileName = string.Concat(Document.IdDocument, Path.GetExtension(Document.Name), ".xml");
                DocumentService.AddDocumentToSQL(Document, attributesFileName, searchableStream.ToArray(), DocumentType.SearchableAttributes);
            }            
        }

        protected override long SaveDocument(string localFilePath, DocumentStorage storage, DocumentStorageArea storageArea, Document document, BindingList<DocumentAttributeValue> attributeValue)
        {
            byte[] content = File.ReadAllBytes(localFilePath);
            string fileName = GetFileName(document);
            //Save document
            DocumentService.AddDocumentToSQL(document, fileName, content, DocumentType.Default);

            //Save searchable document
            string searchableFileName;
            byte[] extractedContent = base.GetDocumentSignedContent(document, document.Content.Blob, out searchableFileName);
            searchableFileName = string.Concat(document.IdDocument, Path.GetExtension(searchableFileName));
            DocumentService.AddDocumentToSQL(document, searchableFileName, extractedContent, DocumentType.Searchable);

            //Write attributes on file system
            WriteAttributes(document);

            return content.Length;
        }

        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content)
        {
            DocumentService.AddDocumentToSQL(document, saveFileName, content.Blob, DocumentType.Attachment);
            return content.Blob.Length;
        }

        /// <summary>
        /// Inizializza un nuovo storage di tipo SQL2014.
        /// L'inizializzazione dello storage comporta la creazione in DB di 3 FileTable più 1 tabella Ponte
        /// con la tabella Document per le relazioni.
        /// </summary>
        /// <param name="storage"></param>
        public override void InitializeStorage(DocumentStorage storage)
        {
            StorageService.InitializeSQLStorage(storage);
        }
        #endregion        
    }
}
