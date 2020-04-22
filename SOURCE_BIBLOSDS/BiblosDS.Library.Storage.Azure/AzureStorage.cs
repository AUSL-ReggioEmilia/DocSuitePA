using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.IStorage;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using BiblosDS.Library.Common.Services;
using System.Collections.Specialized;
using System.IO;

namespace BiblosDS.Library.Storage.Azure
{
    public class AzureStorage : StorageBase
    {
        BlobContainer container = null;

        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content)
        {
            if (string.IsNullOrEmpty(storage.AuthenticationKey))
                storage = StorageService.GetStorage(storage.IdStorage);
            StorageAccountInfo account = StorageAccountInfo.GetAccountInfoFromConfiguration(
                string.Empty,
                storage.MainPath,
                storage.AuthenticationKey,
                true);

            BlobStorage blobStorage = BlobStorage.Create(account);
            blobStorage.RetryPolicy = RetryPolicies.RetryN(2, TimeSpan.FromMilliseconds(100));
            
            //Check if exist storage area
            //If exist put the storage in the configured path
            if (!string.IsNullOrEmpty(storageArea.Path))
            {
                container = blobStorage.GetBlobContainer(storage.Name.ToLower() + storageArea.Path.ToLower());                
            }else
                container = blobStorage.GetBlobContainer(storage.Name.ToLower());
            //Create the container if it does not exist.
            if (!container.DoesContainerExist())
            {
                container.CreateContainer();
                //throw new Exception("Attenzione, container non esistente. Procedere alla creazione utilizzando il metodo SaveDocument");
            }

            BlobProperties blobProperty = new BlobProperties(saveFileName);
            NameValueCollection metadata = new NameValueCollection();                        
            container.CreateBlob(
                blobProperty,
                new BlobContents(content.Blob),
                true
                );

            return content.Blob.Length;
        }

        protected override long SaveDocument(string LocalFilePath, BiblosDS.Library.Common.Objects.DocumentStorage Storage, BiblosDS.Library.Common.Objects.DocumentStorageArea StorageArea, BiblosDS.Library.Common.Objects.Document Document, System.ComponentModel.BindingList<BiblosDS.Library.Common.Objects.DocumentAttributeValue> attributeValue)
        {
            if (string.IsNullOrEmpty(Document.Storage.AuthenticationKey))
                Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);
            StorageAccountInfo account = StorageAccountInfo.GetAccountInfoFromConfiguration(
                string.Empty,
                Storage.MainPath,
                Document.Storage.AuthenticationKey,
                true);

            BlobStorage blobStorage = BlobStorage.Create(account);
            blobStorage.RetryPolicy = RetryPolicies.RetryN(2, TimeSpan.FromMilliseconds(100));
            
            //Check if exist storage area
            //If exist put the storage in the configured path
            if (!string.IsNullOrEmpty(StorageArea.Path))
            {
                container = blobStorage.GetBlobContainer(Storage.Name.ToLower()+StorageArea.Path.ToLower());                
            }else
                container = blobStorage.GetBlobContainer(Storage.Name.ToLower());
            //Create the container if it does not exist.
            if (!container.DoesContainerExist())
            {
                BindingList<DocumentAttribute> attribute = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);

