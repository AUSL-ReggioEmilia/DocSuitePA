using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Objects;
using CommonServices = BiblosDS.Library.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiblosDS.Library.IStorage;
using System.ComponentModel;

namespace VecompSoftware.BiblosDS.Service.Storage
{
    public class StorageService
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public StorageService()
        {

        }
        #endregion

        #region [ Methods ]
        public DocumentContent GetDocumentContent(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            Document toProcessDocument = CommonServices.DocumentService.GetById(document.IdDocument);
            switch ((DocumentStatus)toProcessDocument.Status.IdStatus)
            {
                case DocumentStatus.Undefined:
                case DocumentStatus.InTransito:
                    {
                        return GetDocumentContentFromTransito(toProcessDocument);
                    }                    
                case DocumentStatus.InStorage:
                    {
                        return GetDocumentContentFromStorage(toProcessDocument);
                    }                    
                case DocumentStatus.InCache:
                    {
                        return GetDocumentContentFromCache(toProcessDocument);
                    }                    
                case DocumentStatus.ProfileOnly:
                    {
                        if (toProcessDocument.DocumentLink != null)
                        {
                            return GetDocumentContent(toProcessDocument.DocumentLink);
                        }
                        return new DocumentContent(new byte[] { });
                    }                    
                case DocumentStatus.RemovedFromStorage:
                    {
                        return new DocumentContent(new byte[] { });
                    }                    
                default:
                    throw new Exception("Impossibile elaborare lo stato del documento.");
            }
        }

        public void CopyDocumentTo(Document document, string destinationPath)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new ArgumentNullException("Non è stato definito il parametro destinationPath");
            }

            Document toProcessDocument = CommonServices.DocumentService.GetById(document.IdDocument);
            switch ((DocumentStatus)toProcessDocument.Status.IdStatus)
            {
                case DocumentStatus.Undefined:
                case DocumentStatus.InTransito:
                    {
                        CopyDocumentFromTransitoTo(toProcessDocument, destinationPath);
                    }
                    break;
                case DocumentStatus.InStorage:
                    {
                        CopyDocumentFromStorageTo(toProcessDocument, destinationPath);
                    }
                    break;
                case DocumentStatus.InCache:
                    {
                        CopyDocumentFromCacheTo(toProcessDocument, destinationPath);
                    }
                    break;
                case DocumentStatus.ProfileOnly:
                    {
                        if (toProcessDocument.DocumentLink != null)
                        {
                            CopyDocumentTo(toProcessDocument.DocumentLink, destinationPath);
                        }
                    }
                    break;
                default:
                    throw new Exception("Impossibile elaborare lo stato del documento.");
            }
        }

        private void CopyDocumentFromTransitoTo(Document document, string destinationPath)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            string fullPath = GetDocumentPathFromTransito(document);
            File.Copy(fullPath, destinationPath, true);
        }

        private string GetDocumentPathFromTransito(Document document)
        {
            var transito = CommonServices.DocumentService.GetTransito(document.IdDocument);
            if (transito == null)
            {
                throw new FileNotFound_Exception($"Il documento {document.IdDocument} non è stato trovato nel transito");
            }

            return Path.Combine(transito.LocalPath, $"{document.IdDocument}{Path.GetExtension(document.Name)}");
        }

        private DocumentContent GetDocumentContentFromTransito(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            string fullPath = GetDocumentPathFromTransito(document);
            byte[] documentContent = File.ReadAllBytes(fullPath);
            return new DocumentContent(documentContent);
        }

        private IStorage InitializeStorageInstance(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            if (document.Storage == null)
            {
                throw new StorageNotFound_Exception($"Il documento {document.IdDocument} non è stato trovato nello storage");
            }

            DocumentStorageType storageType = document.Storage.StorageType;
            if (storageType == null)
            {
                storageType = CommonServices.StorageService.GetStorageTypeByStorage(document.Storage.IdStorage);
            }

            IStorage storageInstance = new StorageInstanceBuilder($"{storageType.StorageAssembly}.{storageType.StorageClassName}").BuildStorage();
            if (storageInstance == null)
            {
                throw new Generic_Exception($"Nessuna tipologia di storage trovata per il documento {document.IdDocument}");
            }
            return storageInstance;
        }

        private void CopyDocumentFromStorageTo(Document document, string destinationPath)
        {
            IStorage storageInstance = InitializeStorageInstance(document);
            BindingList<DocumentAttributeValue> attributeValues = CommonServices.AttributeService.GetAttributeValues(document.IdDocument);
            bool isAttributesVerified = storageInstance.VerifyAttribute(document, attributeValues);
            if (!isAttributesVerified)
            {
                throw new Attribute_Exception($"Attributi modificati per il documento {document.IdDocument}");
            }

            Document originalDocument = document.DocumentLink != null
                ? CommonServices.DocumentService.GetDocument(document.DocumentLink.IdDocument)
                : document;

            storageInstance.CopyTo(originalDocument, destinationPath);
        }

        private DocumentContent GetDocumentContentFromStorage(Document document)
        {
            IStorage storageInstance = InitializeStorageInstance(document);
            BindingList<DocumentAttributeValue> attributeValues = CommonServices.AttributeService.GetAttributeValues(document.IdDocument);
            bool isAttributesVerified = storageInstance.VerifyAttribute(document, attributeValues);
            if (!isAttributesVerified)
            {
                throw new Attribute_Exception($"Attributi modificati per il documento {document.IdDocument}");
            }

            Document originalDocument = document.DocumentLink != null 
                ? CommonServices.DocumentService.GetDocument(document.DocumentLink.IdDocument) 
                : document;

            byte[] documentContent = storageInstance.GetDocument(originalDocument);
            return new DocumentContent(documentContent);
        }

        private void CopyDocumentFromCacheTo(Document document, string destinationPath)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            string fullPath = Path.Combine(document.Archive.PathCache, $"{document.IdDocument}{Path.GetExtension(document.Name)}");
            File.Copy(fullPath, destinationPath);
        }

        private DocumentContent GetDocumentContentFromCache(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            string fullPath = Path.Combine(document.Archive.PathCache, $"{document.IdDocument}{Path.GetExtension(document.Name)}");
            byte[] documentContent = File.ReadAllBytes(fullPath);
            return new DocumentContent(documentContent);
        }

        public void DeleteDocument(Document document, bool includeAttributes = true)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Non è stato definito il parametro document");
            }

            DocumentStorageType storageType = document.Storage.StorageType;
            if (storageType == null)
            {
                storageType = CommonServices.StorageService.GetStorageTypeByStorage(document.Storage.IdStorage);
            }

            IStorage storageInstance = new StorageInstanceBuilder($"{storageType.StorageAssembly}.{storageType.StorageClassName}").BuildStorage();
            if (includeAttributes)
            {
                storageInstance.DeleteDocument(document);
                return;
            }
            storageInstance.DeleteDocumentWithoutAttributes(document);
        }
        #endregion
    }
}
