using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.IStorage;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.WebControls;
using BiblosDS.Library.Common.Objects;
using System.IO;
using BiblosDS.Library.Common.Services;
using System.ComponentModel;
using BiblosDS.Library.Common.Exceptions;
using BiblosDS.Library.Common;
using System.Configuration;
using System.Collections;
using BiblosDS.Library.Storage.SharePoint.lists;
using System.Net;

namespace BiblosDS.Library.Storage.SharePoint
{
    public class SharePointStorage : StorageBase
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
            SPSite site = null;
            SPWeb web = null;
            byte[] data = null;
            SPFile fileUploaded = null;
            string RootLibraryName = String.Empty;
            SPDocumentLibrary doclib = null;
            
            //Pick up the file in binary stream
            data = Document.Content.Blob;
            
            using (site = new SPSite(Storage.MainPath))
            {
                using (web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;

                    //SPFolder Folder = web.GetFolder(StorageArea.Path);
                    doclib = web.Lists[Storage.Name] as SPDocumentLibrary;
                    if (doclib == null)
                        web.Lists.Add(Storage.Name, string.Empty, SPListTemplateType.DocumentLibrary);
                  
                    /// **REMOVE**: 20090818 
                    /// viene impostato l'override, altrimenti il documento resterebbe nel transito
                    /// TODO : da sistemare con la gestione delle versioni in sharepoint
                    try
                    {
                        SPFolder foolder = null;
                        if (data != null)
                        {
                            if (!string.IsNullOrEmpty(StorageArea.Path))
                            {
                                try
                                {
                                    if (doclib.RootFolder.SubFolders[StorageArea.Path] == null)
                                        doclib.RootFolder.SubFolders.Add(StorageArea.Path);
                                }
                                catch (Exception)
                                {
                                    doclib.RootFolder.SubFolders.Add(StorageArea.Path);

                                }
                                foolder = doclib.RootFolder.SubFolders[StorageArea.Path];
                            }
                            else
                                foolder = doclib.RootFolder;

                            string fileName = GetIdDocuemnt(Document) + Path.GetExtension(Document.Name);
                            try
                            {
                                fileUploaded = foolder.Files[fileName];
                            }
                            catch { }
                            if (fileUploaded != null)
                            {
                                fileUploaded.CheckOut();
                                fileUploaded.SaveBinary(data);
                                fileUploaded.CheckIn("BiblosDS", SPCheckinType.MajorCheckIn);                                                                
                            }else
                                fileUploaded = foolder.Files.Add(fileName, data, true);
                            //Set the file version
                            Document.StorageVersion = fileUploaded.MajorVersion;


                            if (ConfigurationManager.AppSettings["ForceSharePointSecurity"] != null && ConfigurationManager.AppSettings["ForceSharePointSecurity"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                            {
                                fileUploaded.Item.BreakRoleInheritance(false);
                                try
                                {
                                    for (int i = 0; i < fileUploaded.Item.RoleAssignments.Count; i++)
                                    {
                                        try
                                        {
                                            fileUploaded.Item.RoleAssignments.Remove((SPPrincipal)fileUploaded.Item.RoleAssignments[i].Member);
                                        }
                                        catch (Exception)
                                        {

                                        }
                                        //                                        
                                    }
                                    string SiteGroupsName = ConfigurationManager.AppSettings["SiteGroupsName"] == null ? string.Empty : ConfigurationManager.AppSettings["SiteGroupsName"].ToString();
                                    //foreach (var item in Document.Permissions)
                                    //{
                                    SPRoleDefinitionCollection webroledefinition = web.RoleDefinitions;

                                    SPGroup group = null;
                                    try
                                    {
                                        group = web.SiteGroups[SiteGroupsName];
                                    }
                                    catch (Exception)
                                    {
                                        web.SiteGroups.Add(SiteGroupsName, web.AssociatedOwnerGroup, null, "");
                                        group = web.SiteGroups[SiteGroupsName];
                                    }

                                    //Add user to the group of viewer
                                    //try
                                    //{
                                    //    group.AddUser()
                                    //}
                                    //catch (Exception)
                                    //{

                                    //    throw;
                                    //}
                                    SPRoleAssignment assignment = new SPRoleAssignment(group);
                                    assignment.RoleDefinitionBindings.Add(webroledefinition.GetByType(SPRoleType.Reader));
                                    fileUploaded.Item.RoleAssignments.Add(assignment);
                                    //}                                                   
                                }
                                catch (Exception)
                                {

                                }
                                finally
                                {
                                    fileUploaded.Item.BreakRoleInheritance(true);
                                }
                            }

                            //In questo caso forse conviene salvare gli attributi al momento dell'upload del file.
                            //SPListItem MyListItem = fileUploaded.Item;
                            foreach (var item in Document.AttributeValues)
                            {
                                try
                                {
                                    fileUploaded.Item[item.Attribute.Name] = item.Value;
                                }
                                catch (Exception)
                                {                                                        
                                    doclib.Fields.Add(item.Attribute.Name, ParseSPFieldType(item.Attribute.AttributeType), item.Attribute.IsRequired);
                                    doclib.Update();                                    
                                }                                
                            }
                            fileUploaded.Item.SystemUpdate();
                        }
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
                    web.AllowUnsafeUpdates = false;
                }
            }
            return data.Length;    
        }

        protected override byte[] LoadDocument(Document Document)
        {
            SPSite site = null;
            SPWeb web = null;
            SPDocumentLibrary doclib = null;
            SPFile file = null;            
            using (site = new SPSite(Document.Storage.MainPath))
            {                
                using (web = site.OpenWeb())
                {
                    //SPFolder Folder = web.GetFolder(Document.Storage.Name);
                    doclib = web.Lists[Document.Storage.Name] as SPDocumentLibrary;
                    if (!string.IsNullOrEmpty(Document.StorageArea.Path))
                        file = doclib.RootFolder.SubFolders[Document.StorageArea.Path].Files[GetIdDocuemnt(Document) + Path.GetExtension(Document.Name)];
                    else
                        file = doclib.RootFolder.Files[GetIdDocuemnt(Document) + Path.GetExtension(Document.Name)];
                    if (Document.StorageVersion.HasValue && file.Versions.Count >0)
                        file = file.Versions.OfType<SPFileVersion>().Where(x => x.File.MajorVersion == (int)Document.StorageVersion.Value).First().File;
                    return file.OpenBinary();
                }
            }
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



        protected override void RemoveDocument(Document Document)
        {
            SPSite site = null;
            SPWeb web = null;            
            SPDocumentLibrary doclib = null;            
            using (site = new SPSite(Document.Storage.MainPath))
            {
                using (web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;

                    //SPFolder Folder = web.GetFolder(StorageArea.Path);
                    doclib = web.Lists[Document.Storage.Name] as SPDocumentLibrary;
                    try
                    {

                        if (!string.IsNullOrEmpty(Document.StorageArea.Path))
                            doclib.RootFolder.SubFolders[Document.StorageArea.Path].Files.Delete(GetIdDocuemnt(Document) + Path.GetExtension(Document.Name));
                        else
                            doclib.RootFolder.Files.Delete(GetIdDocuemnt(Document) + Path.GetExtension(Document.Name));
                    }
                    catch (Exception ex)
                    {
                        //Write the log
                        throw new FileNotUploaded_Exception("File not uploaded" + Environment.NewLine + ex.ToString());
                    }
                    web.AllowUnsafeUpdates = false;
                }
            }
        }

        #region SPFieldType

        /// <summary>
        /// Custom convert between type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private SPFieldType ParseSPFieldType(string type)
        {
            switch (type.ToLower())
            {
                case "system.boolean":
                    return SPFieldType.Boolean;
                case "system.int32":
                    return SPFieldType.Integer;
                default:
                    return SPFieldType.Text;
            }
        }

        #endregion

        protected override void SaveAttributes(Document Document)
        {
            SPSite site = null;
            SPWeb web = null;
            SPFile file;          
            SPDocumentLibrary doclib = null;            
            using (site = new SPSite(Document.Storage.MainPath))
            {
                using (web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;                    
                    doclib = web.Lists[Document.Storage.Name] as SPDocumentLibrary;
                    try
                    {
                        if (!string.IsNullOrEmpty(Document.StorageArea.Path))
                            file = doclib.RootFolder.SubFolders[Document.StorageArea.Path].Files[GetIdDocuemnt(Document) + Path.GetExtension(Document.Name)];
                        else
                            file = doclib.RootFolder.Files[GetIdDocuemnt(Document) + Path.GetExtension(Document.Name)];
                        foreach (DocumentAttributeValue item in Document.AttributeValues)
                        {
                            file.Item[item.Attribute.Name] = item.Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Write the log
                        throw new FileNotUploaded_Exception("File not uploaded" + Environment.NewLine + ex.ToString());
                    }
                    web.AllowUnsafeUpdates = false;
                }
            }
        }

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            BindingList<DocumentAttributeValue> attributeValues = new BindingList<DocumentAttributeValue>();
            try
            {
                SPSite site = null;
                SPWeb web = null;
                SPDocumentLibrary doclib = null;
                SPFile file = null;
                using (site = new SPSite(Document.Storage.MainPath))
                {
                    using (web = site.OpenWeb())
                    {
                        //SPFolder Folder = web.GetFolder(Document.Storage.Name);
                        doclib = web.Lists[Document.Storage.Name] as SPDocumentLibrary;
                        if (!string.IsNullOrEmpty(Document.StorageArea.Path))
                            file = doclib.RootFolder.SubFolders[Document.StorageArea.Path].Files[GetIdDocuemnt(Document) + Path.GetExtension(Document.Name)];
                        else
                            file = doclib.RootFolder.Files[GetIdDocuemnt(Document) + Path.GetExtension(Document.Name)];
                        BindingList<DocumentAttribute> attributes = AttributeService.GetAttributesFromArchive(Document.Archive.IdArchive);
                        foreach (var item in file.Properties.Keys)
                        {
                            try
                            {
                                DocumentAttribute attribute = attributes.Where(x => x.Name == item.ToString()).Single();
                                if (attribute != null)
                                {
                                    DocumentAttributeValue attr = new DocumentAttributeValue();
                                    attr.Attribute = attribute;
                                    attr.Value = file.Properties[item].ToString();
                                    attributeValues.Add(attr);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }                        
                    }
                }               
            }
            catch (Exception ex)
            {
                //Write the log
                throw new FileNotFound_Exception("File non trovato" + Environment.NewLine + ex.ToString());
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
