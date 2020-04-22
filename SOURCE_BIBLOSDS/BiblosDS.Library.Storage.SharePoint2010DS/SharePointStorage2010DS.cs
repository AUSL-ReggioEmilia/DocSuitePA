using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.IStorage;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using System.ServiceModel;
using BiblosDS.Library.Storage.SharePoint2010DS.ServiceReferenceDS;
using System.Net;
using System.IO;
using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common.Services;

namespace BiblosDS.Library.Storage.SharePoint2010DS
{
    public class SharePointStorage2010DS : StorageBase
    {
        protected override bool StorageSupportVersioning
        {
            get
            {
                return true;
            }
        }

        protected override long SaveDocument(string LocalFilePath, DocumentStorage Storage, DocumentStorageArea StorageArea, Document Document, BindingList<DocumentAttributeValue> attributeValue)
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

                BiblosSvcClient client = new BiblosSvcClient(binding, new EndpointAddress(Storage.MainPath));
                client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Storage.AuthenticationKey, Storage.AuthenticationPassword);
                client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
                string fileName = GetIdDocuemnt(Document) + Path.GetExtension(Document.Name);
                if (!client.ExistsDocumentSet(Storage.Name, StorageArea.Path, Document.DocumentParent.IdDocument.ToString()))
                {
                    var chainAttributes = new Dictionary<object, object>();
                    var chainContentType = attributeValue.Where(x => (x.Attribute.IsChainAttribute.HasValue && x.Attribute.IsChainAttribute.Value) && x.Attribute.Name.Equals("contentType", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    //attributeValue.Where(x => (!x.Attribute.IsChainAttribute.HasValue || x.Attribute.IsChainAttribute.Value) && !x.Attribute.Name.Equals("contentType", StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(x => chainAttributes.Add(x.Attribute.Name, x.Value));
                    attributeValue.Where(x => !x.Attribute.Name.Equals("contentType", StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(x => chainAttributes.Add(x.Attribute.Name, x.Value));
                    client.CreateDocumentSet(Storage.Name, StorageArea.Path, chainContentType != null ? chainContentType.Attribute.KeyFilter.ToString() : string.Empty, Document.DocumentParent.IdDocument.ToString(), chainAttributes);
                }
                var attributes = new Dictionary<string, object>();
                var contentType = attributeValue.Where(x => (!x.Attribute.IsChainAttribute.HasValue || !x.Attribute.IsChainAttribute.Value) && x.Attribute.Name.Equals("contentType", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                //attributeValue.Where(x => (!x.Attribute.IsChainAttribute.HasValue || !x.Attribute.IsChainAttribute.Value) && !x.Attribute.Name.Equals("contentType", StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(x => attributes.Add(x.Attribute.Name, x.Value));
                attributeValue.Where(x => !x.Attribute.Name.Equals("contentType", StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(x => attributes.Add(x.Attribute.Name, x.Value));
                return (long)client.AddToDocumentSet(Storage.Name, StorageArea.Path, Document.DocumentParent.IdDocument.ToString(), contentType != null ? contentType.Attribute.KeyFilter : string.Empty, fileName, Document.Content.Blob, attributes);
            }
            catch (Exception ex)
            {
                //Write the log
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_Sharepoint,
                    "SaveDocument",
                    ex.ToString(),
                     BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                     BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw new FileNotUploaded_Exception("File not uploaded" + Environment.NewLine + ex.ToString());
            }
        }

        protected override byte[] LoadDocument(Document Document)
        {
            try
            {
                 BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

                BiblosSvcClient client = new BiblosSvcClient(binding, new EndpointAddress(Document.Storage.MainPath));
                client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
                client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
                string fileName = GetIdDocuemnt(Document) + Path.GetExtension(Document.Name);
                return client.GetDocument(Document.Storage.Name, Document.StorageArea.Path, Document.DocumentParent.IdDocument.ToString(), fileName, Document.StorageVersion.HasValue ? (int)Document.StorageVersion.Value : 0);
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_Sharepoint,
                  "LoadDocument",
                  ex.ToString(),
                   BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                   BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw;
            }           
        }       

        protected override void RemoveDocument(Document Document)
        {
          
        }     

        protected override void SaveAttributes(Document Document)
        {
           
        }

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            BindingList<DocumentAttributeValue> attributeValues = new BindingList<DocumentAttributeValue>();
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

                BiblosSvcClient client = new BiblosSvcClient(binding, new EndpointAddress(Document.Storage.MainPath));
                client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
                client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
                 string fileName = GetIdDocuemnt(Document) + Path.GetExtension(Document.Name);
                var attributesSP = client.GetFileAttributes(Document.Storage.Name, Document.StorageArea.Path, Document.DocumentParent.IdDocument.ToString(), fileName, Document.StorageVersion.HasValue ? (int)Document.StorageVersion.Value : 0);

                BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);
                foreach (var item in attributes)
                {
                    try
                    {
                        if(attributesSP.ContainsKey(item.Name))
                        {
                            DocumentAttributeValue attr = new DocumentAttributeValue();
                            attr.Attribute = item;
                            attr.Value = attributesSP[item.Name];
                            attributeValues.Add(attr);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }               
            }
            catch (Exception ex)
            {
                //Write the log
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_Sharepoint,
                 "LoadAttributes",
                 ex.ToString(),
                  BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                  BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw;
            }
            return attributeValues;
        }     

        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content)
        {
            throw new NotImplementedException();
        }

        protected override byte[] LoadAttach(Document Document, string name)
        {
            throw new NotImplementedException();
        }
    }
}