                NameValueCollection containerMetadata = new NameValueCollection();
                foreach (DocumentAttribute item in attribute)
                {
                    containerMetadata.Add(item.Name, item.Name);
                }               
                container.CreateContainer(containerMetadata, ContainerAccessControl.Private);
            }
            
            //ContainerAccessControl acl = container.GetContainerAccessControl();
            //
            BlobProperties blobProperty = new BlobProperties(GetFileName(Document));
            NameValueCollection metadata = new NameValueCollection();
            
            foreach (DocumentAttributeValue item in Document.AttributeValues)
            {
                metadata.Add(item.Attribute.Name, item.Value.ToString());
            }   
            blobProperty.Metadata = metadata;
            container.CreateBlob(
                blobProperty,
                new BlobContents(Document.Content.Blob),
                true
                );
            

            return Document.Content.Blob.Length;
        }

        protected override byte[] LoadDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            if (string.IsNullOrEmpty(Document.Storage.AuthenticationKey))
                Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);
            

            StorageAccountInfo account = StorageAccountInfo.GetAccountInfoFromConfiguration(
                string.Empty,
                Document.Storage.MainPath,
                Document.Storage.AuthenticationKey,
                true);

            BlobStorage blobStorage = BlobStorage.Create(account);
            blobStorage.RetryPolicy = RetryPolicies.RetryN(2, TimeSpan.FromMilliseconds(100));

            //Check if exist storage area
            //If exist put the storage in the configured path
            if (!string.IsNullOrEmpty(Document.StorageArea.Path))
            {
                container = blobStorage.GetBlobContainer(Document.Storage.Name.ToLower() + Document.StorageArea.Path.ToLower());
            }
            else
                container = blobStorage.GetBlobContainer(Document.Storage.Name.ToLower());

            BlobContents contents = new BlobContents(new MemoryStream());
            BlobProperties blob = container.GetBlob(GetFileName(Document), contents, false);
            return contents.AsBytes();            
        }

        protected override void SaveAttributes(Document Document)
        {
            if (container == null)
            {
                if (string.IsNullOrEmpty(Document.Storage.AuthenticationKey))
                    Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);

                StorageAccountInfo account = StorageAccountInfo.GetAccountInfoFromConfiguration(
                string.Empty,
                Document.Storage.MainPath,
                Document.Storage.AuthenticationKey,
                true);

                BlobStorage blobStorage = BlobStorage.Create(account);
                blobStorage.RetryPolicy = RetryPolicies.RetryN(2, TimeSpan.FromMilliseconds(100));

                //Check if exist storage area
                //If exist put the storage in the configured path
                if (!string.IsNullOrEmpty(Document.StorageArea.Path))
                {
                    container = blobStorage.GetBlobContainer(Document.Storage.Name.ToLower() + Document.StorageArea.Path.ToLower());
                }
                else
                    container = blobStorage.GetBlobContainer(Document.Storage.Name.ToLower());
            }
            BlobProperties blobProperty = container.GetBlobProperties(GetFileName(Document));
            foreach (DocumentAttributeValue item in Document.AttributeValues)
	        {
                try
                {
                    blobProperty.Metadata[item.Attribute.Name] = item.Value.ToString();
                }
                catch (Exception)
                {
                    blobProperty.Metadata.Add(item.Attribute.Name, item.Value.ToString());                    
                }
                
            }
            container.UpdateBlobMetadata(blobProperty);
        }

        protected override void RemoveDocument(BiblosDS.Library.Common.Objects.Document Document)
        {
            if (string.IsNullOrEmpty(Document.Storage.AuthenticationKey))
                Document.Storage = StorageService.GetStorage(Document.Storage.IdStorage);

            StorageAccountInfo account = StorageAccountInfo.GetAccountInfoFromConfiguration(
               string.Empty,
               Document.Storage.MainPath,
               Document.Storage.AuthenticationKey,
               true);

            BlobStorage blobStorage = BlobStorage.Create(account);
            blobStorage.RetryPolicy = RetryPolicies.RetryN(2, TimeSpan.FromMilliseconds(100));

            //Check if exist storage area
            //If exist put the storage in the configured path
            if (!string.IsNullOrEmpty(Document.StorageArea.Path))
            {
                container = blobStorage.GetBlobContainer(Document.Storage.Name.ToLower() + Document.StorageArea.Path.ToLower());
            }
            else
                container = blobStorage.GetBlobContainer(Document.Storage.Name.ToLower());

            container.DeleteBlob(GetFileName(Document));            
        }

        protected override System.ComponentModel.BindingList<BiblosDS.Library.Common.Objects.DocumentAttributeValue> LoadAttributes(BiblosDS.Library.Common.Objects.Document Document)
        {
            return new System.ComponentModel.BindingList<Common.Objects.DocumentAttributeValue>();
        }        

        protected override void CheckAttribute(System.ComponentModel.BindingList<Common.Objects.DocumentAttributeValue> DbAttributes, System.ComponentModel.BindingList<Common.Objects.DocumentAttributeValue> StorageAttributes)
        {

        }

        protected override byte[] LoadAttach(Document Document, string name)
        {
            throw new NotImplementedException();
        }
    }
}
