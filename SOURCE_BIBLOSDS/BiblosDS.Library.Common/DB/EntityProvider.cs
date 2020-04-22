using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Objects.Response;
using BiblosDS.Library.Common.Utility;

using log4net;

using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Transactions;
using DataObjects = System.Data.Objects;

namespace BiblosDS.Library.Common.DB
{
    public partial class EntityProvider
    {
        protected readonly ILog logger = LogManager.GetLogger(typeof(EntityProvider));
        private readonly Model.BiblosDS2010Entities _db;

        public EntityProvider()
        {
            _db = new Model.BiblosDS2010Entities(BiblosDSConnectionString);
            requireDispose = true;
            requireSave = true;
        }

        private bool requireDispose = false;
        private bool requireSave = false;
        private Model.BiblosDS2010Entities db
        {
            get
            {
                return _db;
            }
        }

        private void Dispose()
        {
            if (_db != null && requireDispose && _db.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified).Count() <= 0)
            {
                _db.Dispose();
                //_db = null;
            }

        }

        public void ForceDispose()
        {
            _db.Dispose();
        }

        #region IDbProvider ConnectionString

        public string BiblosDSConnectionString
        {
            get
            {
                if (AzureService.IsAvailable)
                {
                    var cnn = RoleEnvironment.GetConfigurationSettingValue("BiblosDS");
                    if (string.IsNullOrEmpty(cnn))
                        throw new Exception("Impostare una connessione \"BiblosDS\" nel file .cscfg.");
                    return cnn;
                }
                else
                {
                    if (ConfigurationManager.ConnectionStrings["BiblosDS"] == null)
                        throw new Exception("Impostare una connessione \"BiblosDS\" nel file .config.");

                    return ConfigurationManager.ConnectionStrings["BiblosDS"].ConnectionString;
                }
            }
        }

        #endregion

        #region DBVersion

