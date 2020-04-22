using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

using BiblosDS.Library.Common.Objects;
using System.IO;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.DB;
using System.Security.Principal;
using BiblosDS.Library.Common.Objects.Response;
using System.Data.Common;
using BiblosDS.Library.Common.Objects.UtilityService;
using BiblosDS.Library.Common.Utility;
using VecompSoftware.ServiceContract.BiblosDS.Documents;

namespace BiblosDS.Library.Common.Services
{
    public partial class DocumentService
    {

        public static Document GetDocumentWithServerDetails(Guid idDocument)
        {
            return DbProvider.GetDocumentWithServerDetails(idDocument);
        }

        public static DocumentServer AddDocumentToMaster(Document document)
        {
            var server = ServerService.GetCurrentServer();
            if (server == null)
                throw new Exceptions.ServerNotDefined_Exception();            
            string pathTransito = ServerService.GetPathTransito(document.Archive, server.IdServer);
            DocumentStatus status = DocumentStatus.Undefined;
            if (document.Archive.TransitoEnabled)
            {
                if ((document.Content != null && document.Content.Blob != null && document.Content.Blob.Length > 0))
                {                    
                    FileService.SaveFileToTransitoLocalPath(document, pathTransito, document.Content.Blob);
                    status = DocumentStatus.InTransito;
                }
                else
                {
                    if (ConfigurationManager.AppSettings["AllowZeroByteDocument"].ToStringExt() != "true")
                        throw new Exception("Impossibile inserire un documento di zero byte.");
                    else
                        status = DocumentStatus.ProfileOnly;
                }
            }
            else
            {
                if ((document.Content != null && document.Content.Blob != null && document.Content.Blob.Length > 0))
                {
                    using (var clientChannel = WCFUtility.GetClientConfigChannel<IServiceDocumentStorage>(ServerService.WCF_DocumentStorage_HostName, server.ServerName))
                    {
                        (clientChannel as IServiceDocumentStorage).AddDocument(document);                        
                    }
                    status = DocumentStatus.InStorage;
                }
                else
                {
                    if (ConfigurationManager.AppSettings["AllowZeroByteDocument"].ToStringExt() != "true")
                        throw new Exception("Impossibile inserire un documento di zero byte.");
                    else
                        status = DocumentStatus.ProfileOnly;
                }
            }
            document.Storage = null;
            document.StorageArea = null;
            return DbProvider.SaveDocumentToMaster(document, server, status, pathTransito);
        }

        public static void SaveDocumentToMaster(Document document, Server server, DocumentStatus status)
        {            
            if (server == null)
                throw new Exceptions.ServerNotDefined_Exception();
            DbProvider.SaveDocumentToMaster(document, server, status);
        }

    }
}
