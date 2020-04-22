using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using BiblosDS.Library.Common.Services;
using System.Security.Cryptography;
using BiblosDS.Library.Common.Exceptions;
using System.IO;
using BiblosDS.Library.Common.Enums;
using System.Configuration;
using BiblosDS.WCF.DigitalSign;

namespace BiblosDS.Library.IStorage
{
    /// <summary>
    /// Base class to imlement to put a document into new archive
    /// </summary>
    public abstract class StorageBase : IStorage
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StorageBase));  

        /// <summary>
        /// If storage support versionig use the versionig of the storage support
        /// </summary>
        protected virtual bool StorageSupportVersioning
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get the docuemnt Id
        /// If the document is Linked and the storage support the versioning 
        /// return the first Id of the docuemt and use the native storage versioning
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        /// <example>
        /// In the sharepoint storage the first document added is the id returned
        /// the native Check-In,Check-Out is use to versioning the file
        /// </example>
        protected Guid GetIdDocuemnt(Document Document)
        {       
            if (StorageSupportVersioning && Document.DocumentParentVersion != null)
            {
                //Verifica dell'id originario del documento
                return Document.DocumentParentVersion.IdDocument;
            }
            return Document.IdDocument;
        }

        #region Save
        /// <summary>
        /// Add a document to an archive        
        /// </summary>        
        /// <param name="document"></param>
        /// <returns>
        /// The size of the docuemnt Blob
        /// </returns>
        protected abstract long SaveDocument(string localFilePath, DocumentStorage storage, DocumentStorageArea storageArea, Document document, BindingList<DocumentAttributeValue> attributeValue);

        protected abstract long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content);

        protected abstract void SaveAttributes(Document Document);
                
        /// <summary>
        /// Metodo per il salvataggio dei documenti per la fulltext indexing
        /// </summary>
        /// <param name="document"></param>
        protected virtual void SaveSearchableDocument(Document document)
        {
            //TODO: Implementare salvataggio fulltext index base
        }

        protected virtual void SaveSearchableAttributes(Document document)
        {
            //TODO: Implementare salvataggio fulltext index base
        }
        #endregion

        #region Load

        /// <summary>
        /// Retrive document from an archive
        /// </summary>        
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract byte[] LoadDocument(Document Document);

        protected abstract byte[] LoadAttach(Document Document, string name);


        protected abstract BindingList<DocumentAttributeValue> LoadAttributes(Document Document);

        #endregion
        
        #region Remove

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        protected abstract void RemoveDocument(Document Document);

        protected virtual void RemoveSearchableDocuments(Document document)
        {
            //TODO: Implementare rimozione fulltext index base
        }
        #endregion       

        protected virtual string GetFileName(Document Document)
        {
            return string.Format("{0}{1}", Document.DocumentLink == null ? Document.IdDocument : Document.DocumentLink.IdDocument, Path.GetExtension(Document.Name));
        }

        protected virtual void CheckPrerequisite(Document Document)
        {
            if (Document.Storage == null)
                throw new BiblosDS.Library.Common.Exceptions.StorageNotFound_Exception();
            if (Document.Archive == null)
                throw new Exception("Documento non consistente: archivio non configurato.");
            if (Document.StorageArea == null)
                throw new Exception("Documento non consistente: storage area non configurata.");
        }


        protected virtual void CheckAttribute(BindingList<DocumentAttributeValue> DbAttributes, BindingList<DocumentAttributeValue> StorageAttributes)
        {
            if (ConfigurationManager.AppSettings["CheckAttribute"] != null && ConfigurationManager.AppSettings["CheckAttribute"].ToString().ToLower() == "false")
                return;
            foreach (var item in DbAttributes)
            {
                var attribute = StorageAttributes.Where(x => x.Attribute.IdAttribute == item.Attribute.IdAttribute).FirstOrDefault();
                if (attribute == null)
                    throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi modificati");
                else if (attribute.Value != null && item.Value != null)
                {
                    if (attribute.Attribute.AttributeType == "System.DateTime")
                    {
                        if (attribute.Value.TryConvert<DateTime>() != item.Value.TryConvert<DateTime>())
                            throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi modificati");
                    }
                    else if (!attribute.Value.ToString().Equals(item.Value.ToString()))
                        throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi modificati");
                }
                else
                {
                    if ((attribute.Value == null || string.IsNullOrWhiteSpace(attribute.Value.ToString())) && (item.Value == null || string.IsNullOrWhiteSpace(item.Value.ToString())))
                        continue;
                    if (attribute.Value != item.Value)
                            throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi modificati");
                }
            }
        }
                
        #region Sign

        internal void SignDocument(Document Document, byte[] DocumentContent)
        {
            
        }

        protected byte[] GetDocumentSignedContent(Document document, byte[] documentContent, out string fileName)
        {
            if(document.IsSigned())
            {
                using (CompEdLib comped = new CompEdLib())
                {
                    string metadata;
                    byte[] extractedContent = comped.GetContent(true, documentContent, out metadata);
                    fileName = document.Name;
                    while (true)
                    {
                        fileName = Path.GetFileNameWithoutExtension(fileName);
                        if (!fileName.IsSignedFile())
                        {
                            break;
                        }
                    }
                    return extractedContent;
                }                    
            }
            fileName = document.Name;
            return documentContent;
        }

        #endregion

        #region IStorage Members

        public void AddDocumentAttach(DocumentAttach attach)
        {
            string localPath = string.Empty;
            bool forceDelete = false;
            string localFilePath = string.Empty;
            CheckPrerequisite(attach);
            attach.IdDocument = attach.IdDocumentAttach;

            if (ConfigurationManager.AppSettings["PathCache"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PathCache"].ToString()))
            {
                localFilePath = CacheService.AddCache(attach, attach.Content, attach.Name, "");
            }
            if (string.IsNullOrEmpty(localFilePath))
            {
                forceDelete = true;
                localPath = Path.GetTempPath();
                localFilePath = Path.Combine(localPath, attach.IdDocumentAttach.ToString() + Path.GetExtension(attach.Name));
            }
            try
            {
                //Document.StorageArea = DocumentService.GetStorageArea(Document, AttributeValues);
                //

                File.WriteAllBytes(localFilePath, attach.Content.Blob);
                //    
                //Document.Content = new DocumentContent(GetByteArrayFromFile(localFilePath));
                //Save the document on the storageArea
                attach.Size = SaveDocument(localFilePath, attach.Storage, attach.StorageArea, attach, null);
                StorageService.UpdateStorageAreaSize(attach.StorageArea.IdStorageArea, (long)attach.Size);
                attach.Status = new Status((short)DocumentTarnsitoStatus.StorageProcessing);
                //Update the calculated value.
                //TODO Update del Attach
                //DocumentService.UpdateDocumentAttach(attach));
                if (attach.Storage.EnableFulText)
                {
                    SaveSearchableDocument(attach);
                }
            }
            finally
            {
                if (forceDelete)
                {
                    try
                    {
                        File.Delete(localFilePath);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }    
        }

        /// <summary>
        /// Insert the document into the archive
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="document"></param>
        /// <param name="attributeValues"></param>
        /// <returns>
        /// 
        /// </returns>
        public void AddDocument(Document Document, BindingList<DocumentAttributeValue> AttributeValues)
        {
            string localPath = string.Empty;
            bool forceDelete = false;
            string localFilePath = string.Empty;
            CheckPrerequisite(Document);

            if (ConfigurationManager.AppSettings["PathCache"] != null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PathCache"].ToString()))
            {
                localFilePath = CacheService.AddCache(Document, Document.Content, Document.Name, "");                
            }
            if (string.IsNullOrEmpty(localFilePath))
            {
                forceDelete = true;
                localPath = Path.GetTempPath();
                localFilePath = Path.Combine(localPath, Document.IdDocument.ToString() + Path.GetExtension(Document.Name)); 
            }
            try
            {
                //Document.StorageArea = DocumentService.GetStorageArea(Document, AttributeValues);
                //
                
                File.WriteAllBytes(localFilePath, Document.Content.Blob);
                //    
                //Document.Content = new DocumentContent(GetByteArrayFromFile(localFilePath));
                //Save the document on the storageArea
                Document.Size = SaveDocument(localFilePath, Document.Storage, Document.StorageArea, Document, AttributeValues);
                StorageService.UpdateStorageAreaSize(Document.StorageArea.IdStorageArea, (long)Document.Size);
                Document.Status = new Status((short)DocumentTarnsitoStatus.StorageProcessing);
                //Update the calculated value.
                DocumentService.UpdateDocument(Document);
                if(Document.Storage.EnableFulText)
                {
                    SaveSearchableDocument(Document);
                }
            }
            finally
            {
                if (forceDelete)
                {
                    try
                    {
                        File.Delete(localFilePath);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);                        
                    }
                }
            }                       
        }

        public void AddConformAttach(Document document, DocumentContent content, string name)
        {            
            WriteFile(name, document.Storage, document.StorageArea, document, content);
        }

        /// <summary>
        /// Retrive the document with the provider Proxy
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public byte[] GetDocument(Document Document)
        {
            CheckPrerequisite(Document);           
            byte[] res;
            if ((res = CacheService.GetFromChache(Document, Document.Name, "")) != null)
                return res;
            //
            return LoadDocument(Document);
        }

        /// <summary>
        /// Retrive the document with the provider Proxy
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public byte[] GetAttach(Document Document, string name)
        {
            CheckPrerequisite(Document);
            //
            return LoadAttach(Document, name);
        }

        #endregion

        private byte[] GetByteArrayFromFile(string FilName)
        {
            if (!File.Exists(FilName))
            {
                return null;
            }
            byte[] data = null;
            using (FileStream fs = File.OpenRead(FilName))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            return data;
        }

        #region Delete

        public virtual void DeleteDocumentWithoutAttributes(Document document)
        {
            throw new NotImplementedException();
        }

        public void DeleteDocument(Document Document)
        {
            RemoveDocument(Document);
            if(Document.Storage.EnableFulText)
            {
                RemoveSearchableDocuments(Document);
            }
        }

        public virtual void DeleteFullTextDocuments(Document document)
        {
            if(document.Storage.EnableFulText)
            {
                RemoveSearchableDocuments(document);
            }
        }
               
        #endregion

        #region Attributes


        public void RestoreAttribute(Document Document)
        {
            CheckPrerequisite(Document);
            if (Document.AttributeValues == null)
                throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi obbligatori per continuare.");            
            SaveAttributes(Document);
            if(Document.Storage.EnableFulText)
            {
                SaveSearchableAttributes(Document);
            }
        }                      


        public void WriteAttributes(Document Document)
        {
            try
            {
                CheckPrerequisite(Document);
            }
            catch (Exception)
            {
                Document = DocumentService.GetDocument(Document.IdDocument);
                if (Document == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentNotFound_Exception();
                CheckPrerequisite(Document);
            }
            if (Document.AttributeValues == null)
            {
                Document.AttributeValues = AttributeService.GetAttributeValues(Document.IdDocument);
                if (Document.AttributeValues == null)
                    throw new BiblosDS.Library.Common.Exceptions.Attribute_Exception("Attributi obbligatori per continuare.");
            }
            SaveAttributes(Document);
            if(Document.Storage.EnableFulText)
            {
                SaveSearchableAttributes(Document);
            }
        }

        #endregion

        #region IStorage Members

        /// <summary>
        /// Verifica che gli attributi salvati nello storage siano uguali a quelli memorizzati
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="DbAttributes"></param>
        /// <returns></returns>
        public bool VerifyAttribute(Document Document, BindingList<DocumentAttributeValue> DbAttributes)
        {
            try
            {
                var savedAttributes = LoadAttributes(Document);
                CheckAttribute(DbAttributes, savedAttributes);
                return true;
            }
            catch (Attribute_Exception)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void InitializeStorage(DocumentStorage storage)
        {
            if (storage.EnableFulText)
            {
                //TODO: applicare gestione fulltext index lato SQL Server
            }
        }

        public void WriteFullTextDocuments(Document document)
        {
            if(document.Storage.EnableFulText)
            {
                if (document.Content == null)
                    document.Content = new DocumentContent(GetDocument(document));

                SaveSearchableDocument(document);
                SaveSearchableAttributes(document);
            }
        }

        public virtual void CopyTo(Document document, string destinationPath)
        {
            byte[] documentContent = GetDocument(document);
            File.WriteAllBytes(destinationPath, documentContent);
        }
        #endregion                           
    }
}