        public string GetDatabaseVersion()
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                return db.DataBaseVersion.FirstOrDefault().LocalPath;
            }
        }

        #endregion

        #region Document

        public string GetTransitoLocalPath(DocumentTransito transito)
        {
            try
            {
                var transit = db.Transit.Where(x => x.IdTransit == transito.IdTransit).FirstOrDefault();
                if (transit == null)
                    throw new Exceptions.DocumentNotFound_Exception("Documento configurato in transito ma non presente.");
                return transit.LocalPath;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public string GetAttachTransitLocalPath(Guid IdDocumentAttach)
        {
            try
            {
                var transit = db.DocumentAttachTransit.Where(x => x.IdDocumentAttach == IdDocumentAttach).FirstOrDefault();
                if (transit == null)
                    throw new Exceptions.DocumentNotFound_Exception("Documento configurato in transito ma non presente.");
                return transit.LocalPath;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public int? CheckPrimaryKey(Guid IdArchive, Guid IdDocument, string PrimaryKeyValue)
        {
            try
            {
                return db.Document.Where(x => 
                    db.Document.Any(xx => xx.IdDocument == IdDocument 
                            && (xx.IdDocumentStatus == (short)Enums.DocumentStatus.InCache || xx.IdDocumentStatus == (short)Enums.DocumentStatus.InStorage || xx.IdDocumentStatus == (short)Enums.DocumentStatus.InTransito || xx.IdDocumentStatus == (short)Enums.DocumentStatus.MovedToPreservation))
                    && x.Archive.IdArchive == IdArchive 
                    && x.IdDocument != IdDocument && x.PrimaryKeyValue == PrimaryKeyValue 
                    && x.IdDocumentLink == null
                    && (x.IdDocumentStatus == (short)Enums.DocumentStatus.InCache || x.IdDocumentStatus == (short)Enums.DocumentStatus.InStorage || x.IdDocumentStatus == (short)Enums.DocumentStatus.InTransito || x.IdDocumentStatus == (short)Enums.DocumentStatus.MovedToPreservation)
                    && x.IsLatestVersion == true).Count();
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaxRetry">
        /// Numero massimo di tentativi possibili
        /// 0 for Infinite
        /// </param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentInTransito(Guid IdArchive, int MaxRetry)
        {
            try
            {
                var query = db.Transit
                    .Include("Document")
                    .Include("Document.PreservationDocuments")
                    .Include("Document.DocumentStatus")
                    .Include("Document.StorageArea")
                    .Include("Document.StorageArea.Storage")
                    .Include("Document.Storage")
                    .Include("Document.Storage.StorageType")
                    .Include("Document.Archive")
                    .Include("Document.DocumentLink")
                    .Include("Document.DocumentParent")
                    .Include("Document.DocumentNodeType").Where(x =>
                        (string.IsNullOrEmpty(x.ServerName) || x.ServerName == System.Environment.MachineName)
                        && x.Document.Archive.IdArchive == IdArchive
                        && (x.Retry <= MaxRetry || MaxRetry <= 0)).Select(x => x.Document);
                BindingList<Document> list = new BindingList<Document>();
                foreach (var item in query)
                {
                    if (item == null)
                        continue;
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentResponse GetDocumentInTransitoByArchive(string archiveName, int take, int skip)
        {
            var result = new DocumentResponse { Documents = new BindingList<Document>(), TotalRecords = 0 };
            try
            {
                var query = db.Transit
                    .Include("Document")
                    .Include("Document.PreservationDocuments")
                    .Include("Document.DocumentStatus")
                    .Include("Document.StorageArea")
                    .Include("Document.StorageArea.Storage")
                    .Include("Document.Storage")
                    .Include("Document.Storage.StorageType")
                    .Include("Document.Archive")
                    .Include("Document.DocumentLink")
                    .Include("Document.DocumentParent")
                    .Include("Document.DocumentNodeType")
                    .Where(x =>
                        (string.IsNullOrEmpty(x.ServerName) || x.ServerName == System.Environment.MachineName)
                        && x.Document.Archive.Name == archiveName
                        && (x.Status == null || x.Status.Value != (short)DocumentTarnsitoStatus.StorageProcessing))
                    .Select(x => x.Document);

                result.TotalRecords = query.Count();

                query = query
                    .OrderBy(x => x.IdDocument)
                    .Skip(skip)
                    .Take(take);

                foreach (var item in query)
                {
                    if (item == null)
                        continue;

                    result.Documents.Add(item.Convert());
                }
            }
            catch (Exception ex)
            {
                result.Error = new ResponseError(ex);
            }
            finally
            {
                Dispose();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaxRetry">
        /// Numero massimo di tentativi possibili
        /// 0 for Infinite
        /// </param>
        /// <returns></returns>
        public BindingList<Document> GetDocumentInTransito(int MaxRetry, bool waitToProcess)
        {
            try
            {
                var query = db.Transit
                    .Include("Document")
                    .Include("Document.PreservationDocuments")
                    .Include("Document.DocumentStatus")
                    .Include("Document.StorageArea")
                    .Include("Document.StorageArea.Storage")
                    .Include("Document.Storage")
                    .Include("Document.Storage.StorageType")
                    .Include("Document.Archive")
                    .Include("Document.DocumentLink")
                    .Include("Document.DocumentParent")
                    .Include("Document.DocumentNodeType")
                    .Where(x =>
                        (string.IsNullOrEmpty(x.ServerName) || x.ServerName == System.Environment.MachineName)
                        && (x.Retry <= MaxRetry || MaxRetry <= 0)
                        && (x.Status == null || x.Status.Value != (short)DocumentTarnsitoStatus.StorageProcessing))
                        .Select(x => x.Document);
                BindingList<Document> list = new BindingList<Document>();
                foreach (var item in query)
                {
                    if (waitToProcess && (DateTime.Now - item.DateCreated.GetValueOrDefault()).TotalMinutes < 1)
                        continue;
                    if (item == null)
                        continue;

                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public void DeleteChain(Guid idChain, bool isVisible)
        {
            try
            {
                var chainDocument = db.Document.Include(x => x.DocumentParent).Include(x => x.DocumentParent.DocumentParents).Include(x => x.DocumentParents).Include(x => x.DocumentParentVersions).Where(x => x.IdDocument == idChain).Single();
                SetDocumentVisibility(chainDocument, isVisible);
                foreach (var item in chainDocument.DocumentParentVersions)
                {
                    item.IsVisible = isVisible ? (short)1 : (short)0;
                    if (!isVisible)
                    {
                        item.PrimaryKeyValue = string.Empty;
                        item.IsDetached = true;
                    }
                }
                if (chainDocument.DocumentParent != null)
                {
                    SetDocumentVisibility(chainDocument.DocumentParent, isVisible);
                }
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        void SetDocumentVisibility(Model.Document doc, bool isVisible)
        {
            doc.IsVisible = isVisible ? (short)1 : (short)0;
            if (!isVisible)
            {
                doc.PrimaryKeyValue = string.Empty;
                doc.IsDetached = true;
            }

            foreach (var item in doc.DocumentParents)
            {
                SetDocumentVisibility(item, isVisible);
            }
        }

        public void DeleteDocument(Guid IdDocument, bool isVisible)
        {
            try
            {
                var entity = db.Document.Where(x => x.IdDocument == IdDocument).Single();
                entity.IsVisible = isVisible ? (short)1 : (short)0;
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal void DeleteDocumentAttach(Guid idDocumentAttach, bool visibility)
        {
            try
            {
                var entity = db.DocumentAttach.SingleOrDefault(x => x.IdDocumentAttach == idDocumentAttach);

                if (entity == null)
                    throw new BiblosDS.Library.Common.Exceptions.DocumentAttachNotFound_Exception();

                entity.IsVisible = visibility ? (short)1 : (short)0;
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public Document GetDocumentLatestVersion(Guid IdDocument)
        {
            try
            {
                DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentLatestVersionByGuidQuery(db, IdDocument);
                //LogQuery(queryToExecute as ObjectQuery);
                BindingList<Document> list = new BindingList<Document>();
                var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                var obj = query.FirstOrDefault();
                if (obj != null)
                {
                    return obj.Convert();
                }
                else
                    return null;
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Ricerca per IdDocumento e Versione
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <param name="VersionNumber"></param>
        /// <returns></returns>
        public Document GetDocument(Guid IdDocument, decimal? VersionNumber = null)
        {
            if (VersionNumber != null)
            {
                try
                {
                    DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentByGuidAndVersionQuery(db, IdDocument, VersionNumber);
                    var obj = queryToExecute.Execute(DataObjects.MergeOption.NoTracking).FirstOrDefault();
                    if (obj != default(Model.Document))
                        return obj.Convert();
                    else
                        return null;
                }
                finally
                {
                    Dispose();
                }
            }
            else
            {
                try
                {
                    DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentByGuidQuery(db, IdDocument);
                    BindingList<Document> list = new BindingList<Document>();
                    var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                    var obj = query.FirstOrDefault();
                    if (obj != null)
                    {
                        return obj.Convert();
                    }
                    else
                        return null;
                }
                finally
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// Ritorno la versione corrente del documento.
        /// Tra tutti i documenti presenti scelgo quello con la versione massima
        /// </summary>
        /// <param name="IdDocument"></param>
        /// <returns></returns>
        public decimal GetDocumentVersion(Guid IdDocument)
        {
            try
            {
                return GetDocumentVersionQuery(db, IdDocument);
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentResponse GetDocumentsPaged(IEnumerable<Guid> idDocuments, int skip = 0, int take = -1)
        {
            var ret = new DocumentResponse { Documents = new BindingList<Document>(), TotalRecords = 0 };

            try
            {
                var ids = idDocuments.ToArray<Guid>();

                ret.TotalRecords = db.Document
                    .Where(x => ids.Contains<Guid>(x.IdDocument))
                    .LongCount();

                if (take == 0) //Si vuole solo avere il conteggio dei documenti che sono presenti in db?
                    return ret;

                var query = db.Document
                    .Include("PreservationDocuments")
                    .Include("PreservationDocuments.Preservation")
                    .Include("DocumentStatus")
                    .Include("DocumentParent")
                    .Include("DocumentParentVersion")
                    .Include("DocumentLink")
                    .Include("StorageArea")
                    .Include("StorageArea.Storage")
                    .Include("Storage")
                    .Include("Storage.StorageType")
                    .Include("Archive")
                    .Include("CertificateStore")
                    .Where(x => ids.Contains<Guid>(x.IdDocument))
                    .OrderBy(x => x.IdDocument) as IQueryable<Model.Document>;

                if (skip > 0)
                    query = query.Skip<Model.Document>(skip);

                if (take > 0) //Da 1 in su, perchè se 0 è già stato ritornato il conteggio dei documenti, mentre se è -1 vengono presi TUTTI i documenti.
                    query = query.Take<Model.Document>(take);

                foreach (var doc in query)
                {
                    ret.Documents.Add(doc.Convert());
                }
            }
            finally
            {
                Dispose();
            }

            return ret;
        }

        /// <summary>
        /// Get a Document from IdBiblos.
        /// </summary>
        /// <param name="IdBiblos"></param>
        /// <param name="IdArchive"></param>
        /// <returns></returns>
        public Document GetDocument(int IdBiblos, Guid IdArchive)
        {
            try
            {
                DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentByIdQuery(db, IdBiblos, IdArchive);
                BindingList<Document> list = new BindingList<Document>();
                var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                var obj = query.FirstOrDefault();
                if (obj != null)
                {
                    return obj.Convert();
                }
                else
                    return null;
            }
            finally
            {
                Dispose();
            }
        }

        public Document GetDocumentChild(int IdBiblos, Guid IdArchive)
        {
            try
            {
                DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentChildByIdQuery(db, IdBiblos, IdArchive);
                BindingList<Document> list = new BindingList<Document>();
                var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                var obj = query.FirstOrDefault();
                if (obj != null)
                {
                    return obj.Convert();
                }
                else
                    return null;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// Get a Document from IdBiblos.
        /// </summary>
        /// <param name="IdBiblos"></param>
        /// <param name="IdArchive"></param>
        /// <returns></returns>
        public Document GetDocumentDirect(int IdBiblos, Guid IdArchive)
        {
            try
            {
                DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentDirectByIdQuery(db, IdBiblos, IdArchive);
                BindingList<Document> list = new BindingList<Document>();
                var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                var obj = query.FirstOrDefault();
                if (obj != null)
                {
                    return obj.Convert();
                }
                else
                    return null;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> GetDocumentsByAttributeValue(DocumentArchive archive, DocumentAttributeValue attributeValue, bool? stringContains, int? skip, int? take)
        {
            try
            {
                var query = db.Document
                    .Include(d => d.PreservationDocuments)
                    .Include(d => d.DocumentParent)
                    .Include(d => d.AttributesValue)
                    .Include(d => d.AttributesValue.First().Attributes)
                    .Where(d => d.IdArchive == archive.IdArchive
                        && (d.IsDetached == null || d.IsDetached == false)
                        && d.AttributesValue.Any(a => a.IdAttribute == attributeValue.Attribute.IdAttribute));

                switch (attributeValue.Attribute.AttributeType)
                {
                    case "System.Int64":
                        var intVal = attributeValue.Value.TryConvert<Int64>();
                        query = query.Where(d => d.AttributesValue.Any(a => a.ValueInt == intVal));
                        break;
                    case "System.Double":
                        var floatVal = attributeValue.Value.TryConvert<double>();
                        query = query.Where(d => d.AttributesValue.Any(a => a.ValueFloat == floatVal));
                        break;
                    case "System.DateTime":
                        var dateVal = attributeValue.Value.TryConvert<DateTime>();
                        query = query.Where(d => d.AttributesValue.Any(a => a.ValueDateTime == dateVal));
                        break;
                    default:
                        var strVal = attributeValue.Value.TryConvert<string>();
                        if (stringContains.GetValueOrDefault(false))
                            query = query.Where(d => d.AttributesValue.Any(a => a.ValueString.ToUpper().Contains(strVal.ToUpper())));
                        else
                            query = query.Where(d => d.AttributesValue.Any(a => a.ValueString.Equals(strVal, StringComparison.InvariantCultureIgnoreCase)));
                        break;
                }

                // Imposto i parametri per la paginazione.
                query = query.OrderBy(d => d.IdDocument);
                if (skip.HasValue)
                    query = query.Skip(skip.Value);
                if (take.HasValue)
                    query = query.Take(take.Value);

                BindingList<Document> result = new BindingList<Document>();
                foreach (var item in query)
                    result.Add(item.Convert());
                return result;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> GetAllDocuments(DocumentArchive archive, bool visible, int skip, int take, out int documentsInArchive)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                    .Include(x => x.PreservationDocuments)
                    .Include(x => x.AttributesValue)
                    .Include(x => x.AttributesValue.First().Attributes)
                    .Include(x => x.Archive)
                    .Include(x => x.DocumentParent)
                    .Include(x => x.Storage)
                    .Where(x => x.DocumentParent != null && x.IsVisible == (visible ? 1 : 0) && x.Archive.IdArchive == archive.IdArchive && !x.IdParentVersion.HasValue);
                documentsInArchive = query.Count();
                foreach (var item in query.OrderBy(x => x.DateCreated).Skip(skip).Take(take))
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }


        public BindingList<Document> GetAllDocumentsExt(DocumentArchive archive, bool visible, int skip, int take,
                                                        Func<IQueryable<Model.Document>, object, IQueryable<Model.Document>> queryExt, object pars, out int documentsInArchive)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                    .Where(x => x.IdParentBiblos.HasValue && x.IsVisible == (visible ? 1 : 0) && x.Archive.IdArchive == archive.IdArchive && x.IsLatestVersion);

                if (queryExt != null)
                    query = queryExt(query, pars);

                logger.Info(LogQuery(query as ObjectQuery));

                documentsInArchive = query.Count();

                foreach (var item in query.Skip(skip).Take(take))
                {
                    var doc = db.Document.Include(x => x.AttributesValue)
                    .Include(x => x.PreservationDocuments)
                    .Include(x => x.AttributesValue.First().Attributes)
                    .Include(x => x.Archive)
                    .Include(x => x.DocumentParent).Single(x => x.IdDocument == item.IdDocument);
                    list.Add(doc.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }


        public BindingList<Document> GetAllDocumentChains(DocumentArchive archive, int skip, int take, out int documentsInArchive)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document.Include(x => x.PreservationDocuments).Where(x => !x.IdParentBiblos.HasValue && x.IsVisible == 1 && !x.IdParentVersion.HasValue && x.Archive.IdArchive == archive.IdArchive);
                documentsInArchive = query.Count();
                foreach (var item in query.OrderBy(x => x.DateCreated).Skip(skip).Take(take))
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> GetChainDocuments(int IdBiblosParentDocument)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                  .Include("PreservationDocuments")
                  .Include("StorageArea")
                  .Include("StorageArea.Storage")
                  .Include("Storage")
                  .Include("Archive")
                  .Where(x => x.DocumentParent.IdBiblos == IdBiblosParentDocument);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> GetChainDocuments(Guid IdParentDocument)
        {
            try
            {
                DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetChainDocumentsByGuidQuery(db, IdParentDocument);
                BindingList<Document> list = new BindingList<Document>();
                var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> GetDocumentChildren(Guid idParent)
        {
            try
            {
                DataObjects.ObjectQuery<Model.Document> queryToExecute = (DataObjects.ObjectQuery<Model.Document>)GetDocumentChildrenQuery(db, idParent);
                BindingList<Document> converted = new BindingList<Document>();
                var result = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                foreach (var item in result)
                    converted.Add(item.Convert());
                return converted;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> GetChainHistoryDocuments(Guid IdParentDocument)
        {
            try
            {
                BindingList<Document> list = new BindingList<Document>();
                var query = db.Document
                  .Include("PreservationDocuments")
                  .Include("StorageArea")
                  .Include("StorageArea.Storage")
                  .Include("Storage")
                  .Include("Archive")
                  .Where(x => x.DocumentParent.IdDocument == IdParentDocument);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }


        public Guid AddDocumentAttributeGroup(DocumentAttributeGroup AttributeGroup)
        {
            Guid retval = Guid.NewGuid();
            Model.AttributesGroup entityAG = new Model.AttributesGroup
            {
                IdAttributeGroup = retval,
                IdArchive = Guid.NewGuid(),
                Description = AttributeGroup.Description,
            };
            return retval;
        }

        public int GetNextArchiveBiblosId(Document Document)
        {
            Model.Archive archive = null;
            try
            {
                archive = db.Archive.Where(x => x.IdArchive == Document.Archive.IdArchive).Single();
                archive.LastIdBiblos += 1;
                db.SaveChanges();
                return archive.LastIdBiblos;
            }
            catch (OptimisticConcurrencyException)
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public Document AddDocument(Document Document, Server server, string TransitoLocalPath)
        {
            try
            {
                Model.Archive archive = null;
                if (Document.DocumentParent != null && (!Document.DocumentParent.IdBiblos.HasValue || Document.DocumentParent.IdBiblos.Value <= 0))
                {
                    if (archive == null)
                        archive = db.Archive.Where(x => x.IdArchive == Document.Archive.IdArchive).Single();
                    archive.LastIdBiblos += 1;
                    Document.DocumentParent.IdBiblos = archive.LastIdBiblos;
                }
                if (!Document.IdBiblos.HasValue || Document.IdBiblos.Value <= 0)
                {
                    if (archive == null)
                        archive = db.Archive.Where(x => x.IdArchive == Document.Archive.IdArchive).Single();
                    archive.LastIdBiblos += 1;
                    Document.IdBiblos = archive.LastIdBiblos;
                }
                //Convert
                Document.IsLatestVersion = true;
                Model.Document entityDocument = Document.TryToConvertTo<Model.Document>(db);
                entityDocument.DateCreated = DateTime.Now;
                Model.Document documentParent = null;
                //Attach parent docuemnt or create parent
                if (Document.DocumentParent != null)
                {
                    entityDocument.IdParentBiblos = Document.DocumentParent.IdDocument;
                    Model.Document outParent = null;
                    if ((outParent = db.Document.Where(x => x.IdDocument == Document.DocumentParent.IdDocument).FirstOrDefault()) == null)
                    {
                        documentParent = Document.DocumentParent.TryToConvertTo<Model.Document>(db);
                        documentParent.DateCreated = DateTime.Now;
                        if (Document.DocumentParent.Archive != null)
                            documentParent.IdArchive = Document.DocumentParent.Archive.IdArchive;
                        if (Document.DocumentParent.Status != null)
                            documentParent.IdDocumentStatus = Document.DocumentParent.Status.IdStatus;
                    }
                    else
                        documentParent = outParent as Model.Document;
                    entityDocument.DocumentParent = documentParent;
                }
                //Attach all the refernce
                if (Document.Storage != null)
                    entityDocument.IdStorage = Document.Storage.IdStorage;
                if (Document.StorageArea != null)
                    entityDocument.IdStorageArea = Document.StorageArea.IdStorageArea;
                if (Document.DocumentLink != null)
                    entityDocument.IdDocumentLink = Document.DocumentLink.IdDocument;
                if (Document.DocumentParentVersion != null)
                    entityDocument.IdParentVersion = Document.DocumentParentVersion.IdDocument;
                if (Document.Certificate != null)
                    entityDocument.IdCertificate = Document.Certificate.IdCertificate;
                if (Document.Archive != null)
                    entityDocument.IdArchive = Document.Archive.IdArchive;
                if (Document.Status != null)
                    entityDocument.IdDocumentStatus = Document.Status.IdStatus;
                if (!string.IsNullOrEmpty(Document.IdPdf))
                    entityDocument.IdPdf = Document.IdPdf;
                if (!string.IsNullOrEmpty(Document.IdThumbnail))
                    entityDocument.IdThumbnail = Document.IdThumbnail;
                //
                if (Document.DocumentParent != null && Document.Content != null && Document.Status.IdStatus == (int)DocumentStatus.InTransito)
                {
                    var transit = new Model.Transit
                    {
                        IdTransit = Guid.NewGuid(),
                        LocalPath = TransitoLocalPath,
                        Status = (int)DocumentTarnsitoStatus.DaProcessare,
                        ServerName = Environment.MachineName,
                        DateCreated = DateTime.Now
                    };
                    entityDocument.Transit.Add(transit);
                    if (server != null)
                    {
                        entityDocument.DocumentServer.Add(new Model.DocumentServer { IdDocumentStatus = (int)DocumentStatus.InTransito, IdServer = server.IdServer, DateCreated = DateTime.Now });
                    }
                }

                if (Document.AttributeValues != null)
                    foreach (DocumentAttributeValue item in Document.AttributeValues)
                    {
                        if (item.Value != null)
                        {
                            Model.AttributesValue value = item.TryToConvertTo<Model.AttributesValue>(db);
                            value.IdAttribute = item.Attribute.IdAttribute;
                            value.IdDocument = entityDocument.IdDocument;
                            switch (item.Attribute.AttributeType)
                            {
                                case "System.String":
                                    value.ValueString = item.Value.TryConvert<string>();
                                    break;
                                case "System.Int64":
                                    value.ValueInt = item.Value.TryConvert<Int64>();
                                    break;
                                case "System.Double":
                                    value.ValueFloat = item.Value.TryConvert<double>();
                                    break;
                                case "System.DateTime":
                                    value.ValueDateTime = item.Value.TryConvert<DateTime>();
                                    break;
                                default:
                                    throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato.", item.Attribute.AttributeType));
                            }
                            value.IdAttribute = item.Attribute.IdAttribute;
                            entityDocument.AttributesValue.Add(value);
                        }
                    }
                if (Document.Permissions != null)
                    foreach (DocumentPermission item in Document.Permissions)
                    {
                        Model.Permission value = item.TryToConvertTo<Model.Permission>(db);
                        value.IdMode = (short)item.Mode;
                        entityDocument.Permission.Add(value);
                    }
                //Force the visibility of the document & Creation Date & primaryKey.                
                Document.IsVisible = true;
                Document.DateCreated = DateTime.Now;
                entityDocument.IsVisible = 1;
                entityDocument.DateCreated = DateTime.Now;
                db.AddToDocument(entityDocument);
                if (requireSave)
                    db.SaveChanges();
                return entityDocument.TryToConvertTo<Document>(true);
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateDocument(Document Document, Model.Document attachedEntity)
        {
            try
            {
                Model.Document entityDocument = Document.TryToConvertTo<Model.Document>(false);

                entityDocument.EntityKey = db.CreateEntityKey(entityDocument.GetType().Name, entityDocument);

                if (Document.Storage != null)
                    entityDocument.IdStorage = Document.Storage.IdStorage;
                if (Document.StorageArea != null)
                    entityDocument.IdStorageArea = Document.StorageArea.IdStorageArea;
                if (Document.DocumentParent != null)
                    entityDocument.IdParentBiblos = Document.DocumentParent.IdDocument;
                if (Document.DocumentLink != null)
                    entityDocument.IdDocumentLink = Document.DocumentLink.IdDocument;
                if (Document.Certificate != null)
                    entityDocument.IdCertificate = Document.Certificate.IdCertificate;
                if (Document.Archive != null)
                    entityDocument.IdArchive = Document.Archive.IdArchive;
                if (Document.Status != null)
                    entityDocument.IdDocumentStatus = Document.Status.IdStatus;

                db.ApplyCurrentValues(entityDocument.EntityKey.EntitySetName, entityDocument);
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateDocument(Document Document)
        {
            try
            {
                Model.Document entityDocument = Document.TryToConvertTo<Model.Document>(false);

                entityDocument.EntityKey = db.CreateEntityKey(entityDocument.GetType().Name, entityDocument);
                var attachedEntity = db.GetObjectByKey(entityDocument.EntityKey) as Model.Document;

                if (Document.Storage != null)
                    entityDocument.IdStorage = Document.Storage.IdStorage;
                if (Document.StorageArea != null)
                    entityDocument.IdStorageArea = Document.StorageArea.IdStorageArea;
                if (Document.DocumentParent != null)
                    entityDocument.IdParentBiblos = Document.DocumentParent.IdDocument;
                if (Document.DocumentLink != null)
                    entityDocument.IdDocumentLink = Document.DocumentLink.IdDocument;
                if (Document.DocumentParentVersion != null)
                    entityDocument.IdParentVersion = Document.DocumentParentVersion.IdDocument;
                if (Document.Certificate != null)
                    entityDocument.IdCertificate = Document.Certificate.IdCertificate;
                if (Document.Archive != null)
                    entityDocument.IdArchive = Document.Archive.IdArchive;
                if (Document.Status != null)
                    entityDocument.IdDocumentStatus = Document.Status.IdStatus;
                else
                    entityDocument.IdDocumentStatus = attachedEntity.IdDocumentStatus;
                if (Document.Preservation != null)
                {
                    Model.PreservationDocuments preservationDocument = db.PreservationDocuments.SingleOrDefault(x => x.IdPreservation == Document.Preservation.IdPreservation);
                    if (preservationDocument == null)
                    {
                        Model.PreservationDocuments item = new Model.PreservationDocuments
                        {
                            IdPreservation = Document.Preservation.IdPreservation,
                            Document = entityDocument,
                            RegistrationUser = "BiblosDS",
                            RegistrationDate = DateTimeOffset.UtcNow
                        };
                        db.AddToPreservationDocuments(item);
                    }
                }

                db.ApplyCurrentValues(entityDocument.EntityKey.EntitySetName, entityDocument);
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateDocumentStorageVersion(Guid IdDocument, decimal Version)
        {
            try
            {
                Model.Document entityDocument = new Model.Document { IdDocument = IdDocument };

                entityDocument.EntityKey = db.CreateEntityKey(entityDocument.GetType().Name, entityDocument);
                var attachedEntity = db.GetObjectByKey(entityDocument.EntityKey) as Model.Document;

                db.ApplyCurrentValues(entityDocument.EntityKey.EntitySetName, entityDocument);
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentTransito GetTransito(Guid idDocument)
        {
            Model.Transit entityTransito = db.Transit.Where(x => x.IdDocument == idDocument && (string.IsNullOrEmpty(x.ServerName) || x.ServerName == Environment.MachineName)).FirstOrDefault();
            if (entityTransito == null)
                return null;
            return entityTransito.Convert();
        }

        public DocumentTransito TransitoChangeState(DocumentTransito transito, DocumentTarnsitoStatus Status)
        {
            try
            {
                //string path = string.Empty;

                Model.Transit entityTransito = db.Transit.Include(x => x.Document).Where(x => x.IdTransit == transito.IdTransit).First();
                if (entityTransito == null)
                    throw new Exceptions.DocumentNotFound_Exception("Documento non in Transito.");
                //Accesso esclusivo in Transito da parte dello storage.
                if (Status == DocumentTarnsitoStatus.StorageProcessing && entityTransito.Status.HasValue && entityTransito.Status.Value == (short)DocumentTarnsitoStatus.StorageProcessing)
                    throw new Exceptions.StorageIsProcessingFile_Exception();
                //path = entityTransito.LocalPath;
                entityTransito.DateRetry = DateTime.Now;
                entityTransito.Status = (short)Status;
                if (Status == DocumentTarnsitoStatus.EndProcessing)
                {
                    db.Transit.DeleteObject(entityTransito);
                }
                else
                {
                    entityTransito.Retry += 1;
                }
                if (requireSave)
                    db.SaveChanges();

                return entityTransito.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public void DetachDocument(Document Document)
        {
            try
            {
                Model.Document entityDocument = _db.Document.Where(x => x.IdDocument == Document.IdDocument).FirstOrDefault();
                if (entityDocument == null)
                    return;
                entityDocument.PrimaryKeyValue = null;
                entityDocument.IsDetached = true;

                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<string> GetPreservationFiscalDocumentsTypes()
        {
            return new BindingList<string>(db.PreservationFiscalDocumentType.Select(x => x.Name).ToList());
        }

        #endregion

        #region Storage

        public BindingList<DocumentStorage> GetStoragesActiveFromArchive(Guid IdArchive)
        {
            try
            {
                var query = db.ArchiveStorage
                    .Include("Storage")
                    .Include("Storage.StorageType").Where(x => x.IdArchive == IdArchive && x.Active == (short)1).Select(x => x.Storage).OrderByDescending(x => x.Priority);
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorage> GetStoragesActiveFromArchiveServer(Guid idArchive, Guid idServer)
        {
            try
            {
                var query = db.ArchiveStorage
                    .Include("Storage")
                    .Include("Storage.StorageType").Where(x => x.IdArchive == idArchive && x.Storage.IdServer == idServer && x.Active == (short)1).Select(x => x.Storage).OrderByDescending(x => x.Priority);
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorage> GetStoragesFromArchive(Guid IdArchive)
        {
            try
            {
                var query = db.ArchiveStorage
                    .Include("Storage")
                    .Include("Storage.StorageType").Where(x => x.IdArchive == IdArchive).Select(x => x.Storage).OrderByDescending(x => x.Priority);
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorage> GetStorages()
        {
            try
            {
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                var query = db.Storage.Include("StorageType").Where(x => !x.IsVisible.HasValue || x.IsVisible.Value != 0);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorage> GetStoragesWithServer()
        {
            try
            {
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                var query = db.Storage
                    .Include(x => x.StorageType)
                    .Include(x => x.Server)
                    .Where(x => !x.IsVisible.HasValue || x.IsVisible.Value != 0);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentStorage GetStorage(Guid IdStorage)
        {
            try
            {
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                var query = db.Storage.Include("StorageType").Where(x => x.IdStorage == IdStorage).FirstOrDefault();
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentStorage GetStorageWithServer(Guid IdStorage)
        {
            try
            {
                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                var query = db.Storage.Include("StorageType").Include("Server").Where(x => x.IdStorage == IdStorage).FirstOrDefault();
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public System.ComponentModel.BindingList<DocumentStorageRule> GetStorageRulesFromStorage(Guid IdStorage)
        {
            try
            {
                BindingList<DocumentStorageRule> list = new BindingList<DocumentStorageRule>();
                var query = db.StorageRule
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Include("Attributes.Archive")
                    .Where(x => x.Storage.IdStorage == IdStorage);
                foreach (var item in query)
                {
                    list.Add(item.TryToConvertTo<DocumentStorageRule>(true));
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorage> GetStoragesNotRelatedToArchive(Guid IdArchive)
        {
            try
            {
                //where !(from s in db.ArchiveStorage
                //              where s.IdArchive != IdArchive
                //              && s.IdStorage == m.IdStorage
                //              select s.IdStorage).Any()
                var q = db.Storage.Where(x => !(db.ArchiveStorage.Any(y => y.IdStorage == x.IdStorage && y.IdArchive == IdArchive)));

                BindingList<DocumentStorage> list = new BindingList<DocumentStorage>();
                foreach (var item in q)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorageRule> GetStorageRuleFromStorageArchive(Guid IdStorage, Guid IdArchive)
        {
            try
            {
                BindingList<DocumentStorageRule> list = new BindingList<DocumentStorageRule>();

                var query = db.StorageRule
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Include("RuleOperator")
                    .Where(x => x.Storage.IdStorage == IdStorage
                        && x.Attributes.Archive.IdArchive == IdArchive);
                foreach (var item in query)
                {
                    list.Add(item.TryToConvertTo<DocumentStorageRule>(true));
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        #endregion

        #region Attributes


        public BindingList<DocumentAttribute> GetAttributesFromArchive(Guid IdArchive)
        {
            try
            {
                BindingList<DocumentAttribute> list = new BindingList<DocumentAttribute>();
                var query = db.Attributes
                    .Include(x => x.AttributesMode)
                    .Include(x => x.Archive)
                    .Include(x => x.AttributesGroup)
                    .Where(x => x.Archive.IdArchive == IdArchive && (!x.IsVisible.HasValue || x.IsVisible != 0))
                    .OrderBy(x => x.ConservationPosition);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentAttribute GetAttribute(Guid IdAttribute)
        {
            try
            {
                var query = db.Attributes.Include(x => x.AttributesMode).Include(x => x.AttributesGroup).Include(x => x.Archive)
                  .Where(x => x.IdAttribute == IdAttribute && (!x.IsVisible.HasValue || x.IsVisible != 0)).First();
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentAttribute> GetAttributesFromArchiveStorage(Guid IdArchive, Guid IdStorage)
        {
            try
            {
                BindingList<DocumentAttribute> list = new BindingList<DocumentAttribute>();
                var query = db.ArchiveStorage
                    .Include(x => x.Archive.Attributes)
                    .Include(x => x.Archive.Attributes.First().AttributesMode)
                    .Include(x => x.Archive.Attributes.First().AttributesGroup)
                    .Where(x => x.Archive.IdArchive == IdArchive && x.Storage.IdStorage == IdStorage).Select(x => x.Archive.Attributes);
                foreach (var item in query)
                {
                    foreach (var attr in item)
                    {
                        list.Add(attr.Convert());
                    }
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentAttributeValue> GetAttributesValuesFromDocument(Guid IdDocument)
        {
            try
            {
                var query = db.AttributesValue
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Where(x => x.IdDocument == IdDocument);
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentAttributeValue> GetFullDocumentAttributeValues(Guid idDocument)
        {
            Model.Document currentDocument = ((ObjectQuery)db.Document.Where(x => x.IdDocument == idDocument)).Execute(MergeOption.NoTracking).OfType<Model.Document>().Single();
            ICollection<Model.AttributesValue> attributesValues = ((ObjectQuery)db.AttributesValue.Include(i => i.Attributes).Where(x => x.IdDocument == idDocument)).Execute(MergeOption.NoTracking).OfType<Model.AttributesValue>().ToList();
            ICollection<DocumentAttributeValue> documentAttributeValues = attributesValues.Select(s => s.Convert()).ToList();
            bool archiveHasChainAttributes = db.Attributes.Any(x => x.IdArchive == currentDocument.IdArchive && x.IsChainAttribute == 1);
            if (archiveHasChainAttributes && currentDocument.IdParentBiblos.HasValue && currentDocument.IdParentBiblos.Value != Guid.Empty)
            {
                IEnumerable<DocumentAttributeValue> chainAttributeValues = db.AttributeValues_FX_FullDocumentAttributeValues(currentDocument.IdParentBiblos).ToList().Select(s => s.Convert());
                foreach (DocumentAttributeValue chainAttributeValue in chainAttributeValues.Where(x => !documentAttributeValues.Any(xx => xx.IdAttribute == x.IdAttribute)))
                {
                    documentAttributeValues.Add(chainAttributeValue);
                }
            }

            BindingList<DocumentAttributeValue> results = new BindingList<DocumentAttributeValue>();
            foreach (DocumentAttributeValue attributesValue in documentAttributeValues)
            {
                results.Add(attributesValue);
            }
            return results;
        }
        #endregion


        #region AttributeGroup

        public BindingList<DocumentAttributeGroup> GetAttributeGroup(Guid IdArchive)
        {
            try
            {
                var query = db.AttributesGroup.Where(idArch => idArch.IdArchive == IdArchive && (!idArch.IsVisible.HasValue || idArch.IsVisible != 0)).ToList();
                BindingList<DocumentAttributeGroup> retval = new BindingList<DocumentAttributeGroup>();
                query.ForEach(x => retval.Add(
                    //x.TryToConvertTo<DocumentAttributeGroup>(true)
                    new DocumentAttributeGroup
                    {
                        Description = x.Description,
                        GroupType = (AttributeGroupType)Enum.Parse(typeof(AttributeGroupType), x.IdAttributeGroupType.ToString()),
                        IdArchive = IdArchive,
                        IdAttributeGroup = x.IdAttributeGroup,
                    }
                    ));
                return retval;
                //return new DocumentAttributeGroup 
                //{
                //    Description = query.Description,
                //    GroupType = (AttributeGroupType)Enum.Parse(typeof(AttributeGroupType), query.IdAttributeGroupType.ToString()),
                //    IdArchive = IdArchive,
                //    IdAttributeGroup = query.IdAttributeGroup,
                //};
            }
            finally
            {
                Dispose();
            }
        }

        public void AddAttributeGroup(DocumentAttributeGroup AttributeGroup)
        {
            db.AddToAttributesGroup(new Model.AttributesGroup
            {
                IdArchive = AttributeGroup.IdArchive,
                Description = AttributeGroup.Description,
                IdAttributeGroup = AttributeGroup.IdAttributeGroup,
                IdAttributeGroupType = (int)AttributeGroup.GroupType,
                IsVisible = AttributeGroup.IsVisible.HasValue ? (short)(AttributeGroup.IsVisible.Value ? 1 : 0) : (short)0,
            });
            db.SaveChanges();
        }

        public void UpdateAttributeGroup(DocumentAttributeGroup AttributeGroup)
        {
            if (AttributeGroup != null)
            {
                if (!string.IsNullOrEmpty(AttributeGroup.Description)
                    && AttributeGroup.IdAttributeGroup != null)
                {
                    bool done = false;
                    if (AttributeGroup.IdAttributeGroup != null)
                    {
                        var ToUpdate = db.AttributesGroup.Where(idAttGrp => idAttGrp.IdArchive == AttributeGroup.IdArchive).FirstOrDefault();
                        if (ToUpdate != null)
                        {
                            done = true;
                            ToUpdate.Description = AttributeGroup.Description;
                            ToUpdate.IdArchive = AttributeGroup.IdArchive;
                            ToUpdate.IdAttributeGroupType = (int)AttributeGroup.GroupType;
                            if (AttributeGroup.IsVisible.HasValue)
                            {
                                ToUpdate.IsVisible = (short)(AttributeGroup.IsVisible.Value ? 1 : 0);
                            }
                            else
                            {
                                ToUpdate.IsVisible = null;
                            }
                        }
                    }
                    if (!done)
                    {
                        if (AttributeGroup.IdArchive != null)
                        {
                            var ToUpdate = db.AttributesGroup.Where(idArch => idArch.IdArchive == AttributeGroup.IdArchive);
                            foreach (Model.AttributesGroup ag in ToUpdate)
                            {
                                ag.Description = AttributeGroup.Description;
                                ag.IdArchive = AttributeGroup.IdArchive;
                                ag.IdAttributeGroupType = (int)AttributeGroup.GroupType;
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        public void DeleteAttributeGroup(Guid IdAttributeGroup)
        {
            if (IdAttributeGroup != null && IdAttributeGroup != Guid.Empty)
            {
                var l = db.AttributesGroup.Where(x => x.IdAttributeGroup == IdAttributeGroup).ToList();
                l.ForEach(x => x.IsVisible = 0);
                db.SaveChanges();
            }
        }

        public bool ExistMainDateAttribute(Guid idArchive)
        {
            return db.Attributes.Where(x => x.IdArchive == idArchive && x.IsMainDate == 1).Any();
        }
        #endregion
        #region Storage Area

        public BindingList<DocumentStorageArea> GetStorageAreaActiveFromStorage(Guid IdStorage)
        {
            try
            {
                DataObjects.ObjectQuery<Model.StorageArea> queryToExecute = (DataObjects.ObjectQuery<Model.StorageArea>)GetStorageAreaActiveByStorageQuery(db, IdStorage);
                var query = queryToExecute.Execute(DataObjects.MergeOption.NoTracking);
                BindingList<DocumentStorageArea> list = new BindingList<DocumentStorageArea>();
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorageArea> GetStorageAreaFromStorage(Guid IdStorage)
        {
            try
            {
                BindingList<DocumentStorageArea> list = new BindingList<DocumentStorageArea>();
                var query = db.StorageArea.Where(x => x.Storage.IdStorage == IdStorage).OrderByDescending(x => x.Priority);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateStorageAreaSize(Guid IdStorageArea, long Size)
        {
            //using (TransactionScope scope = new TransactionScope())
            //{
            //try
            //{
            var entity = db.StorageArea.Include(x => x.StorageAreaRule).Include(x => x.StorageAreaRule.First().Attributes).Where(x => x.IdStorageArea == IdStorageArea).FirstOrDefault();
            if (entity.CurrentSize.HasValue)
                entity.CurrentSize += Size;
            else
                entity.CurrentSize = Size;
            if (entity.CurrentFileNumber.HasValue)
                entity.CurrentFileNumber += 1;
            else
                entity.CurrentFileNumber = 1;
            if (requireSave)
                db.SaveChanges(DataObjects.SaveOptions.DetectChangesBeforeSave);
            if (entity.MaxSize > 0 || entity.MaxFileNumber > 0)
            {
                if ((entity.MaxSize > 0 && entity.CurrentSize > entity.MaxSize) || (entity.MaxFileNumber > 0 && entity.CurrentFileNumber == entity.MaxFileNumber))
                {
                    entity.Enable = 0;
                    db.SaveChanges();
                    DocumentStorageArea s = entity.Convert();
                    s.IdStorageArea = Guid.NewGuid();
                    string[] oldFormat = s.Path.Split('_');
                    if (oldFormat.Count() > 2 && oldFormat[2] == entity.IdStorageArea.ToString())
                    {
                        s.Path = string.Format("{0:yyyMMdd}_", DateTime.Now) + (string.IsNullOrEmpty(s.Path) ? oldFormat[1] : oldFormat[1] + "_") + s.IdStorageArea;
                    }
                    else
                        s.Path = string.Format("{0:yyyMMdd}_", DateTime.Now) + (string.IsNullOrEmpty(s.Path) ? "" : s.Path + "_") + s.IdStorageArea;
                    BindingList<DocumentStorageRule> rules = new BindingList<DocumentStorageRule>();
                    foreach (var item in entity.StorageAreaRule)
                    {
                        rules.Add(item.TryToConvertTo<DocumentStorageRule>(true));
                    }
                    Model.StorageArea entityToSave = s.TryToConvertTo<Model.StorageArea>(false);
                    //Format name
                    string[] oldNameFormat = entity.Name.Split('_');
                    if (oldNameFormat.Count() >= 1)
                    {
                        entityToSave.Name = oldNameFormat[0] + string.Format("_{0:ddMMyyy HHmmss}", DateTime.Now);
                    }
                    else
                        entityToSave.Name = entity.Name + string.Format("_{0:ddMMyyy HHmmss}", DateTime.Now);
                    entityToSave.StorageReference.EntityKey = entity.StorageReference.EntityKey;
                    entityToSave.StorageStatusReference.EntityKey = entity.StorageStatusReference.EntityKey;
                    foreach (var item in rules)
                    {
                        Model.StorageAreaRule rule = item.TryToConvertTo<Model.StorageAreaRule>(db);
                        rule.IdStorageArea = entityToSave.IdStorageArea;
                        rule.IdAttribute = item.Attribute.IdAttribute;
                        rule.IdAttribute = item.Attribute.IdAttribute;
                        entityToSave.StorageAreaRule.Add(rule);
                    }
                    entityToSave.CurrentFileNumber = 0;
                    entityToSave.CurrentSize = 0;
                    entityToSave.Enable = 1;
                    entity.Enable = 0;
                    if (entityToSave.IdStorage == Guid.Empty)
                        entityToSave.IdStorage = entity.IdStorage;
                    db.AddToStorageArea(entityToSave);
                    //                            
                    if (requireSave)
                        db.SaveChanges(DataObjects.SaveOptions.DetectChangesBeforeSave);
                }

            }
            //    scope.Complete();
            //}                
            //finally
            //{
            //    Dispose();
            //}
            //}
        }

        public DocumentStorageRule GetStorageRule(Guid IdStorage, Guid IdAttibute)
        {
            try
            {
                using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
                {
                    var query = db.StorageRule
                        .Include("Attributes")
                        .Include("Storage")
                        .Include("Attributes.Archive")
                        .Include("RuleOperator")
                        .Where(x => x.Storage.IdStorage == IdStorage
                            && x.Attributes.IdAttribute == IdAttibute
                            && (!x.Attributes.IsVisible.HasValue || x.Attributes.IsVisible.Value != 0)).FirstOrDefault();
                    if (query == null)
                        return null;
                    else
                        return query.TryToConvertTo<DocumentStorageRule>(true);
                    //        var query = from m in db.GetStorageRuleById(IdStorage, IdAttibute)
                    //                    select new DocumentStorageRule()
                    //                    {
                    //                        Attribute = new DocumentAttribute()
                    //                        {
                    //                            IdAttribute = m.IdAttribute
                    //                        },
                    //                        Storage = new DocumentStorage()
                    //                        {
                    //                            IdStorage = m.IdStorage
                    //                        },
                    //                        RuleOrder = m.RuleOrder,
                    //                        RuleFilter = m.RuleFilter,
                    //                        RuleFormat = m.RuleFormat
                    //                    };
                    //        return query.Single();
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            finally
            {
                Dispose();
            }
        }
        #endregion

        #region Storage Area Rule

        public BindingList<DocumentStorageAreaRule> GetStorageRulesFromStorageArea(Guid IdStorageArea)
        {
            try
            {
                BindingList<DocumentStorageAreaRule> list = new BindingList<DocumentStorageAreaRule>();
                var query = db.StorageAreaRule
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Where(x => x.StorageArea.IdStorageArea == IdStorageArea);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentStorageAreaRule> GetStorageRulesFromStorageAreaArchive(Guid IdStorageArea, Guid IdArchive)
        {
            try
            {
                BindingList<DocumentStorageAreaRule> list = new BindingList<DocumentStorageAreaRule>();
                var query = db.StorageAreaRule
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Include("RuleOperator")
                    .Where(x => x.StorageArea.IdStorageArea == IdStorageArea && x.Attributes.Archive.IdArchive == IdArchive);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        #endregion

        #region IDbProvider Members


        public void CheckIn(Guid IdDocument, string UserId)
        {
            try
            {
                var entity = db.Document.Where(x => x.IdDocument == IdDocument).FirstOrDefault();
                if (entity == null)
                    throw new Exceptions.DocumentNotFound_Exception();
                entity.IsCheckOut = 0;
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void CheckOut(Guid IdDocument, string UserId)
        {
            try
            {
                var entity = db.Document.Where(x => x.IdDocument == IdDocument).FirstOrDefault();
                if (entity == null)
                    throw new Exceptions.DocumentNotFound_Exception();
                entity.IdUserCheckOut = UserId;
                entity.IsCheckOut = 1;
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void UndoCheckOut(Guid IdDocument, string UserId)
        {
            try
            {
                var entity = db.Document.Where(x => x.IdDocument == IdDocument).FirstOrDefault();
                if (entity == null)
                    throw new Exceptions.DocumentNotFound_Exception();
                entity.IsCheckOut = 0;
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentCertificate GetCertificateDefault()
        {
            try
            {

                var query = db.CertificateStore.Where(x => x.IsDefault == 1).FirstOrDefault();
                if (query == null)
                    throw new Exception("Default certificate not fount");
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentCertificate GetCertificate(Guid IdCertificate)
        {
            try
            {
                var query = db.CertificateStore.Where(x => x.IdCertificate == IdCertificate).FirstOrDefault();
                if (query == null)
                    throw new Exception("Default certificate not fount");
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        #endregion

        #region Log

        public void AddLog(BiblosDS.Library.Common.Enums.LoggingOperationType OperationType, Guid IdEntry, Guid IdArchive, Guid IdStorage, Guid IdCorrelation, string Message, string Server, string Client)
        {
            try
            {
                Model.Log log = new Model.Log
                {
                    IdEntry = IdEntry,
                    IdOperationType = (short)OperationType,
                    IdArchive = IdArchive,
                    IdStorage = IdStorage,
                    IdCorrelation = IdCorrelation,
                    Message = Message,
                    Server = Server,
                    Client = Client,
                    TimeStamp = DateTime.Now
                };
                db.AddToLog(log);
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void AddJournal(BiblosDS.Library.Common.Enums.LoggingOperationType OperationType, string UserAgent, Guid IdEntry, Guid IdArchive, Guid IdStorage, Guid IdCorrelation, string Message)
        {
            try
            {
                Model.Journal entity = new Model.Journal
                {
                    IdEntry = IdEntry,
                    IdOperationType = (short)OperationType,
                    UserAgent = UserAgent,
                    IdArchive = IdArchive,
                    IdStorage = IdStorage,
                    IdCorrelation = IdCorrelation,
                    Message = Message,
                    TimeStamp = DateTime.Now
                };
                db.AddToJournal(entity);
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Log> GetAllLogs(DateTime from, DateTime to)
        {
            BindingList<Log> retval = new BindingList<Log>();
            var query = db.Log.Where(x => x.TimeStamp >= from && x.TimeStamp <= to);
            if (query != null)
            {
                foreach (Model.Log l in query)
                    retval.Add(l.TryToConvertTo<Log>(true));
            }
            return retval;
        }

        public BindingList<Log> GetArchiveLogs(DateTime from, Guid IdArchive)
        {
            BindingList<Log> retval = new BindingList<Log>();
            var query = db.Log.Where(x => x.TimeStamp >= from && x.IdArchive == IdArchive);
            if (query != null)
            {
                foreach (Model.Log l in query)
                    retval.Add(l.TryToConvertTo<Log>(true));
            }
            return retval;
        }

        public BindingList<Log> GetLogsPaged(DateTime from, DateTime to, Guid? IdArchive, int skip, int take, out int totalRecord)
        {
            totalRecord = 0;
            BindingList<Log> retval = new BindingList<Log>();
            var query = db.Log.Where(x => x.TimeStamp >= from && x.TimeStamp <= to && (!IdArchive.HasValue || x.IdArchive == IdArchive.Value)).OrderBy(x => x.TimeStamp);
            if (query != null)
            {
                LogQuery(query as ObjectQuery);
                totalRecord = query.Count();
                foreach (Model.Log l in query.Skip(skip).Take(take))
                    retval.Add(l.TryToConvertTo<Log>(true));
            }
            return retval;
        }

        public BindingList<Guid> GetLogIDArchives()
        {
            var query = db.Log.Select(x => x.IdArchive).Distinct();
            return new BindingList<Guid>((query != null) ? query.ToArray() : null);
        }

        #endregion

        #region Private

        public bool ShortToBool(Int16 value)
        {
            return value.Equals((short)1);
        }

        public bool ShortToBool(Int16? value)
        {
            return value != null && value.Equals((short)1);
        }

        #endregion

        #region Archive

        public DocumentArchive GetArchive(Guid IdArchive)
        {
            try
            {
                var query = db.Archive
                    .Where(x => x.IdArchive == IdArchive)
                    .FirstOrDefault();
                //return query.TryToConvertTo<DocumentArchive>(true);
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentArchive GetArchiveWithServerConfigs(Guid IdArchive)
        {
            try
            {
                var query = db.Archive
                    .Include(x => x.ArchiveServerConfig)
                    .Include(x => x.ArchiveServerConfig.First().Server)
                    .Where(x => x.IdArchive == IdArchive)
                    .FirstOrDefault();
                //return query.TryToConvertTo<DocumentArchive>(true);
                return query.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<PreservationArchiveInfoResponse> GetLegalArchives(string domainUserName)
        {
            var list = new BindingList<PreservationArchiveInfoResponse>();

            var query = db.Archive
                .Include(x => x.PreservationUserRole)
                .Include(x => x.PreservationUserRole.First().PreservationRole)
                .Include(x => x.PreservationUserRole.First().PreservationUser)
                .Where(x => x.IsLegal == 1)
                .OrderBy(x => x.Name);

            foreach (var item in query)
            {
                var info = new PreservationArchiveInfoResponse();
                info.Archive = item.Convert();
                info.UserArchiveRole = item.PreservationUserRole.Where(x => x.PreservationUser.DomainUser.Equals(domainUserName, StringComparison.InvariantCultureIgnoreCase)).Select(x => x.PreservationRole.Convert(0, 1)).ToList();
                list.Add(info);
            }

            return list;
        }

        public BindingList<DocumentArchive> GetArchives(int skip, int take, DocumentCondition filter, List<DocumentSortCondition> sort, out int totalItems)
        {
            try
            {
                var whereClause = string.Empty;
                var parameters = new List<object>();
                BindingList<DocumentArchive> list = new BindingList<DocumentArchive>();

                var query = db.Archive.AsQueryable();
                ProcessFilters<Model.Archive>(filter, ref query);
                totalItems = query.Count();
                foreach (var item in sort)
                {
                    query = query.OrderBy(item.Name + " " + item.Dir);
                }
                foreach (var item in query.Skip(skip).Take(take))
                {
                    //list.Add(item.TryToConvertTo<DocumentArchive>(true));
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public static void ProcessFilters<T>(DocumentCondition filters, ref IQueryable<T> queryable)
        {
            var whereClause = string.Empty;
            var parameters = new List<object>();
            if (filters.DocumentAttributeConditions == null)
                return;
            for (int i = 0; i < filters.DocumentAttributeConditions.Count; i++)
            {
                var f = filters.DocumentAttributeConditions[i];

                if (f.DocumentAttributeConditions == null)
                {
                    if (i == 0)
                        whereClause += BuildWherePredicate<T>(f, i, parameters) + " ";
                    if (i != 0)
                        whereClause += ToLinqOperator(f.Condition) + BuildWherePredicate<T>(f, i, parameters) + " ";
                    if (i == (filters.DocumentAttributeConditions.Count - 1))
                    {
                        TrimWherePredicate(ref whereClause);
                        queryable = queryable.Where(whereClause, parameters.ToArray());
                    }
                }
                else
                    ProcessFilters(f, ref queryable);
            }
        }

        public static string TrimWherePredicate(ref string whereClause)
        {
            switch (whereClause.Trim().Substring(0, 2).ToLower())
            {
                case "&&":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
                case "||":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
            }

            return whereClause;
        }

        public static string BuildWherePredicate<T>(DocumentCondition filter, int index, List<object> parameters)
        {
            var entityType = (typeof(T));
            PropertyInfo property;

            if (filter.Name.Contains("."))
                property = GetNestedProp<T>(filter.Name);
            else
                property = entityType.GetProperty(filter.Name);

            var parameterIndex = parameters.Count;

            switch (filter.Operator)
            {
                case DocumentConditionFilterOperator.IsEqualTo:
                    if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                    {
                        parameters.Add(filter.Value);
                        return string.Format("EntityFunctions.TruncateTime(" + filter.Value + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                    }
                    if (typeof(int).IsAssignableFrom(property.PropertyType))
                    {
                        parameters.Add(filter.Value);
                        return string.Format(filter.Name + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                    }
                    parameters.Add(filter.Value);
                    return string.Format(filter.Name + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                case DocumentConditionFilterOperator.Contains:
                    parameters.Add(filter.Value);
                    return filter.Name + ".Contains(" + "@" + parameterIndex + ")";
                case DocumentConditionFilterOperator.IsNullOrEmpty:
                    parameters.Add(filter.Value);
                    return filter.Name + ".IsNullOrEmpty(" + "@" + parameterIndex + ")";
                default:
                    throw new NotImplementedException();
            }
            //switch (filter.Operator.ToLower())
            //{
            //    case "eq":
            //    case "neq":
            //    case "gte":
            //    case "gt":
            //    case "lte":
            //    case "lt":
            //        if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
            //        {
            //            parameters.Add(DateTime.Parse(filter.Value).Date);
            //            return string.Format("EntityFunctions.TruncateTime(" + filter.Field + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
            //        }
            //        if (typeof(int).IsAssignableFrom(property.PropertyType))
            //        {
            //            parameters.Add(int.Parse(filter.Value));
            //            return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
            //        }
            //        parameters.Add(filter.Value);
            //        return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
            //    case "startswith":
            //        parameters.Add(filter.Value);
            //        return filter.Field + ".StartsWith(" + "@" + parameterIndex + ")";
            //    case "endswith":
            //        parameters.Add(filter.Value);
            //        return filter.Field + ".EndsWith(" + "@" + parameterIndex + ")";
            //    case "contains":
            //        parameters.Add(filter.Value);
            //        return filter.Field + ".Contains(" + "@" + parameterIndex + ")";
            //    default:
            //        throw new ArgumentException("This operator is not yet supported for this Grid", filter.Operator);
            //}
        }

        public static string ToLinqOperator(DocumentConditionFilterCondition @condition)
        {
            switch (@condition)
            {
                case DocumentConditionFilterCondition.And:
                    return " || ";
                case DocumentConditionFilterCondition.Or:
                    return " && ";
                default:
                    return "";
            }
        }
        public static string ToLinqOperator(DocumentConditionFilterOperator @operator)
        {
            switch (@operator)
            {
                case DocumentConditionFilterOperator.IsEqualTo:
                    return " == ";
                case DocumentConditionFilterOperator.Contains:
                    return "";
                default:
                    return "";
            }
            //switch (@operator.ToLower())
            //{
            //    case "eq":
            //        return " == ";
            //    case "neq":
            //        return " != ";
            //    case "gte":
            //        return " >= ";
            //    case "gt":
            //        return " > ";
            //    case "lte":
            //        return " <= ";
            //    case "lt":
            //        return " < ";
            //    case "or":
            //        return " || ";
            //    case "and":
            //        return " && ";
            //    default:
            //        return null;
            //}
        }

        public static string ToLinqOperator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq":
                    return " == ";
                case "neq":
                    return " != ";
                case "gte":
                    return " >= ";
                case "gt":
                    return " > ";
                case "lte":
                    return " <= ";
                case "lt":
                    return " < ";
                case "or":
                    return " || ";
                case "and":
                    return " && ";
                default:
                    return null;
            }
        }

        public static PropertyInfo GetNestedProp<T>(String name)
        {
            PropertyInfo info = null;
            var type = (typeof(T));
            foreach (var prop in name.Split('.'))
            {
                info = type.GetProperty(prop);
                type = info.PropertyType;
            }
            return info;
        }

        public BindingList<DocumentArchive> GetArchives()
        {
            try
            {
                BindingList<DocumentArchive> list = new BindingList<DocumentArchive>();
                foreach (var item in db.Archive)
                {
                    //list.Add(item.TryToConvertTo<DocumentArchive>(true));
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentArchive> GetArchivesById(IEnumerable<Guid> idsArchive)
        {
            var retval = new BindingList<DocumentArchive>();

            try
            {
                var query = db.Archive
                    .Where(x => idsArchive.Contains(x.IdArchive));

                foreach (var arch in query)
                {
                    retval.Add(arch.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        public BindingList<DocumentArchive> GetArchivesByIdPaged(IEnumerable<Guid> idsArchive, int skip, int take, out int total)
        {
            var retval = new BindingList<DocumentArchive>();
            total = 0;
            try
            {
                IQueryable<Model.Archive> query = db.Archive
                    .Where(x => idsArchive.Contains(x.IdArchive));

                total = db.Archive
                    .Count(x => idsArchive.Contains(x.IdArchive));

                if (skip > 0 || take > 0)
                {
                    query = query.OrderBy(x => x.IdArchive);

                    if (skip > 0)
                        query = query.Skip(skip);

                    if (take > 0)
                        query = query.Take(take);
                }

                foreach (var arch in query)
                {
                    retval.Add(arch.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                }
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        public BindingList<DocumentArchive> GetArchivesFromStorage(Guid IdStorage)
        {
            try
            {
                var query = db.ArchiveStorage
                    .Include("Archive").Where(x => x.IdStorage == IdStorage).Select(x => x.Archive);
                BindingList<DocumentArchive> list = new BindingList<DocumentArchive>();
                foreach (var item in query)
                {
                    //list.Add(item.TryToConvertTo<DocumentArchive>(true));
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<DocumentArchive> GetArchivesNotRelatedToStorage(Guid IdStorage)
        {
            try
            {
                var q = db.Archive.Where(x => !db.ArchiveStorage.Any(y => x.IdArchive == y.IdArchive && y.IdStorage == IdStorage));
                BindingList<DocumentArchive> list = new BindingList<DocumentArchive>();
                foreach (var item in q)
                {
                    //list.Add(item.TryToConvertTo<DocumentArchive>(true));
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentArchiveStorage GetArchiveStorage(Guid IdArchive, Guid IdStorage)
        {
            try
            {
                BindingList<DocumentArchiveStorage> list = new BindingList<DocumentArchiveStorage>();
                var query = db.ArchiveStorage.Where(x => x.IdArchive == IdArchive && x.IdStorage == IdStorage).FirstOrDefault();
                return query.Convert();// TryToConvertTo<DocumentArchiveStorage>(true);
            }
            finally
            {
                Dispose();
            }
        }

        public ArchiveStatistics GetArchiveStatistics(Guid idArchive)
        {
            var query = db.Archive
                .Include(x => x.Preservation)
                .Include(x => x.Document)
                .Select(x => new
                {
                    IdArchive = x.IdArchive,
                    DocumentsCount = x.Document.Where(z => z.IdParentBiblos.HasValue).Count(),
                    DocumentsVolume = x.Document.Where(z => z.IdParentBiblos.HasValue).Sum(y => y.Size),
                    PreservationsCount = x.Preservation.Count(y => y.CloseDate.HasValue),
                    ForwardedDevicesCount = 0
                })
                .Where(x => x.IdArchive == idArchive)
                .Single();

            return new ArchiveStatistics
            {
                DocumentsCount = query.DocumentsCount,
                DocumentsVolume = query.DocumentsVolume.HasValue ? query.DocumentsVolume.Value : 0L,
                ForwardedDevicesCount = query.ForwardedDevicesCount,
                PreservationsCount = query.PreservationsCount,
            };
        }        
        #endregion

        #region Server
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BindingList<Server> GetServers()
        {
            var retval = new BindingList<Server>();
            try
            {
                db.Server
                    .ToList()
                    .ForEach(x => retval.Add(x.Convert()));

                return retval;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public Server GetServer(Guid serverId)
        {
            try
            {
                Server retval = null;

                var model = db.Server
                    .SingleOrDefault(x => x.IdServer == serverId);

                if (model != null)
                    retval = model.Convert();

                return retval;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public Server GetServerByName(string serverName)
        {
            try
            {
                Server retval = null;

                var model = db.Server
                    .SingleOrDefault(x => serverName.Equals(x.ServerName, StringComparison.InvariantCultureIgnoreCase));

                if (model != null)
                    retval = model.Convert();

                return retval;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public Server UpdateServer(Server server)
        {
            try
            {
                if (server == null || server.IdServer == Guid.Empty)
                    return null;

                var model = db.Server
                    .SingleOrDefault(x => x.IdServer == server.IdServer);

                if (model != null)
                {
                    model.ServerName = server.ServerName;
                    model.ServerRole = server.ServerRole.ToString();
                }

                if (requireSave)
                    SaveChanges();

                return server;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public Server AddServer(Server server)
        {
            try
            {
                if (server == null || string.IsNullOrWhiteSpace(server.ServerName))
                    return null;

                server.IdServer = Guid.NewGuid();

                var model = new Model.Server
                {
                    IdServer = server.IdServer,
                    ServerName = server.ServerName,
                    ServerRole = server.ServerRole.ToString(),
                };

                db.Server.AddObject(model);

                if (requireSave)
                    SaveChanges();

                return server;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public bool DeleteServer(Guid serverId)
        {
            try
            {
                var toDelete = db.Server
                    .SingleOrDefault(x => x.IdServer == serverId);

                if (toDelete == null)
                    return false;

                db.Server.DeleteObject(toDelete);

                if (requireSave)
                    db.SaveChanges();

                return true;
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public ArchiveServerConfig AddArchiveServerConfig(ArchiveServerConfig config)
        {
            if (config == null || config.Server == null || config.Archive == null)
                return null;

            Model.ArchiveServerConfig toAdd = null;

            try
            {
                toAdd = new Model.ArchiveServerConfig
                {
                    IdArchiveServerConfig = Guid.NewGuid(),
                    IdServer = config.Server.IdServer,
                    IdArchive = config.Archive.IdArchive,
                    PathTransito = config.TransitPath,
                    TransitoEnabled = config.TransitEnabled,
                };

                db.ArchiveServerConfig.AddObject(toAdd);

                if (requireSave)
                {
                    db.SaveChanges();

                    toAdd = db.ArchiveServerConfig
                        .Include(x => x.Archive)
                        .Include(x => x.Server)
                        .Single(x => x.IdArchiveServerConfig == toAdd.IdArchiveServerConfig);
                }
                else
                {
                    return config;
                }
            }
            finally
            {
                Dispose();
            }

            return toAdd.Convert();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void DeleteArchiveServerConfig(ArchiveServerConfig config)
        {
            if (config == null || config.IdArchiveServerConfig == Guid.Empty)
                return;

            try
            {
                var toDelete = db.ArchiveServerConfig
                    .SingleOrDefault(x => x.IdArchiveServerConfig == config.IdArchiveServerConfig);

                if (toDelete != null)
                    db.ArchiveServerConfig.DeleteObject(toDelete);

                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        #endregion


        #region StorageType


        public BindingList<DocumentStorageType> GetStoragesType()
        {
            try
            {
                BindingList<DocumentStorageType> list = new BindingList<DocumentStorageType>();
                var query = db.StorageType;
                foreach (var item in query)
                {
                    //list.Add(item.TryToConvertTo<DocumentStorageType>(true));
                    list.Add(item.Convert());
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        public DocumentStorageType GetStorageTypeByStorage(Guid idStorage)
        {
            try
            {
                Model.StorageType storageType = db.StorageType.SingleOrDefault(x => x.Storage.Any(xx => xx.IdStorage == idStorage));
                if (storageType != null)
                {
                    return storageType.Convert();
                }
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        #endregion


        #region Permission


        public void AddPermission(DocumentPermission Permission, Guid IdDocument)
        {
            try
            {
                Model.Permission entity = new Model.Permission
                {
                    PermissionName = Permission.Name,
                    IdDocument = IdDocument,
                    IdMode = (short)Permission.Mode,
                    IsGroup = Permission.IsGroup ? (short)1 : (short)0
                };
                db.AddToPermission(entity);
                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }


        public BindingList<DocumentPermission> GetDocumentPermissions(Guid IdDocument)
        {
            try
            {
                BindingList<DocumentPermission> list = new BindingList<DocumentPermission>();
                var query = db.Permission.Where(x => x.Document.IdDocument == IdDocument);
                DocumentPermission permission = null;
                foreach (var item in query)
                {
                    permission = item.TryToConvertTo<DocumentPermission>(true);
                    permission.Mode = (DocumentPermissionMode)item.IdMode;
                    list.Add(permission);
                }
                return list;
            }
            finally
            {
                Dispose();
            }
        }

        #endregion

        #region Transaction

        //private List<IDbTransaction> transactions;

        /// <summary>
        /// Crea una nuova transazione DB sfruttando la proprietà "Connection" della classe concreta "BiblosDS2010Entities".
        /// <remarks>N.B.: Invocare questo metodo PRIMA di qualsivoglia query dml (come le SELECT) o ddl (come le INSERT)!
        /// Questo perche', data l'implementazione attuale della gestione delle transazioni DB, il manager dell'entity framework
        /// potrebbe NON riconoscere gli oggetti da inserire, modificare, ecc. come fossero appartenenti al medesimo contesto.</remarks>
        /// <example>
        /// Chiamata errata:
        ///     <code>
        ///         void Example()
        ///         {
        ///             ExampleObject toDelete = this.db.ExampleObjects.FirstOrDefault();
        ///             
        ///             this.db.BeginTransaction();
        ///             
        ///             this.db.ExampleObjects.DeleteObject(toDelete);
        ///                 
        ///             this.db.SaveChanges();
        ///             
        ///             this.db.CommitTransaction();
        ///         }
        ///     </code>
        ///     
        /// Chiamata corretta:
        ///     <code>
        ///         void Example()
        ///         {
        ///             this.db.BeginTransaction();
        ///             
        ///             try
        ///             {
        ///             
        ///                 ExampleObject toDelete = this.db.ExampleObjects.FirstOrDefault();
        ///                 
        ///                 this.db.ExampleObjects.DeleteObject(toDelete);
        ///                 
        ///                 this.db.SaveChanges();
        ///             
        ///                 this.db.CommitTransaction();
        ///             }
        ///             catch(Exception exx)
        ///             {
        ///                 this.db.RollbackTransaction();
        ///                 
        ///                 throw exx;
        ///             }
        ///         }
        ///     </code>
        /// </example>
        /// </summary>
        /// <returns></returns>
        //public IDbTransaction BeginTransaction()
        //{
        //    IDbTransaction retval = null;

        //    if (this.transactions == null)
        //        this.transactions = new List<IDbTransaction>();

        //    if (this._db != null)
        //    {
        //        this.RollbackTransaction();
        //    }

        //    this._db = new Model.BiblosDS2010Entities(BiblosDSConnectionString);

        //    this._db.Connection.Open();

        //    this.transactions.Clear();

        //    retval = this._db.Connection.BeginTransaction();

        //    this.transactions.Add(retval);

        //    return retval;
        //}

        //public void RollbackTransaction()
        //{
        //    if (this._db == null || this.transactions == null)
        //        throw new Exception("Nessuna transazione inizializzata. Inizializzare una transazione con BeginTransaction();");

        //    foreach (var tran in this.transactions)
        //    {
        //        try { tran.Rollback(); }
        //        catch { }
        //    }

        //    this.transactions.Clear();

        //    this._db.Dispose();
        //    this._db = null;
        //}

        public DbTransaction BeginNoSave()
        {
            this.requireDispose = false;
            this.requireSave = false;
            db.Connection.Open();
            return db.Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }

        public void BeginNoSaveNoTransaction()
        {
            this.requireDispose = false;
            this.requireSave = false;
        }

        public int SaveChanges()
        {
            return _db.SaveChanges();
        }

        //public void CommitTransaction()
        //{
        //    if (this._db == null || this.transactions == null)
        //        throw new Exception("Nessuna transazione inizializzata. Inizializzare una transazione con BeginTransaction();");
        //    try
        //    {
        //        //using (var tr = _db.Connection.BeginTransaction())
        //        //{
        //        //    try
        //        //    {

        //        //_db.SaveChanges();

        //        foreach (var tran in this.transactions)
        //        {
        //            try { tran.Commit(); }
        //            catch { }
        //        }

        //        this.transactions.Clear();

        //        //        tr.Commit();
        //        //    }
        //        //    catch (Exception)
        //        //    {
        //        //        tr.Rollback();
        //        //        throw;
        //        //    }                    
        //        //}
        //    }
        //    finally
        //    {
        //        this._db.Dispose();
        //        this._db = null;
        //    }
        //}

        #endregion


        #region CompiledQuery

        internal static readonly Func<Model.BiblosDS2010Entities,
                Guid,
                IQueryable<Model.Document>> GetChainDocumentsByGuidQuery =
                DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
                Guid,
                IQueryable<Model.Document>>(
       (db, IdParentDocument) => (from s in db.Document
                                      .Include("PreservationDocuments")
                                      .Include("DocumentParent")
                                      .Include("DocumentParentVersion")
                                      .Include("StorageArea")
                                      .Include("StorageArea.Storage")
                                      .Include("Storage")
                                      .Include("Archive")
                                  where s.DocumentParent.IdDocument == IdParentDocument
                                  && (!s.IsDetached.HasValue || !s.IsDetached.Value)
                                  select s));

        internal static readonly Func<Model.BiblosDS2010Entities, Guid, IQueryable<Model.Document>> GetDocumentChildrenQuery =
            DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities, Guid, IQueryable<Model.Document>>(
                (db, IdParentDocument) => (from s in db.Document
                                               .Include("PreservationDocuments")
                                               .Include("DocumentParent")
                                               .Include("DocumentParentVersion")
                                               .Include("StorageArea")
                                               .Include("StorageArea.Storage")
                                               .Include("Storage")
                                               .Include("Archive")
                                               .Include("AttributesValue")
                                               .Include("AttributesValue.Attributes")
                                           where s.DocumentParent.IdDocument == IdParentDocument
                                               && (s.DocumentParentVersion == null || s.DocumentParentVersion.IdDocument != IdParentDocument)
                                               && (!s.IsDetached.HasValue || !s.IsDetached.Value)
                                               && s.IsLatestVersion
                                           select s));

        internal static readonly Func<Model.BiblosDS2010Entities,
               int, Guid,
               IQueryable<Model.Document>> GetDocumentByIdQuery =
               DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
               int, Guid,
               IQueryable<Model.Document>>(
      (db, IdBiblos, IdArchive) => (from s in db.Document
                                       .Include("PreservationDocuments")
                                       .Include("DocumentStatus")
                                       .Include("DocumentParent")
                                       .Include("DocumentParentVersion")
                                       .Include("DocumentLink")
                                       .Include("StorageArea")
                                       .Include("StorageArea.Storage")
                                       .Include("Storage")
                                       .Include("Storage.StorageType")
                                       .Include("Archive")
                                    where s.IdBiblos == IdBiblos
                                    && s.DocumentParent == null
                                    && s.Archive.IdArchive == IdArchive
                                    select s));

        internal static readonly Func<Model.BiblosDS2010Entities,
              int, Guid,
              IQueryable<Model.Document>> GetDocumentChildByIdQuery =
              DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
              int, Guid,
              IQueryable<Model.Document>>(
     (db, IdBiblos, IdArchive) => (from s in db.Document
                                      .Include("PreservationDocuments")
                                      .Include("DocumentStatus")
                                      .Include("DocumentParent")
                                      .Include("DocumentParentVersion")
                                      .Include("DocumentLink")
                                      .Include("StorageArea")
                                      .Include("StorageArea.Storage")
                                      .Include("Storage")
                                      .Include("Storage.StorageType")
                                      .Include("Archive")
                                   where s.IdBiblos == IdBiblos
                                   && s.Archive.IdArchive == IdArchive
                                   select s));

        internal static readonly Func<Model.BiblosDS2010Entities,
               int, Guid,
               IQueryable<Model.Document>> GetDocumentDirectByIdQuery =
               DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
               int, Guid,
               IQueryable<Model.Document>>(
      (db, IdBiblos, IdArchive) => (from s in db.Document
                                       .Include("PreservationDocuments")
                                       .Include("DocumentStatus")
                                       .Include("DocumentParent")
                                       .Include("DocumentParentVersion")
                                       .Include("DocumentLink")
                                       .Include("StorageArea")
                                       .Include("StorageArea.Storage")
                                       .Include("Storage")
                                       .Include("Storage.StorageType")
                                       .Include("Archive")
                                    where s.IdBiblos == IdBiblos
                                    && s.Archive.IdArchive == IdArchive
                                    select s));

        internal static readonly Func<Model.BiblosDS2010Entities,
               Guid,
               IQueryable<Model.Document>> GetDocumentByGuidQuery =
               DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
               Guid,
               IQueryable<Model.Document>>(
      (db, IdDocument) => (from s in db.Document
                                        .Include("PreservationDocuments")
                                        .Include("DocumentStatus")
                                        .Include("DocumentParent")
                                        .Include("DocumentParentVersion")
                                        .Include("DocumentLink")
                                        .Include("StorageArea")
                                        .Include("StorageArea.Storage")
                                        .Include("Storage")
                                        .Include("Storage.StorageType")
                                        .Include("Archive")
                                        .Include("CertificateStore")
                                        .Include("PreservationDocuments.Preservation")
                           where s.IdDocument == IdDocument
                           && (!s.IsDetached.HasValue || !s.IsDetached.Value)
                           orderby s.Version descending // ordino per ultima versione
                           select s));

        internal static readonly Func<Model.BiblosDS2010Entities,
            Guid,
            decimal?,
            IQueryable<Model.Document>> GetDocumentByGuidAndVersionQuery =
            DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
            Guid,
            decimal?,
            IQueryable<Model.Document>>(
     (db, IdDocument, VersionNumber) => (from s in db.Document
                                                     .Include("PreservationDocuments")
                                                     .Include("DocumentStatus")
                                                     .Include("DocumentParent")
                                                     .Include("DocumentParentVersion")
                                                     .Include("DocumentLink")
                                                     .Include("StorageArea")
                                                     .Include("StorageArea.Storage")
                                                     .Include("Storage")
                                                     .Include("Storage.StorageType")
                                                     .Include("Archive")
                                                     .Include("CertificateStore")
                                                     .Include("PreservationDocuments.Preservation")
                                         where s.IdDocument == IdDocument
                                         && (VersionNumber == null || s.Version == VersionNumber)
                                         && (!s.IsDetached.HasValue || !s.IsDetached.Value)
                                         select s));

        internal static readonly Func<Model.BiblosDS2010Entities, Guid, decimal> GetDocumentVersionQuery =
            DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities, Guid, decimal>(
     (db, IdDocument) => (from s in db.Document
                          where s.IdDocument == IdDocument
                          select s.Version).Max());

        internal static readonly Func<Model.BiblosDS2010Entities,
              Guid,
              IQueryable<Model.Document>> GetDocumentLatestVersionByGuidQuery =
              DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
              Guid,
              IQueryable<Model.Document>>(
     (db, IdDocument) => (from s in db.Document
                                       .Include("PreservationDocuments")
                                       .Include("DocumentStatus")
                                       .Include("DocumentParent")
                                       .Include("DocumentParentVersion")
                                       .Include("DocumentLink")
                                       .Include("StorageArea")
                                       .Include("StorageArea.Storage")
                                       .Include("Storage")
                                       .Include("Storage.StorageType")
                                       .Include("Archive")
                                       .Include("CertificateStore")
                                       .Include("PreservationDocuments.Preservation")
                                       .Include("AttributesValue")
                                       .Include("AttributesValue.Attributes")
                          where (s.IdDocument == IdDocument || s.IdParentVersion == IdDocument) && s.IsLatestVersion
                          && (!s.IsDetached.HasValue || !s.IsDetached.Value)
                          select s));

        internal static readonly Func<Model.BiblosDS2010Entities,
               Guid,
               IQueryable<Model.StorageArea>> GetStorageAreaActiveByStorageQuery =
               DataObjects.CompiledQuery.Compile<Model.BiblosDS2010Entities,
               Guid,
               IQueryable<Model.StorageArea>>(
      (db, IdStorage) => (from s in db.StorageArea
                          where s.Storage.IdStorage == IdStorage
                          && s.Enable == 1
                          orderby s.Priority
                          select s));



        #endregion

        public BindingList<Document> SearchByAttributeCondition(BindingList<DocumentCondition> conditions, out int documentsInArchiveCount, int? skip = null, int? take = null, Guid? idArchive = null)
        {
            return this.SearchByAttributeCondition(conditions, out documentsInArchiveCount, skip, take, null, idArchive);
        }

        public BindingList<Document> SearchByAttributeCondition(BindingList<DocumentCondition> conditions, out int documentsInArchiveCount, int? skip = null, int? take = null, bool? thumbnail = null, Guid? idArchive = null, bool? latestVersion = false)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    BindingList<Document> documents = new BindingList<Document>();
                    IQueryable<Model.Document> query = db.Document.Include(x => x.Archive)
                        .Include(x => x.PreservationDocuments)
                        .Include(x => x.AttributesValue.First())
                        .Include(x => x.AttributesValue.First().Attributes);

                    if (idArchive.HasValue)
                    {
                        query = query.Where(x => x.Archive.IdArchive == idArchive.Value);
                    }

                    foreach (var item in conditions)
                    {
                        if (item.Value != null)
                        {
                            switch (item.Value.GetTypeName())
                            {
                                case "System.Int64":
                                    var intVal = item.Value.TryConvert<Int64>();
                                    query = query.Where(x => x.AttributesValue.Any(a => a.ValueInt == intVal));
                                    break;
                                case "System.Double":
                                    var floatVal = item.Value.TryConvert<double>();
                                    query = query.Where(x => x.AttributesValue.Any(a => a.ValueFloat == floatVal));
                                    break;
                                case "System.DateTime":
                                    var dateVal = item.Value.TryConvert<DateTime>();
                                    query = query.Where(x => x.AttributesValue.Any(a => a.ValueDateTime == dateVal));
                                    break;
                                default:
                                    var strVal = item.Value.TryConvert<string>();
                                    if (item.Operator == DocumentConditionFilterOperator.Contains)
                                    {
                                        query = query.Where(x => x.AttributesValue.Any(a => a.ValueString.Contains(strVal)));
                                    }
                                    else
                                    {
                                        query = query.Where(x => x.AttributesValue.Any(a => a.ValueString == strVal));
                                    }
                                    break;
                            }
                        }
                    }

                    if (latestVersion.GetValueOrDefault(false))
                        query = query.Where(x => x.IsLatestVersion);

                    if (skip.HasValue && take.HasValue)
                        query = query.OrderBy(x => x.DateCreated).Skip(skip.Value).Take(take.Value);
                    else if (skip.HasValue)
                        query = query.OrderBy(x => x.DateCreated).Skip(skip.Value);
                    else if (take.HasValue)
                        query = query.Take(take.Value);

                    query.ToList().ForEach(f => documents.Add(f.Convert(3)));
                    documentsInArchiveCount = query.Count();
                    transactionScope.Complete();
                    return documents;
                }
            }
            finally
            {
                Dispose();
            }
        }

        public BindingList<Document> SearchByAttributeConditionExt(Guid idArchive, BindingList<DocumentCondition> conditions, Func<IQueryable<Model.Document>, object, IQueryable<Model.Document>> queryExt, object pars, out int documentsInArchiveCount, int? skip, int? take, bool? thumbnail)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    BindingList<Document> documents = new BindingList<Document>();
                    IQueryable<Model.Document> query = db.Document.Include(x => x.Archive)
                        .Include(x => x.PreservationDocuments)
                        .Include(x => x.AttributesValue.First())
                        .Include(x => x.AttributesValue.First().Attributes);

                    query = query.Where(x => x.IsVisible == 1
                                             && x.IdArchive == idArchive
                                             && x.IsLatestVersion);

                    if (conditions.Count > 0)
                    {
                        BindingList<DocumentAttribute> attributes = GetAttributes(idArchive);
                        foreach (DocumentCondition condition in conditions)
                        {
                            DocumentAttribute documentAttribute = attributes.FirstOrDefault(x => x.Name == condition.Name);
                            if (documentAttribute != null)
                            {
                                switch (documentAttribute.AttributeType)
                                {
                                    case "System.Int64":
                                        long intVal = condition.Value.TryConvert<long>();
                                        query = query.Where(x => x.AttributesValue.Any(a => a.IdAttribute == documentAttribute.IdAttribute && a.ValueInt == intVal));
                                        break;
                                    case "System.Double":
                                        double doubleVal = condition.Value.TryConvert<double>();
                                        query = query.Where(x => x.AttributesValue.Any(a => a.IdAttribute == documentAttribute.IdAttribute && a.ValueFloat == doubleVal));
                                        break;
                                    case "System.DateTime":
                                        DateTime dateVal = condition.Value.TryConvert<DateTime>(SqlDateTime.MinValue.Value);
                                        query = query.Where(x => x.AttributesValue.Any(a => a.IdAttribute == documentAttribute.IdAttribute && a.ValueDateTime == dateVal));
                                        break;
                                    default:
                                        string stringVal = condition.Value.TryConvert<string>();
                                        if (condition.Operator == DocumentConditionFilterOperator.Contains)
                                            query = query.Where(x => x.AttributesValue.Any(a => a.IdAttribute == documentAttribute.IdAttribute && a.ValueString.Contains(stringVal)));
                                        else
                                            query = query.Where(x => x.AttributesValue.Any(a => a.IdAttribute == documentAttribute.IdAttribute && a.ValueString == stringVal));
                                        break;
                                }
                            }
                        }
                    }

                    if (queryExt != null)
                        query = queryExt(query, pars);

                    IQueryable<Model.Document> pagingQuery = query;
                    if (skip.HasValue || take.HasValue)
                        pagingQuery = pagingQuery.OrderBy(x => x.DateCreated);

                    if (skip.HasValue)
                        pagingQuery = pagingQuery.Skip(skip.Value);

                    if (take.HasValue)
                        pagingQuery = pagingQuery.Take(take.Value);

                    pagingQuery.ToList().ForEach(f => documents.Add(f.Convert(3)));
                    documentsInArchiveCount = query.Count();
                    transactionScope.Complete();
                    return documents;
                }
            }
            finally
            {
                Dispose();
            }
        }


        private object GetAttributeValue(string attributeType, string ValueString, long? ValueInt, double? ValueFloat, DateTime? ValueDateTime)
        {
            switch (attributeType)
            {
                case "System.Int64":
                    return ValueInt;
                case "System.Double":
                    return ValueFloat;
                case "System.DateTime":
                    return ValueDateTime;
                default:
                    return ValueString;
            }
        }

        private void CreateFilter(DocumentCondition condition, string logicalCondition, ref string query)
        {
            query += logicalCondition + "( ";
            if (condition.DocumentAttributeConditions != null && condition.DocumentAttributeConditions.Count() > 0)
            {
                for (int i = 0; i < condition.DocumentAttributeConditions.Count(); i++)
                {
                    CreateFilter(condition.DocumentAttributeConditions[i], i == 0 ? string.Empty : Condition(condition.DocumentAttributeConditions[i - 1].Condition), ref query);
                }

            }
            else
            {
                query += string.Format(@"d.IdDocument in ( select Value a.IdDocument from BiblosDS2010Entities.AttributesValue as a where
                    ( {0} ", Operator<string>(condition.Operator, condition.Value, "a.ValueString "));
                if (condition.Value.TryConvert<Int64>(-1) != -1)
                {
                    query += string.Format(@" or {1} ", condition.Name, Operator<Int64>(condition.Operator, condition.Value, "a.ValueInt "));
                }
                if (condition.Value.TryConvert<Double>(-1) != -1)
                {
                    query += string.Format(@" or {1} ", condition.Name, Operator<Int64>(condition.Operator, condition.Value, "a.ValueFloat"));
                }
                if (condition.Value.TryConvert<DateTime>(DateTime.MinValue) != DateTime.MinValue)
                {
                    query += string.Format(@" or a.ValueDateTime {1} ", condition.Name, Operator<DateTime>(condition.Operator, condition.Value, "a.ValueDateTime"));
                }
                query += string.Format(") and a.Attributes.Name = '{0}' )", condition.Name);
            }
            query += " )";
        }


        protected string Condition(DocumentConditionFilterCondition op)
        {
            switch (op)
            {
                case DocumentConditionFilterCondition.Or:
                    return " Or ";
                default:
                case DocumentConditionFilterCondition.And:
                    return " And ";
            }
        }

        protected string Operator<T>(DocumentConditionFilterOperator op, object value, string attributeColumn)
        {
            switch (op)
            {
                case DocumentConditionFilterOperator.IsEqualTo:
                    if (typeof(T) == typeof(DateTime))
                        return string.Format("{0} = DATETIME'{1:yyyy-MM-dd 00:00}' ", attributeColumn, value.TryConvert<DateTime>());
                    else
                        return string.Format("{0} = {1} ", attributeColumn, value);
                case DocumentConditionFilterOperator.Contains:
                    if (typeof(T) == typeof(Int64) || typeof(T) == typeof(Double))
                        return string.Format("{0} = {1} ", attributeColumn, value);
                    else if (typeof(T) == typeof(DateTime))
                        return string.Format("{0} = DATETIME'{1:yyyy-MM-dd 00:00}' ", attributeColumn, value.TryConvert<DateTime>());
                    else
                        return string.Format("{0} Like '%{1}%'", attributeColumn, value);
                case DocumentConditionFilterOperator.IsNullOrEmpty:
                    return string.Format(" {0} IS NULL or {0} = '' ", attributeColumn);
                default:
                    throw new NotImplementedException();
            }
        }

        private string LogQuery(ObjectQuery query)
        {
            if (query == null)
                return string.Empty; ;
            try
            {
                string str = string.Join(" ", query.Parameters.Select(x => "declare @" + x.Name + " as " + x.ParameterType + " = '" + x.Value + "'"));
                str += " " + query.ToTraceString();
                Journaling.WriteJournaling(LoggingSource.BiblosDS_CommonLibrary, "", "LogQuery", str, LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, Guid.Empty, Guid.Empty, Guid.Empty);
                return str;
            }
            catch { /*Ignore*/ }
            return "";
        }

        internal void SetDocumentPdf(Guid idDocument, string pdfName)
        {
            try
            {
                var doc = this.db.Document.Where(x => x.IdDocument == idDocument).Single();
                doc.IdPdf = pdfName;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal void SetDocumentThumbnail(Guid idDocument, string thumbnailName)
        {
            try
            {
                var doc = this.db.Document.Where(x => x.IdDocument == idDocument).Single();
                doc.IdPdf = thumbnailName;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal void DeleteCache(DocumentCache cache, Guid idDocument)
        {
            try
            {
                var dbCache = this.db.DocumentCache.Where(x => x.IdDocument == idDocument && x.ServerName == cache.ServerName && x.FileName == cache.FileName && x.Signature == cache.Signature).SingleOrDefault();
                if (dbCache != null)
                    this.db.DeleteObject(dbCache);
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal void AddCache(DocumentCache cache, Guid idDocument, float size)
        {
            try
            {
                Model.DocumentCache dbCache = null;
                if ((dbCache = this.db.DocumentCache.Where(x => x.IdDocument == idDocument && x.ServerName == cache.ServerName && x.FileName == cache.FileName).FirstOrDefault()) != null)
                    this.db.DocumentCache.DeleteObject(dbCache);
                this.db.DocumentCache.AddObject(new Model.DocumentCache
                {
                    IdDocument = idDocument,
                    FileName = cache.FileName,
                    ServerName = cache.ServerName,
                    Signature = cache.Signature,
                    DateCreated = DateTime.Now,
                    Size = size
                }
                );
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal BindingList<DocumentCache> GetDocumentCache(Guid idDocument, string serverName)
        {
            try
            {
                return this.db.DocumentCache.Where(x => x.IdDocument == idDocument && x.ServerName == serverName).Convert();
            }
            finally
            {
                Dispose();
            }
        }

        internal void UpdateDocumentName(Document item)
        {
            try
            {
                var doc = this.db.Document.Where(x => x.IdDocument == item.IdDocument).Single();
                doc.Name = item.Name;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal double? GetCahceSize(string serverName)
        {
            try
            {
                return this.db.DocumentCache.Where(x => x.ServerName == serverName).Select(x => x.Size).Sum();
            }
            finally
            {
                Dispose();
            }
        }

        internal BindingList<DocumentCache> GetDocumentCache(string serverName, int take, int skip)
        {
            return this.db.DocumentCache.Where(x => x.ServerName == serverName).OrderByDescending(x => x.DateCreated).Skip(skip).Take(take).Convert();
        }

        internal void UpdateDocumentSignHeader(Guid idDocument, string signHeader)
        {
            try
            {
                var doc = this.db.Document.Where(x => x.IdDocument == idDocument).Single();
                doc.SignHeader = signHeader;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public Document GetPreservedDocuments(Guid idPreservation, string name)
        {
            try
            {
                var doc = db.Document.Include(x => x.PreservationDocuments).Where(x => x.PreservationDocuments.Any(pd => pd.IdPreservation == idPreservation) && x.PrimaryKeyValue != null).ToList().Where(x => IsSameDbPreservationName(x.PrimaryKeyValue, name)).SingleOrDefault();
                return doc.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        private bool IsSameDbPreservationName(string uniqueKey, string name)
        {
            var chiaveU = uniqueKey.ToCharArray();
            for (var i = 0; i < uniqueKey.Length; i++)
                if (!(chiaveU[i] >= '0' && chiaveU[i] <= '9') &&
                    !(chiaveU[i] >= 'a' && chiaveU[i] <= 'z') &&
                    !(chiaveU[i] >= 'A' && chiaveU[i] <= 'Z'))
                    chiaveU[i] = '-';
            var cu = new string(chiaveU);
            return name.Contains(cu);
        }

        public void UpdatePrimaryKey(Guid idDocument, string calculatedPrimaryKey, DateTime? dateMain)
        {
            try
            {
                var doc = this.db.Document.Where(x => x.IdDocument == idDocument).Single();
                doc.PrimaryKeyValue = calculatedPrimaryKey;
                doc.DateMain = dateMain;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateDateMain(Guid idDocument, DateTime? dateMain)
        {
            try
            {
                var doc = this.db.Document.Where(x => x.IdDocument == idDocument).Single();
                doc.DateMain = dateMain;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        public DateTime? GetLastPreservedDate(Guid idArchive, Guid preservationId = new Guid())
        {
            try
            {
                return this.db.Preservation.Where(x => x.IdPreservation != preservationId && x.IdArchive == idArchive && x.LockOnDocumentInsert.HasValue && x.LockOnDocumentInsert.Value).Select(x => x.EndDate).Max();
            }
            finally
            {
                Dispose();
            }
        }

        internal void UpdateDocumentAttributeValue(Document document, BindingList<DocumentAttributeValue> attributes)
        {
            try
            {
                foreach (var attribute in attributes)
                {
                    var attr = this.db.AttributesValue.Where(x => x.IdDocument == document.IdDocument && x.IdAttribute == attribute.IdAttribute).SingleOrDefault();
                    if (attr == null)
                    {
                        attr = new Model.AttributesValue { IdAttribute = attribute.Attribute.IdAttribute, IdDocument = document.IdDocument };
                        this.db.AddToAttributesValue(attr);
                    }
                    switch (attribute.Attribute.AttributeType)
                    {
                        case "System.String":
                            attr.ValueString = attribute.Value.TryConvert<string>();
                            break;
                        case "System.Int64":
                            attr.ValueInt = attribute.Value.TryConvert<Int64>();
                            break;
                        case "System.Double":
                            attr.ValueFloat = attribute.Value.TryConvert<double>();
                            break;
                        case "System.DateTime":
                            attr.ValueDateTime = attribute.Value.TryConvert<DateTime>();
                            break;
                        default:
                            throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato.", attribute.Attribute.AttributeType));
                    }
                }
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal void UpdateDocumentAttributeValue(Document document, DocumentAttributeValue attribute)
        {
            try
            {
                var attr = this.db.AttributesValue.Where(x => x.IdDocument == document.IdDocument && x.IdAttribute == attribute.IdAttribute).SingleOrDefault();
                if (attr == null)
                {
                    attr = new Model.AttributesValue { IdAttribute = attribute.Attribute.IdAttribute, IdDocument = document.IdDocument };
                    this.db.AddToAttributesValue(attr);
                }
                switch (attribute.Attribute.AttributeType)
                {
                    case "System.String":
                        attr.ValueString = attribute.Value.TryConvert<string>();
                        break;
                    case "System.Int64":
                        attr.ValueInt = attribute.Value.TryConvert<Int64>();
                        break;
                    case "System.Double":
                        attr.ValueFloat = attribute.Value.TryConvert<double>();
                        break;
                    case "System.DateTime":
                        attr.ValueDateTime = attribute.Value.TryConvert<DateTime>();
                        break;
                    default:
                        throw new Exceptions.Attribute_Exception(string.Format("Attributo di tipo {0} non supportato.", attribute.Attribute.AttributeType));
                }
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal DocumentAttach AddDocumentAttach(DocumentAttach attach, string transitoLocalPath)
        {
            try
            {
                if (attach == null)
                    throw new Exception("AddDocumentAttach : Attach is null.");

                if (attach.Document == null)
                    throw new Exceptions.DocumentNotFound_Exception("AddDocumentAttach : The document who owns the attachment is null.");

                if (attach.Document.DocumentParent == null)
                    throw new Exceptions.DocumentNotFound_Exception("AddDocumentAttach : The parent document of the document " + attach.Document.IdDocument + " is null.");

                var toAdd = new Model.DocumentAttach
                {
                    AttachOrder = this.db.DocumentAttach.Where(x => x.IdDocument == attach.Document.IdDocument).Count() + 1,
                    DateCreated = DateTime.Now,
                    IdDocument = attach.Document.IdDocument,
                    IdParentBiblos = attach.Document.DocumentParent.IdDocument,
                    IdDocumentAttach = attach.IdDocumentAttach,
                    IsConfirmed = (short)(attach.IsConfirmed.GetValueOrDefault(false) ? 1 : 0),
                    IsVisible = (short)(attach.IsVisible ? 1 : 0),
                    Name = attach.Name,
                    Size = attach.Size,
                };

                if (attach.Status != null)
                    toAdd.IdDocumentStatus = attach.Status.IdStatus;

                if (attach.Status.IdStatus == (int)DocumentStatus.InTransito)
                {
                    toAdd.DocumentAttachTransit = new Model.DocumentAttachTransit
                    {
                        LocalPath = transitoLocalPath,
                        Status = (int)DocumentTarnsitoStatus.DaProcessare,
                        DateCreated = DateTime.Now,
                    };
                }

                db.DocumentAttach.AddObject(toAdd);

                if (requireSave)
                    db.SaveChanges();

                var ret = toAdd.Convert();
                ret.Document = attach.Document;

                return ret;
            }
            finally
            {
                if (requireDispose)
                    Dispose();
            }
        }

        internal BindingList<DocumentAttach> GetDocumentAttachesFromDocument(Guid? idDocument)
        {
            var query = this.db.DocumentAttach
                .Include(x => x.Document)
                .Include(x => x.DocumentStatus)
                .Include(x => x.DocumentAttachTransit)
                .Include(x => x.Document.Archive)
                .Where(x => !x.IsVisible.HasValue || x.IsVisible.Value == 1).AsQueryable();

            var ret = new BindingList<DocumentAttach>();

            if (idDocument.HasValue)
                query = query.Where(x => x.IdDocument == idDocument.Value);

            foreach (var item in query)
            {
                ret.Add(item.Convert());
            }

            return ret;
        }

        internal DocumentAttach GetDocumentAttach(Guid idDocumentAttach)
        {
            if (idDocumentAttach == Guid.Empty)
                throw new Exceptions.DocumentNotFound_Exception("GetDocumentAttach : idDocumentAttach is null.");

            var ret = this.db.DocumentAttach
                .Include(x => x.Document)
                .Include(x => x.Document.Storage)
                .Include(x => x.Document.Archive)
                .Include(x => x.Document.StorageArea)
                .Include(x => x.DocumentStatus)
                .Where(x => x.IdDocumentAttach == idDocumentAttach)
                .SingleOrDefault();

            if (ret == null)
                return null;

            return ret.Convert();
        }

        internal DocumentAttach UpdateDocumentAttach(DocumentAttach attach)
        {
            try
            {
                if (attach == null || attach.IdDocumentAttach == Guid.Empty)
                    throw new Exceptions.DocumentNotFound_Exception("UpdateDocumentAttach : invalid attachment to update.");

                var entity = this.db.DocumentAttach
                    .Where(x => x.IdDocumentAttach == attach.IdDocumentAttach)
                    .SingleOrDefault();

                if (entity == null)
                    throw new Exceptions.DocumentNotFound_Exception("UpdateDocumentAttach : no attachment found for update.");

                entity.AttachOrder = attach.AttachOrder;
                entity.DateCreated = attach.DateCreated;
                entity.IdDocument = attach.IdDocument;

                if (attach.Document != null)
                {
                    if (attach.Document.Status != null)
                        entity.IdDocumentStatus = attach.Document.Status.IdStatus;
                    if (attach.Document.DocumentParent != null)
                        entity.IdParentBiblos = attach.Document.DocumentParent.IdDocument;
                }
                entity.IsConfirmed = attach.IsConfirmed.HasValue ? (short?)(attach.IsConfirmed.Value ? 1 : 0) : null;
                entity.IsVisible = (short)(attach.IsVisible ? 1 : 0);
                entity.Name = attach.Name;
                entity.Size = attach.Size;

                if (requireSave)
                    this.db.SaveChanges();

                var ret = entity.Convert();
                ret.Document = attach.Document;

                return ret;
            }
            finally
            {
                Dispose();
            }
        }

        internal DocumentAttachTransito TransitoAttachChangeState(Guid IdDocumentAttach, DocumentTarnsitoStatus Status)
        {
            try
            {
                Model.DocumentAttachTransit entityTransito = db.DocumentAttachTransit
                    .Include(x => x.DocumentAttach)
                    .Where(x => x.IdDocumentAttach == IdDocumentAttach)
                    .SingleOrDefault();
                if (entityTransito == null)
                    throw new Exceptions.DocumentNotFound_Exception("Allegato documento non in Transito.");
                //Accesso esclusivo in Transito da parte dello storage.
                if (Status == DocumentTarnsitoStatus.StorageProcessing && entityTransito.Status.HasValue && entityTransito.Status.Value == (short)DocumentTarnsitoStatus.StorageProcessing)
                    throw new Exceptions.StorageIsProcessingFile_Exception();
                //path = entityTransito.LocalPath;
                entityTransito.DateRetry = DateTime.Now;
                entityTransito.Status = (short)Status;
                if (Status == DocumentTarnsitoStatus.EndProcessing)
                {
                    db.DocumentAttachTransit.DeleteObject(entityTransito);
                }
                else
                {
                    entityTransito.Retry += 1;
                }

                if (requireSave)
                    db.SaveChanges();

                return entityTransito.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        internal BindingList<DocumentAttach> GetDocumentAttachesInTransito(int MaxRetry, bool waitToProcess)
        {
            try
            {
                var query = db.DocumentAttachTransit
                    .Include(x => x.DocumentAttach)
                    .Include(x => x.DocumentAttach.Document)
                    .Include(x => x.DocumentAttach.Document.DocumentStatus)
                    .Include(x => x.DocumentAttach.Document.StorageArea)
                    .Include(x => x.DocumentAttach.Document.StorageArea.Storage)
                    .Include(x => x.DocumentAttach.Document.Storage)
                    .Include(x => x.DocumentAttach.Document.Storage.StorageType)
                    .Include(x => x.DocumentAttach.Document.Archive)
                    .Include(x => x.DocumentAttach.Document.DocumentLink)
                    .Include(x => x.DocumentAttach.Document.DocumentParent)
                    .Include(x => x.DocumentAttach.Document.DocumentNodeType)
                    .Include(x => x.DocumentAttach.Document.PreservationDocuments)
                    .Where(x => (x.Retry <= MaxRetry || MaxRetry <= 0)
                        && (x.Status == null || x.Status.Value != (short)DocumentTarnsitoStatus.StorageProcessing))
                        .Select(x => x.DocumentAttach);

                var list = new BindingList<DocumentAttach>();

                foreach (var item in query)
                {
                    if (waitToProcess && (DateTime.Now - item.DateCreated.GetValueOrDefault()).TotalMinutes < 1)
                        continue;
                    if (item == null)
                        continue;

                    list.Add(item.Convert());
                }

                return list;
            }
            finally
            {
                Dispose();
            }
        }

        internal BindingList<Document> GetDocumentLink(Guid idDocument)
        {
            try
            {
                BindingList<Document> result = new BindingList<Document>();
                var query = db.DocumentConnection.Include(x => x.Document1).Include(x => x.Document1.AttributesValue).Include(x => x.Document1.AttributesValue.First().Attributes).Where(x => x.IdDocument == idDocument);
                foreach (var item in query)
                {
                    result.Add(item.Document1.Convert());
                }
                return result;
            }
            finally
            {
                Dispose();
            }
        }

        internal bool CheckConnectionExists(Guid IdDocument, Guid IdDocumentLink)
        {
            return this.db.DocumentConnection.Any(x => x.IdDocument == IdDocument && x.IdDocumentConnection == IdDocumentLink);
        }

        internal bool AddDocumentLink(Guid IdDocument, Guid IdDocumentLink)
        {
            var retval = true;

            try
            {
                //Controlla se i documenti che si vogliono collegare esistono.
                if (!this.db.Document.Any(x => x.IdDocument == IdDocument) || !this.db.Document.Any(x => x.IdDocument == IdDocumentLink))
                {
                    retval = false;
                }
                else
                {
                    this.db.AddToDocumentConnection(new Model.DocumentConnection { IdDocument = IdDocument, IdDocumentConnection = IdDocumentLink, DateCreated = DateTime.Now });
                    this.db.AddToDocumentConnection(new Model.DocumentConnection { IdDocument = IdDocumentLink, IdDocumentConnection = IdDocument, DateCreated = DateTime.Now });
                    if (requireSave)
                        this.db.SaveChanges();
                }
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        internal bool DeleteDocumentLink(Guid IdDocument, Guid IdDocumentLink)
        {
            try
            {
                var query = this.db.DocumentConnection.Where(x => (x.IdDocument == IdDocument && x.IdDocumentConnection == IdDocumentLink) || (x.IdDocument == IdDocumentLink && x.IdDocumentConnection == IdDocument));
                //Model.DocumentConnection item = null;
                bool ret = false;
                foreach (var item in query)
                {
                    ret = true;
                    this.db.DocumentConnection.DeleteObject(item);
                }
                if (requireSave)
                    this.db.SaveChanges();
                return ret;
            }
            finally
            {
                Dispose();
            }
        }

        internal void ConfirmDocument(Document document, string primaryKey)
        {
            try
            {
                var query = this.db.Document.Where(x => x.IdDocument == document.IdDocument).SingleOrDefault();
                if (query == null)
                    throw new Exceptions.DocumentNotFound_Exception();
                query.IsConfirmed = 1;
                query.PrimaryKeyValue = primaryKey;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal BindingList<string> GetDistinctAttributesValues(Guid idAttribute)
        {
            try
            {
                var query = this.db.AttributesValue.Where(x => x.IdAttribute == idAttribute).Select(x => x.ValueString).Distinct();
                return new BindingList<string>(query.ToList());
            }
            finally
            {
                Dispose();
            }
        }

        internal void UndoLatestVersion(Guid undoDocumentId, Guid originalDocumentId)
        {
            try
            {
                var undo = this.db.Document.Where(x => x.IdDocument == undoDocumentId).SingleOrDefault();
                var original = this.db.Document.Where(x => x.IdDocument == originalDocumentId).SingleOrDefault();
                if (undo == null || original == null)
                    throw new Exceptions.DocumentNotFound_Exception();
                undo.IsLatestVersion = false;
                original.IsLatestVersion = true;
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal void AddArchiveCertificate(Guid idArchive, string userName, string pin, string fileName, byte[] certificateBlob)
        {
            try
            {
                var archiveCert = this.db.ArchiveCertificate.Where(x => (!x.Enabled.HasValue || x.Enabled.Value) && x.IdArchive == idArchive);
                foreach (var item in archiveCert)
                {
                    item.Enabled = false;
                }
                this.db.ArchiveCertificate.AddObject(
                    new Model.ArchiveCertificate
                    {
                        IdArchive = idArchive,
                        CertificateFileName = fileName,
                        CertificateUserName = userName,
                        CertificatePin = pin,
                        CertificateBase64 = Convert.ToBase64String(certificateBlob),
                        Enabled = true,
                        IdArchiveCertificate = Guid.NewGuid()
                    }
                 );
                if (requireSave)
                    this.db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal DocumentArchiveCertificate GetArchiveCertificate(Guid idArchive)
        {
            try
            {
                var archiveCert = this.db.ArchiveCertificate.Where(x => (!x.Enabled.HasValue || x.Enabled.Value) && x.IdArchive == idArchive).FirstOrDefault();
                return archiveCert.Convert();
            }
            finally
            {
                Dispose();
            }
        }

        public bool LockTask(PreservationTask task)
        {
            try
            {
                var item = db.PreservationTask.Single(x => x.IdPreservationTask == task.IdPreservationTask);
                if (!item.LockDate.HasValue || (DateTime.Now - item.LockDate.Value).TotalDays > 1)
                {
                    item.LockDate = DateTime.Now;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UnlockTask(PreservationTask task)
        {
            try
            {
                var item = db.PreservationTask.Single(x => x.IdPreservationTask == task.IdPreservationTask);
                item.LockDate = null;
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Preservation> PreservationToClose()
        {
            try
            {
                List<Preservation> items = new List<Preservation>();
                string preservationCode = ((int)PreservationTaskTypes.Preservation).ToString();
                var presrvation = db.Preservation.Include(x => x.Archive).Where(x => x.PreservationTask1.Any(p => p.PreservationTaskType.KeyCode == preservationCode) && !x.CloseDate.HasValue);
                foreach (var item in presrvation)
                {
                    item.CloseContent = null;
                    items.Add(item.Convert());
                }
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddToFileTable(string storageName, string fileName, Document document, byte[] content, int documentType)
        {
            try
            {
                db.Storage_SP_InsertDocumentToStorage(storageName, fileName, content, document.StorageArea.Path, document.IdDocument, document.DocumentParent.IdDocument, documentType);
            }
            finally
            {
                Dispose();
            }
        }

        public void RemoveDocumentFromSQLStorage(string storageName, Document document)
        {
            try
            {
                db.Storage_SP_DeleteDocumentFromStorage(storageName, document.IdDocument);
            }
            finally
            {
                Dispose();
            }
        }

        public void RemoveSearchableDocumentFromStorage(string storageName, Document document)
        {
            try
            {
                db.Storage_SP_DeleteSearchableDocumentFromStorage(storageName, document.IdDocument);
            }
            finally
            {
                Dispose();
            }
        }

        public FileTableModel GetDocumentByNameFromSQLStorage(string storageName, Document document, string name)
        {
            try
            {
                Model.FileTableModel result = db.SQL2014Storage_SP_DocumentsByName(storageName, document.StorageArea.Path, name).FirstOrDefault();
                return result != null ? result.Convert() : null;
            }
            finally
            {
                Dispose();
            }
        }

        public FileTableModel GetDocumentByIdFromSQLStorage(string storageName, Document document, int documentType)
        {
            try
            {
                Model.FileTableModel result = db.SQL2014Storage_SP_DocumentById(storageName, document.IdDocument, documentType).FirstOrDefault();
                return result != null ? result.Convert() : null;
            }
            finally
            {
                Dispose();
            }
        }

        internal BindingList<Document> GetRemovableDetachedDocuments(Guid idArchive, DateTime? fromDate, DateTime? toDate)
        {
            BindingList<Document> results = new BindingList<Document>();
            IQueryable<Model.Document> query = db.Document.Include("Storage").Include("PreservationDocuments").Where(x => x.Archive.IdArchive == idArchive
                && x.IdParentBiblos.HasValue && (x.IdDocumentStatus != (short)DocumentStatus.RemovedFromStorage 
                                                    && x.IdDocumentStatus != (short)DocumentStatus.ProfileOnly && x.IdDocumentStatus != (short)DocumentStatus.Undefined)
                && x.IsDetached == true && x.IsConservated == 0 
                && !x.PreservationDocuments.Any() && x.IsLatestVersion == true);

            if (fromDate.HasValue && fromDate != DateTime.MinValue)
            {
                query = query.Where(x => x.DateCreated >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.DateCreated <= toDate);
            }

            query.ToList().ForEach(f => results.Add(f.Convert()));
            return results;
        }

        internal BindingList<Document> GetAllVersionedDocuments(Document document)
        {
            BindingList<Document> results = new BindingList<Document>();
            IQueryable<Model.Document> query = db.Document.Where(x => x.IdParentVersion == document.IdDocument).OrderByDescending(o => o.Version);
            query.ToList().ForEach(f => results.Add(f.Convert()));
            return results;
        }

        internal void DeleteDocumentDetached(Guid idDocument)
        {
            try
            {
                Model.Document toDeleteDocument = db.Document.Single(x => x.IdDocument == idDocument);
                toDeleteDocument.IdDocumentStatus = (short)DocumentStatus.RemovedFromStorage;
                toDeleteDocument.IsVisible = 0;
                toDeleteDocument.IsDetached = true;
                toDeleteDocument.PrimaryKeyValue = string.Empty;

                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }

        internal Document GetById(Guid idDocument)
        {
            IQueryable<Model.Document> query = db.Document.Include("DocumentStatus")
                       .Include("DocumentParent")
                       .Include("DocumentParentVersion")
                       .Include("DocumentLink")
                       .Include("StorageArea")
                       .Include("StorageArea.Storage")
                       .Include("Storage")
                       .Include("Storage.StorageType")
                       .Include("Archive")
                       .Include("CertificateStore")
                       .Include("PreservationDocuments")
                       .Include("PreservationDocuments.Preservation")
                       .Where(x => x.IdDocument == idDocument);
            Model.Document document = query.SingleOrDefault();
            return document?.Convert();
        }

        internal Document GetDocumentIgnoreState(Guid idDocument)
        {
            IQueryable<Model.Document> query = db.Document.Include("DocumentStatus")
                       .Include("DocumentParent")
                       .Include("DocumentParentVersion")
                       .Include("DocumentLink")
                       .Include("StorageArea")
                       .Include("StorageArea.Storage")
                       .Include("Storage")
                       .Include("Archive")
                       .Include("AttributesValue")
                       .Include("AttributesValue.Attributes")
                       .Include("PreservationDocuments")
                       .Where(x => (x.IdDocument == idDocument || x.IdParentVersion == idDocument) && x.IsLatestVersion);
            Model.Document document = query.SingleOrDefault();
            return document?.Convert();
        }

        internal BindingList<Document> GetDocumentChildrenIgnoreState(Guid idParent)
        {
            BindingList<Document> results = new BindingList<Document>();
            IQueryable<Model.Document> query = db.Document
                                               .Include("DocumentStatus")
                                               .Include("DocumentParent")
                                               .Include("DocumentParentVersion")
                                               .Include("StorageArea")
                                               .Include("StorageArea.Storage")
                                               .Include("Storage")
                                               .Include("Archive")
                                               .Include("AttributesValue")
                                               .Include("AttributesValue.Attributes")
                                               .Include("PreservationDocuments")
                                               .Where(s => s.DocumentParent.IdDocument == idParent
                                               && (s.DocumentParentVersion == null || s.DocumentParentVersion.IdDocument != idParent)
                                               && s.IsLatestVersion);
            query.ToList().ForEach(f => results.Add(f.Convert()));
            return results;
        }

        internal bool HasDocumentChildren(Guid idParent)
        {
            int foundItems = db.Document
                                  .Count(x => x.DocumentParent.IdDocument == idParent
                                    && (x.DocumentParentVersion == null || x.DocumentParentVersion.IdDocument != idParent)
                                    && (x.IdDocumentStatus == (short)DocumentStatus.InCache || x.IdDocumentStatus == (short)DocumentStatus.InStorage || x.IdDocumentStatus == (short)DocumentStatus.InTransito || x.IdDocumentStatus == (short)Enums.DocumentStatus.MovedToPreservation)
                                    && (!x.IsDetached.HasValue || !x.IsDetached.Value)
                                    && x.IsLatestVersion);
            return foundItems > 0;
        }

        internal Status GetDocumentStatus(Document document)
        {
            try
            {
                Model.DocumentStatus documentStatus = db.DocumentStatus.Where(x => x.Document.Any(xx => xx.IdDocument == document.IdDocument)).SingleOrDefault();
                if (documentStatus != null)
                {
                    return documentStatus.Convert();
                }
                return null;
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateDocumentPreservationReference(Guid idDocument, string preservationDocumentPath, string hash)
        {
            try
            {
                Model.PreservationDocuments preservationDocument = db.PreservationDocuments.SingleOrDefault(x => x.IdDocument == idDocument);
                if (preservationDocument != null)
                {
                    preservationDocument.Path = preservationDocumentPath;
                    preservationDocument.Hash = hash;

                    if (requireSave)
                        db.SaveChanges();
                }                
            }
            finally
            {
                Dispose();
            }
        }

        public void UpdateDocumentStatus(Document document, Status status)
        {
            try
            {
                Model.Document localDocument = db.Document.SingleOrDefault(x => x.IdDocument == document.IdDocument);
                if (localDocument != null)
                {
                    localDocument.IdDocumentStatus = status.IdStatus;
                }

                if (requireSave)
                    db.SaveChanges();
            }
            finally
            {
                Dispose();
            }
        }
    }
}
