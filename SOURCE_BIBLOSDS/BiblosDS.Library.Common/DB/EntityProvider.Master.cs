using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using Model = BiblosDS.Library.Common.Model;
using System.ComponentModel;
using System.Configuration;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Enums;
using System.Data.Objects.DataClasses;
using DataObjects = System.Data.Objects;
using System.Data;
using System.Transactions;
using System.Linq.Expressions;
using BiblosDS.Library.Common.Objects.Response;
using System.Data.Objects;
using System.Data.Common;
using System.Data.SqlClient;
using log4net;

namespace BiblosDS.Library.Common.DB
{
    public partial class EntityProvider
    {
        public Document GetDocumentWithServerDetails(Guid idDocument)
        {
            try
            {
                var query = db.Document.Include(x => x.DocumentServer).Include(x => x.DocumentServer.First().Server).SingleOrDefault(x => x.IdDocument == idDocument);
                Document result = null;
                if (query != null)
                {
                    result = query.Convert();
                    result.DocumentInServer = new List<DocumentServer>();
                    foreach (var item in query.DocumentServer)
                    {
                        result.DocumentInServer.Add(item.Convert(0, 5, typeof(Document)));
                    }
                }
                return result;
            }
            finally
            {
                Dispose();
            }            
        }
        
        public Server GetServer(string serverName)
        {            

            try
            {
                var query = db.Server.Where(x => x.ServerName == serverName).FirstOrDefault();

                if (query != null)
                    return query.Convert();

                return null;

            }
            finally
            {
                Dispose();
            }            
        }

        public ArchiveServerConfig GetArchiveServerConfig(Guid idArchive)
        {
            try
            {
                Model.ArchiveServerConfig query = db.ArchiveServerConfig
                    .Include(i => i.Server)
                    .Where(x => x.IdArchive == idArchive).FirstOrDefault();
                if (query != null)
                    return query.Convert();
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        public ArchiveServerConfig GetArchiveServerConfig(Guid idServer, Guid idArchive)
        {
            try
            {
                var query = db.ArchiveServerConfig.Where(x => x.IdServer == idServer && x.IdArchive == idArchive).FirstOrDefault();

                if (query != null)
                    return query.Convert();

                return null;
            }
            finally
            {
                Dispose();
            }
        }

        internal DocumentServer SaveDocumentToMaster(Document document, Server server, DocumentStatus status, string transitoLocalPath = null)
        {
            try
            {
                var entityDocument = db.Document.Single(x => x.IdDocument == document.IdDocument);
                var entity = db.DocumentServer.SingleOrDefault(x => x.IdDocument == document.IdDocument && x.IdServer == server.IdServer);
                if (entity == null)
                {
                    entity = new Model.DocumentServer();
                    this.db.DocumentServer.AddObject(entity);
                }
                entity.Document = entityDocument;
                entity.IdServer = server.IdServer;
                if (document.Storage != null)
                    entity.IdStorage = document.Storage.IdStorage;
                if (document.StorageArea != null)
                    entity.IdStorageArea = document.StorageArea.IdStorageArea;
                entity.DateCreated = DateTime.Now;
                entity.IdDocumentStatus = (short)status;

                if (transitoLocalPath != null && document.Content != null && status == DocumentStatus.InTransito)
                {
                    var transit = new Model.Transit
                    {
                        IdTransit = Guid.NewGuid(),
                        LocalPath = transitoLocalPath,
                        Status = (int)DocumentTarnsitoStatus.DaProcessare,
                        IdServer = server.IdServer,
                        ServerName = Environment.MachineName,
                        DateCreated = DateTime.Now
                    };
                    entityDocument.Transit.Add(transit);
                }

                if (requireSave)
                    this.db.SaveChanges();
                return entity.Convert();
            }
            finally
            {
                Dispose();
            }            
        }

        internal List<DocumentServer> GetDocumentInServer(Guid idDocument)
        {
            try
            {
                List<DocumentServer> result = new List<DocumentServer>();
                var query = db.DocumentServer.Include(x => x.Server).Where(x => x.IdDocument == idDocument);

                foreach (var item in query)
                {
                    result.Add(item.Convert());
                }

                return result;

            }
            finally
            {
                Dispose();
            }            
        }

        internal DocumentServer GetDocumentInServer(Guid idServer, Guid idDocument)
        {
            try
            {                
                var query = db.DocumentServer.Include(x => x.Server).Include(x => x.Storage).Include(x => x.StorageArea).Include(x => x.DocumentStatus).Where(x => x.IdDocument == idDocument && x.IdServer == idServer).SingleOrDefault();

                if (query != null)
                   return query.Convert();

                return null;

            }
            finally
            {
                Dispose();
            }            
        }
    }
}
