using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BiblosDS.Library.IStorage;

using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common;
using System.Configuration;
using System.Collections;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;
using System.Net;
using BiblosDS.Library.Common.Enums;

namespace BiblosDS.Library.Storage.SharePoint2010
{
    public class SharePointStorage2010 : StorageBase
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
                byte[] data = null;
                File fileUploaded = null;
                //Pick up the file in binary stream
                data = Document.Content.Blob;
                //
                ClientContext context = new ClientContext(Storage.MainPath);
                context.Credentials = new NetworkCredential(Storage.AuthenticationKey, Storage.AuthenticationPassword);
                Web web = context.Web;
                List docs = web.Lists.GetByTitle(Storage.Name);
                context.ExecuteQuery();
                //            
                Folder foolder = docs.RootFolder;
                if (!string.IsNullOrEmpty(StorageArea.Path))
                {
                    try
                    {
                        if ((foolder = docs.RootFolder.Folders.Where(x => x.Name == StorageArea.Path).FirstOrDefault()) == null)
                            docs.RootFolder.Folders.Add(StorageArea.Path);
                    }
                    catch (Exception)
                    {
                        docs.RootFolder.Folders.Add(StorageArea.Path);
                    }
                }
                context.ExecuteQuery();
                string fileName = GetIdDocuemnt(Document) + System.IO.Path.GetExtension(Document.Name);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = data;
                newFile.Url = fileName;
                try
                {
                    IEnumerable<File> resultFiles = context.LoadQuery(foolder.Files.Where(x => x.Name == fileName).Include(x => x.ListItemAllFields));
                    context.ExecuteQuery();
                    fileUploaded = resultFiles.FirstOrDefault();
                }
                catch { }
                if (fileUploaded != null)
                {
                    fileUploaded.CheckOut();
                    context.ExecuteQuery();
                    FileSaveBinaryInformation newFileInfo = new FileSaveBinaryInformation();
                    newFileInfo.Content = data;
                    fileUploaded.SaveBinary(newFileInfo);
                    fileUploaded.CheckIn("BiblosDS", CheckinType.MajorCheckIn);
                    context.ExecuteQuery();
                    ListItem listItems = fileUploaded.ListItemAllFields;                    
                    foreach (var item in Document.AttributeValues)
                    {                        
                        listItems[item.Attribute.Name] = item.Value;
                    }
                    fileUploaded.ListItemAllFields.Update();                    
                }
                else
                {
                    fileUploaded = foolder.Files.Add(newFile);                    
                    ListItem listItems = fileUploaded.ListItemAllFields;
                    foreach (var item in Document.AttributeValues)
                    {
                        listItems[item.Attribute.Name] = item.Value;
                    }
                    fileUploaded.ListItemAllFields.Update();
                }
                //Set the file version                               
                context.ExecuteQuery();
                context.Load(fileUploaded, w => w.MajorVersion);
                context.ExecuteQuery();
                Document.StorageVersion = fileUploaded.MajorVersion;
                return data.Length;    
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WCF_Storage, "SaveDocument", ex.ToString(), LoggingOperationType.BiblosDS_InsertDocument, LoggingLevel.BiblosDS_Errors);
                throw;
            }
           
        }

        protected override byte[] LoadDocument(Document Document)
        {
            ClientContext context = new ClientContext(Document.Storage.MainPath);
            
            context.Credentials = new NetworkCredential(Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
            Web web = context.Web;
            List docs = web.Lists.GetByTitle(Document.Storage.Name);
            
            context.ExecuteQuery();
            Folder foolder = docs.RootFolder;
            if (!string.IsNullOrEmpty(Document.StorageArea.Path))
            {
                    if ((foolder = docs.RootFolder.Folders.Where(x => x.Name == Document.StorageArea.Path).FirstOrDefault()) == null)
                        throw new BiblosDS.Library.Common.Exceptions.StorageAreaConfiguration_Exception("StorageArea not found:"+Document.StorageArea.Path);               
            }
            context.ExecuteQuery();
            string fileName = GetIdDocuemnt(Document) + System.IO.Path.GetExtension(Document.Name);
            IEnumerable<File> resultFiles = context.LoadQuery(foolder.Files.Where(x => x.Name == fileName));
            
            context.ExecuteQuery();
            File file = null;
            if (Document.StorageVersion.HasValue)
            {
                int version = (int)Document.StorageVersion.Value;
                file = resultFiles.Where(x => (Document.StorageVersion != null || x.MajorVersion == version)).FirstOrDefault();
            }else
                file = resultFiles.FirstOrDefault();
            if (file == null)
                throw new BiblosDS.Library.Common.Exceptions.FileNotFound_Exception();                  
            context.Load(file, w => w.ServerRelativeUrl);
            context.ExecuteQuery();
                FileInformation fileInformation =
                    File.OpenBinaryDirect(context, file.ServerRelativeUrl);
                byte[] buffer = new byte[32768];
                List<byte> result = new List<byte>();
                int bytesRead;
                do
                {
                    bytesRead = fileInformation.Stream.Read(buffer, 0, buffer.Length);
                    for (int i = 0; i < bytesRead; i++)
                    {
                        result.Add(buffer[i]);  
                    }                    
                } while (bytesRead != 0);
                return result.ToArray();
        }

        #region Private
        
        private string ClearSharepointReserverChar(string NomeFile)
        {
            //Caratteri non consentito da SharePoint
            // ~ " # % & * : < > ? / \ { | }. 
            return NomeFile.Replace("~", "_").Replace("#", "_").Replace("%", "_").Replace("&", "_").Replace("*", "_")
                .Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace("?", "_").Replace("/", "_").Replace("\\", "_")
                .Replace("{", "_").Replace("|", "_").Replace("}", "_").Replace("..",".");
        }       
   
        #endregion


        static private void CopyStream(System.IO.Stream source, System.IO.Stream destination)
        {
            byte[] buffer = new byte[32768];
            int bytesRead;
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);
        }

        protected override void RemoveDocument(Document Document)
        {
            ClientContext context = new ClientContext(Document.Storage.MainPath);

            context.Credentials = new NetworkCredential(Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
            Web web = context.Web;
            List docs = web.Lists.GetByTitle(Document.Storage.Name);

            context.ExecuteQuery();
            Folder foolder = docs.RootFolder;
            if (!string.IsNullOrEmpty(Document.StorageArea.Path))
            {
                if ((foolder = docs.RootFolder.Folders.Where(x => x.Name == Document.StorageArea.Path).FirstOrDefault()) == null)
                    throw new BiblosDS.Library.Common.Exceptions.StorageAreaConfiguration_Exception("StorageArea not found:" + Document.StorageArea.Path);
            }
            context.ExecuteQuery();
            string fileName = GetIdDocuemnt(Document) + System.IO.Path.GetExtension(Document.Name);
            IEnumerable<File> resultFiles = context.LoadQuery(foolder.Files.Where(x => x.Name == fileName));
            context.ExecuteQuery();
            File file = null;
            if (Document.StorageVersion.HasValue)
            {
                int version = (int)Document.StorageVersion.Value;
                file = resultFiles.Where(x => (Document.StorageVersion != null || x.MajorVersion == version)).FirstOrDefault();
            }
            else
                file = resultFiles.FirstOrDefault();
            if (file == null)
                throw new BiblosDS.Library.Common.Exceptions.FileNotFound_Exception();
            file.DeleteObject();
            context.ExecuteQuery();
        }

        #region SPFieldType

        /// <summary>
        /// Custom convert between type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private FieldType ParseSPFieldType(string type)
        {
            switch (type.ToLower())
            {
                case "system.boolean":
                    return FieldType.Boolean;
                case "system.int32":
                    return FieldType.Integer;
                default:
                    return FieldType.Text;
            }
        }

        #endregion

        protected override void SaveAttributes(Document Document)
        {
            
        }

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            BindingList<DocumentAttributeValue> attributeValues = new BindingList<DocumentAttributeValue>();
            ClientContext context = new ClientContext(Document.Storage.MainPath);

            context.Credentials = new NetworkCredential(Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
            Web web = context.Web;
            List docs = web.Lists.GetByTitle(Document.Storage.Name);

            context.ExecuteQuery();
            Folder foolder = docs.RootFolder;
            if (!string.IsNullOrEmpty(Document.StorageArea.Path))
            {
                if ((foolder = docs.RootFolder.Folders.Where(x => x.Name == Document.StorageArea.Path).FirstOrDefault()) == null)
                    throw new BiblosDS.Library.Common.Exceptions.StorageAreaConfiguration_Exception("StorageArea not found:" + Document.StorageArea.Path);
            }
            context.ExecuteQuery();
            string fileName = GetIdDocuemnt(Document) + System.IO.Path.GetExtension(Document.Name);
            IEnumerable<File> resultFiles = context.LoadQuery(foolder.Files.Where(x => x.Name == fileName).Include(x => x.ListItemAllFields));
            context.ExecuteQuery();
            File file = resultFiles.FirstOrDefault();
            if (file == null)
                throw new BiblosDS.Library.Common.Exceptions.FileNotFound_Exception();
            //if (Document.StorageVersion.HasValue && file.Versions.Count > 0)
            //    file = file.Versions.Where(x => x.VersionLabel == Document.StorageVersion.Value.ToString()).First();         
            BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);
            ListItem listItem = file.ListItemAllFields;
            foreach (var item in listItem.FieldValues.Keys)
            {
                try
                {
                    DocumentAttribute attribute = attributes.Where(x => x.Name == item).Single();
                    if (attribute != null)
                    {
                        DocumentAttributeValue attr = new DocumentAttributeValue();
                        attr.Attribute = attribute;
                        attr.Value = listItem.FieldValues[item];
                        attributeValues.Add(attr);
                    }
                }
                catch (Exception)
                {

                }
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
