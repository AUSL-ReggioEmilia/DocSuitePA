using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BiblosDS.Library.Common.Model;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using Document = BiblosDS.Library.Common.Objects.Document;
using BiblosDS.Library.Common.Utility;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace BiblosDS.Library.Common.DB
{
    public class EntityAdminProvider
    {
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

        #region Document

        public BindingList<Document> GetDocumentsInStorage(Guid IdStorage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                throw new NotImplementedException();
                //var query = from m in db.GetDocumentsByStorage(IdStorage)
                //            select new Document()
                //            {
                //                IdDocument = m.IdDocument,
                //                IdBiblos = m.IdBiblos,
                //                ParentBiblos = m.IdParentBiblos == null ? null : new Document((Guid)m.IdParentBiblos),
                //                StorageArea = m.IdStorageArea == null ? null :
                //                new DocumentStorageArea()
                //                {
                //                    IdStorageArea = (Guid)m.IdStorageArea,
                //                    Name = m.StorageAreaName,
                //                    Path = m.Path,
                //                    Status = new Status()
                //                    {
                //                        IdStatus = m.IdStorageStatus == null ? (short)-1 : (short)m.IdStorageStatus
                //                    },
                //                    Storage = new DocumentStorage()
                //                    {
                //                        IdStorage = (Guid)m.StorageAreaIdStorage
                //                    }
                //                },
                //                Storage = m.IdStorage == null ? null :
                //                new DocumentStorage((Guid)m.IdStorage, m.MainPath, m.StorageName, m.StorageRuleAssembly,
                //                    new DocumentStorageType((Guid)m.IdStorageType, m.StorageAssembly, m.StorageClassName)),
                //                Archive = new DocumentArchive()
                //                {
                //                    IdArchive = m.IdArchive,
                //                    Name = m.ArchiveName,
                //                    IsLegal = ShortToBool(m.IsLegal),
                //                    LowerCache = m.LowerCache,
                //                    UpperCache = m.UpperCache,
                //                    MaxCache = m.MaxCache,
                //                    EnableSecurity = ShortToBool(m.EnableSecurity)
                //                },
                //                ChainOrder = m.ChainOrder,
                //                Version = m.Version,
                //                DocumentLink = m.IdDocumentLink == null ? null : new Document((Guid)m.IdDocumentLink),
                //                Certificate = m.IdCertificate == null ? null : new DocumentCertificate((Guid)m.IdCertificate),
                //                SignHeader = m.SignHeader,
                //                FullSign = m.FullSign,
                //                DocumentHash = m.DocumentHash,
                //                IsLinked = ShortToBool(m.IsLinked),
                //                IsVisible = ShortToBool(m.IsVisible),
                //                IsConservated = ShortToBool(m.IsConservated),
                //                DateExpire = m.DateExpire,
                //                Name = m.Name,
                //                Size = m.Size,
                //                NodeType = m.IdNodeType == null ? null : new DocumentNodeType((short)m.IdNodeType),
                //                IsConfirmed = ShortToBool(m.IsConfirmed),
                //                Status = new Status(m.IdDocumentStatus),
                //                IsCheckOut = ShortToBool(m.IsCheckOut),
                //                IdUserCheckOut = m.IdUserChekOut,
                //                DateMain = m.DateMain,
                //                PrimaryKey = m.PrimaryKeyValue,
                //                DateCreated = m.DateCreated
                //            };
                //BindingList<Document> list = new BindingList<Document>(query.ToList<Document>());
                //return list;
            }
        }

        #endregion

        #region Storage

        public BindingList<DocumentStorageArea> GetAllStorageAreaFromStorage(Guid IdStorage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                BindingList<DocumentStorageArea> list = new BindingList<DocumentStorageArea>();
                var query = db.StorageArea.Include("StorageStatus").Where(x => x.Storage.IdStorage == IdStorage);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
        } 

        public Guid AddStorage(BiblosDS.Library.Common.Objects.DocumentStorage Storage)
        {
            Storage.IdStorage = Guid.NewGuid();

            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.Storage entityStorage = Storage.TryToConvertTo<Model.Storage>(db);

                if (Storage.StorageType != null)
                    entityStorage.IdStorageType = Storage.StorageType.IdStorageType;
                
                if (Storage.Server != null && Storage.Server.IdServer != Guid.Empty)
                    entityStorage.IdServer = Storage.Server.IdServer;

                db.AddToStorage(entityStorage);
                db.SaveChanges();                                
            }
            return Storage.IdStorage;
        }

        public void UpdateStorage(BiblosDS.Library.Common.Objects.DocumentStorage Storage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.Storage entityStorage = Storage.TryToConvertTo<Model.Storage>(false);

                if (entityStorage.EntityKey == null)
                    entityStorage.EntityKey = db.CreateEntityKey(entityStorage.GetType().Name, entityStorage);

                var attachedEntity = db.GetObjectByKey(entityStorage.EntityKey) as Model.Storage;

                if (Storage.StorageType != null)
                    entityStorage.IdStorageType = Storage.StorageType.IdStorageType;

                if (Storage.Server != null && Storage.Server.IdServer != Guid.Empty)
                    entityStorage.IdServer = Storage.Server.IdServer;

                db.ApplyCurrentValues(entityStorage.EntityKey.EntitySetName, entityStorage);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Inizializza un nuovo storage di tipo SQL nel DB di Biblos.
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="storageBasePath"></param>
        public void InitializeSQLStorage(string storageName, string storageBasePath)
        {
            using (BiblosDS2010Entities db = new BiblosDS2010Entities(BiblosDSConnectionString))
            {
                db.Storage_SP_InitializeSQL2014Storage(storageName, storageBasePath);
            }
        }

        /// <summary>
        /// Inizializza le funzionalità di FullText index di SQL per lo storage passato
        /// </summary>
        /// <param name="storageName"></param>
        /// <param name="storageBasePath"></param>
        public void InitializeFullTextStorage(string storageName, string storageBasePath)
        {
            using (BiblosDS2010Entities db = new BiblosDS2010Entities(BiblosDSConnectionString))
            {
                db.Storage_SP_InitializeFullTextStorage(storageName, storageBasePath);
            }
        }
        #endregion

        #region Storage Area

        public BindingList<Status> GetAllStorageAreaStatus()
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                BindingList<Status> list = new BindingList<Status>();
                foreach (var item in db.StorageStatus)
                {
                    list.Add(item.TryToConvertTo<Status>(true));
                }
                return list;
            }
        }

        public DocumentStorageArea GetStorageArea(Guid IdStorageArea)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var query = db.StorageArea.Include("StorageStatus")
                    .Where(x => x.IdStorageArea == IdStorageArea).First();
                return query.Convert();
            }
        }

        public BindingList<DocumentStorageArea> GetStorageAreasFromStorageArchive(Guid IdStorage, Guid IdArchive)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                //var query = db.StorageArea.Include("StorageStatus")
                //    .Where(x => x.Storage.IdStorage.IdStorageArea == IdStorageArea).First();
                //return query.TryToConvertTo<DocumentStorageArea>(true);

                var q = from m in db.StorageArea
                        where (from s in db.ArchiveStorage
                               where s.IdArchive == IdArchive
                               && s.IdStorage == IdStorage
                               && s.IdStorage == m.Storage.IdStorage
                               select s).Any()
                        select m;


                BindingList<DocumentStorageArea> list = new BindingList<DocumentStorageArea>();
                foreach (var item in q)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
        }

        public Guid AddStorageArea(DocumentStorageArea StorageArea)
        {
            StorageArea.IdStorageArea = Guid.NewGuid();

            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.StorageArea entityAttribute = StorageArea.TryToConvertTo<Model.StorageArea>(false);

                if (StorageArea.Status != null && StorageArea.Status.IdStatus > 0)
                    entityAttribute.IdStorageStatus = StorageArea.Status.IdStatus;

                if (StorageArea.Storage != null)
                    entityAttribute.IdStorage = StorageArea.Storage.IdStorage;

                if (StorageArea.Archive != null)
                    entityAttribute.IdArchive = StorageArea.Archive.IdArchive;

                db.AddToStorageArea(entityAttribute);
                db.SaveChanges();
            }

            return StorageArea.IdStorageArea;
        }

        public void UpdateStorageArea(DocumentStorageArea StorageArea)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {

                Model.StorageArea entityAttribute = StorageArea.TryToConvertTo<Model.StorageArea>(false);

                if (entityAttribute.EntityKey == null)
                    entityAttribute.EntityKey = db.CreateEntityKey(entityAttribute.GetType().Name, entityAttribute);

                var attachedEntity = db.GetObjectByKey(entityAttribute.EntityKey) as Model.StorageArea;

                if (StorageArea.Status != null && StorageArea.Status.IdStatus > 0)
                    entityAttribute.IdStorageStatus = StorageArea.Status.IdStatus;

                if (StorageArea.Storage != null)
                    entityAttribute.IdStorage = StorageArea.Storage.IdStorage;

                if (StorageArea.Archive != null)
                    entityAttribute.IdArchive = StorageArea.Archive.IdArchive;

                db.ApplyCurrentValues(entityAttribute.EntityKey.EntitySetName, entityAttribute);

                db.SaveChanges();
            }
        }

        public void DeleteStorageArea(Guid IdStorageArea)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var entity = db.StorageArea.Where(x => x.IdStorageArea == IdStorageArea).First();
                if (entity == null)
                    throw new Exception("Nessuna Storage Area individuata");
                db.DeleteObject(entity);
                db.SaveChanges();
            }

        }

        #endregion       
    
        #region StorageRule

        public void AddStorageRule(BiblosDS.Library.Common.Objects.DocumentStorageRule StorageRule)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.StorageRule entityStorageRule = StorageRule.TryToConvertTo<Model.StorageRule>(db);

                if (StorageRule.Storage != null)
                {
                    entityStorageRule.IdStorage = StorageRule.Storage.IdStorage;
                    //entityStorageRule.StorageReference.TryToAttach(new Model.Storage { IdStorage = StorageRule.Storage.IdStorage }, db);
                }
                else
                    throw new Exception("Storage non valorizzato");

                if (StorageRule.Attribute != null)
                {
                    entityStorageRule.IdAttribute = StorageRule.Attribute.IdAttribute;
                    //entityStorageRule.AttributesReference.TryToAttach(new Model.Attributes { IdAttribute = StorageRule.Attribute.IdAttribute }, db);
                }
                else
                    throw new Exception("Attribute non valorizzato");

                if (StorageRule.RuleOperator != null)
                {
                    entityStorageRule.IdRuleOperator = StorageRule.RuleOperator.IdRuleOperator;
                }

                db.AddToStorageRule(entityStorageRule);
                db.SaveChanges();
            }
        }

        public void UpdateStorageRule(BiblosDS.Library.Common.Objects.DocumentStorageRule StorageRule)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.StorageRule entityStorageRule = StorageRule.TryToConvertTo<Model.StorageRule>(db);                

                if (StorageRule.Storage != null)
                {
                    entityStorageRule.IdStorage = StorageRule.Storage.IdStorage;
                    //entityStorageRule.StorageReference.TryToAttach(new Model.Storage { IdStorage = StorageRule.Storage.IdStorage }, db); 
                }
                else
                    throw new Exception("Storage non valorizzato");

                if (StorageRule.Attribute != null)
                {
                    entityStorageRule.IdAttribute = StorageRule.Attribute.IdAttribute;
                    //entityStorageRule.AttributesReference.TryToAttach(new Model.Archive { IdArchive = StorageRule.Attribute.IdAttribute }, db); 
                }
                else
                    throw new Exception("Attribute non valorizzato");

                if (entityStorageRule.EntityKey == null)
                    entityStorageRule.EntityKey = db.CreateEntityKey(entityStorageRule.GetType().Name, entityStorageRule);
                var attachedEntity = db.GetObjectByKey(entityStorageRule.EntityKey) as Model.StorageRule;

                if (StorageRule.RuleOperator != null)
                {
                    entityStorageRule.IdRuleOperator = StorageRule.RuleOperator.IdRuleOperator;
                }                
               
                db.ApplyCurrentValues(entityStorageRule.EntityKey.EntitySetName, entityStorageRule);
                db.SaveChanges();
            }
        }

        #endregion

        #region StorageAreaRule

        public DocumentStorageAreaRule GetStorageAreaRule(Guid IdStorageArea, Guid IdAttribute)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var query = db.StorageAreaRule
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Include("Attributes.Archive")
                    .Include("StorageArea")
                    .Include("RuleOperator")
                    .Where(x => x.StorageArea.IdStorageArea == IdStorageArea && x.Attributes.IdAttribute == IdAttribute).First();
                return query.Convert();
            }
        }

        public BindingList<DocumentStorageAreaRule> GetStorageAreaRuleFromStorageArea(Guid IdStorageArea)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                BindingList<DocumentStorageAreaRule> list = new BindingList<DocumentStorageAreaRule>();
                var query = db.StorageAreaRule
                    .Include("Attributes")
                    .Include("Attributes.AttributesMode")
                    .Include("Attributes.Archive")
                    .Include("StorageArea")
                    .Include("RuleOperator")
                    .Where(x => x.StorageArea.IdStorageArea == IdStorageArea);
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
        }

        public BindingList<DocumentRuleOperator> GetRuleOperators()
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                BindingList<DocumentRuleOperator> list = new BindingList<DocumentRuleOperator>();
                var query = db.RuleOperator;
                foreach (var item in query)
                {
                    list.Add(item.Convert());
                }
                return list;
            }
        }

        public void UpdateStorageAreaRule(BiblosDS.Library.Common.Objects.DocumentStorageAreaRule StorageAreaRule)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.StorageAreaRule entityStorageAreaRule = StorageAreaRule.TryToConvertTo<Model.StorageAreaRule>(db);

                if (StorageAreaRule.StorageArea != null)
                    entityStorageAreaRule.IdStorageArea = StorageAreaRule.StorageArea.IdStorageArea;
                else
                    throw new Exception("StorageArea non valorizzato");

                if (StorageAreaRule.Attribute != null)
                    entityStorageAreaRule.IdAttribute = StorageAreaRule.Attribute.IdAttribute;
                else
                    throw new Exception("Attribute non valorizzato");

                if (StorageAreaRule.RuleOperator != null)
                {
                    entityStorageAreaRule.IdRuleOperator = StorageAreaRule.RuleOperator.IdRuleOperator;
                }

                if (entityStorageAreaRule.EntityKey == null)
                    entityStorageAreaRule.EntityKey = db.CreateEntityKey(entityStorageAreaRule.GetType().Name, entityStorageAreaRule);                

                var attachedEntity = db.GetObjectByKey(entityStorageAreaRule.EntityKey) as Model.StorageAreaRule;
            
                db.ApplyCurrentValues(entityStorageAreaRule.EntityKey.EntitySetName, entityStorageAreaRule);
                db.SaveChanges();
            }
        }

        public void AddStorageAreaRule(BiblosDS.Library.Common.Objects.DocumentStorageAreaRule StorageAreaRule)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.StorageAreaRule entityStorageAreaRule = StorageAreaRule.TryToConvertTo<Model.StorageAreaRule>(db);

                if (StorageAreaRule.StorageArea != null)
                {
                    entityStorageAreaRule.IdStorageArea = StorageAreaRule.StorageArea.IdStorageArea;
                    //entityStorageAreaRule.StorageAreaReference.TryToAttach(new Model.StorageArea { IdStorageArea = StorageAreaRule.StorageArea.IdStorageArea }, db);
                }
                else
                    throw new Exception("StorageArea non valorizzato");

                if (StorageAreaRule.Attribute != null)
                {
                    entityStorageAreaRule.IdAttribute = StorageAreaRule.Attribute.IdAttribute;
                    //entityStorageAreaRule.AttributesReference.TryToAttach(new Model.Attributes { IdAttribute = StorageAreaRule.Attribute.IdAttribute }, db);
                }
                else
                    throw new Exception("Attribute non valorizzato");

                if (StorageAreaRule.RuleOperator != null)
                {
                    entityStorageAreaRule.IdRuleOperator = StorageAreaRule.RuleOperator.IdRuleOperator;
                }

                db.AddToStorageAreaRule(entityStorageAreaRule);
                db.SaveChanges();
            }
        }

        #endregion

        #region ArchiveStorage

        public BindingList<DocumentArchiveStorage> GetAllStorageAndArchive()
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var query = db.ArchiveStorage.Include("Storage").Include("Storage.StorageType").Include("Archive");
                BindingList<DocumentArchiveStorage> list = new BindingList<DocumentArchiveStorage>();
                foreach (var item in query)
                {
                    list.Add(item.TryToConvertTo<DocumentArchiveStorage>(true));
                }
                return list;
            }
        }

        public BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromArchive(Guid IdArchive)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var query = db.ArchiveStorage
                    .Include("Archive")
                    .Include("Storage")
                    .Include("Storage.StorageType")
                    .Where(x => x.IdArchive == IdArchive);
                BindingList<DocumentArchiveStorage> list = new BindingList<DocumentArchiveStorage>();
                foreach (var item in query)
                {
                    list.Add(item.TryToConvertTo<DocumentArchiveStorage>(true));
                }
                return list;
            }
        }

        public BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromStorage(Guid IdStorage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var query = db.ArchiveStorage
                    .Include("Archive")
                    .Include("Storage")
                    .Include("Storage.StorageType")
                    .Where(x => x.IdStorage == IdStorage);
                BindingList<DocumentArchiveStorage> list = new BindingList<DocumentArchiveStorage>();
                foreach (var item in query)
                {
                    list.Add(item.TryToConvertTo<DocumentArchiveStorage>(true));
                }
                return list;
            }
        }

        public void AddArchiveStorage(BiblosDS.Library.Common.Objects.DocumentArchiveStorage ArchiveStorage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.ArchiveStorage entityArchiveStorage = ArchiveStorage.TryToConvertTo<Model.ArchiveStorage>(db);

                if (ArchiveStorage.Storage != null)
                {
                    entityArchiveStorage.IdStorage = ArchiveStorage.Storage.IdStorage;
                    //entityArchiveStorage.StorageReference.TryToAttach(new Model.Storage { IdStorage = ArchiveStorage.Storage.IdStorage }, db);
                }
                else
                    throw new Exceptions.ArchiveStorage_Exception("Storage non valorizzato");

                if (ArchiveStorage.Archive != null)
                {
                    entityArchiveStorage.IdArchive = ArchiveStorage.Archive.IdArchive;
                    //entityArchiveStorage.ArchiveReference.TryToAttach(new Model.Archive { IdArchive = ArchiveStorage.Archive.IdArchive }, db);
                }
                else
                    throw new Exceptions.ArchiveStorage_Exception("Archivio non valorizzato");

                //if (ArchiveStorage. != null)
                //    entityStorage.StorageTypeReference.TryToAttach(new Model.StorageType { IdStorageType = ArchiveStorage.StorageType.IdStorageType }, db);

                db.AddToArchiveStorage(entityArchiveStorage);
                db.SaveChanges();
            }
        }

        public void UpdateArchiveStorage(BiblosDS.Library.Common.Objects.DocumentArchiveStorage ArchiveStorage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {

                Model.ArchiveStorage entityArchiveStorage = ArchiveStorage.TryToConvertTo<Model.ArchiveStorage>(false);

                //var itemIndB = db.ArchiveStorage.Where(x => x.IdArchive == ArchiveStorage.Archive.IdArchive && x.IdStorage == ArchiveStorage.Storage.IdStorage).First();
                //itemIndB.Active = ArchiveStorage.Active ? (short)1 : (short)0;

                if (ArchiveStorage.Storage != null)
                    entityArchiveStorage.IdStorage = ArchiveStorage.Storage.IdStorage;
                else
                    throw new Exceptions.ArchiveStorage_Exception("Storage non valorizzato");

                if (ArchiveStorage.Archive != null)
                    entityArchiveStorage.IdArchive = ArchiveStorage.Archive.IdArchive;
                else
                    throw new Exceptions.ArchiveStorage_Exception("Archivio non valorizzato");

                if (entityArchiveStorage.EntityKey == null)
                    entityArchiveStorage.EntityKey = db.CreateEntityKey(entityArchiveStorage.GetType().Name, entityArchiveStorage);

                var attachedEntity = db.GetObjectByKey(entityArchiveStorage.EntityKey) as Model.ArchiveStorage;

                //if (ArchiveStorage.StorageType != null)
                //    entityStorage.StorageTypeReference.TryToAttach(new Model.StorageType { IdStorageType = ArchiveStorage.StorageType.IdStorageType }, attachedEntity.StorageTypeReference, db);

                db.ApplyCurrentValues(entityArchiveStorage.EntityKey.EntitySetName, entityArchiveStorage);
                db.SaveChanges();
            }
        }

        public void DeleteArchiveStorage(BiblosDS.Library.Common.Objects.DocumentArchiveStorage ArchiveStorage)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var entity = db.ArchiveStorage.Where(x => x.Archive.IdArchive == ArchiveStorage.Archive.IdArchive && x.Storage.IdStorage == ArchiveStorage.Storage.IdStorage).FirstOrDefault();
                if (entity == null)
                    throw new Exceptions.ArchiveStorage_Exception();
                db.DeleteObject(entity);
                db.SaveChanges();
            }
        }
 
        #endregion

        #region Archive

        internal void DeleteArchive(DocumentArchive archiveToClone)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.Archive entityArchive = db.Archive.Where(x => x.IdArchive == archiveToClone.IdArchive).SingleOrDefault();
                if (entityArchive != null)
                {
                    db.Archive.DeleteObject(entityArchive);
                    db.SaveChanges();
                }
            }
        }

        public Guid AddArchive(BiblosDS.Library.Common.Objects.DocumentArchive Archive)
        {
            Archive.IdArchive = Guid.NewGuid();
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.Archive entityArchive = Archive.TryToConvertTo<Model.Archive>(db);
                db.AddToArchive(entityArchive);
                db.SaveChanges();
            }
            return Archive.IdArchive;
        }

        public void UpdateArchive(BiblosDS.Library.Common.Objects.DocumentArchive Archive)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.Archive entityArchive = Archive.TryToConvertTo<Model.Archive>(false);

                if (entityArchive.EntityKey == null)
                    entityArchive.EntityKey = db.CreateEntityKey(entityArchive.GetType().Name, entityArchive);

                var attachedEntity = db.GetObjectByKey(entityArchive.EntityKey) as Model.Archive;

                db.ApplyCurrentValues(entityArchive.EntityKey.EntitySetName, entityArchive);
                db.SaveChanges();
            }
        }

        public void AddStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable)
        {            
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                throw new NotImplementedException();
                //db.AddStorageToArchive(IdArchive, IdStorage, Enable== true ? (short)1 : (short)0);
            }         
        }

        public void UpdateStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                throw new NotImplementedException();
                //db.UpdateStorageToArchive(IdArchive, IdStorage, Enable == true ? (short)1 : (short)0);
            }
        }       

        #endregion

        #region Attribute

        /// <summary>
        /// Retrive all the attribute modify Mode
        /// </summary>
        /// <returns></returns>
        public BindingList<DocumentAttributeMode> GetAttributeModes()
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                BindingList<DocumentAttributeMode> list = new BindingList<DocumentAttributeMode>();
                foreach (var item in db.AttributesMode)
                {
                    list.Add(item.TryToConvertTo<DocumentAttributeMode>(true));
                }
                return list;
            }
        }

        public void UpdateAttribute(DocumentAttribute Attribute)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {

                Model.Attributes entityAttribute = Attribute.TryToConvertTo<Model.Attributes>(false);

                if (entityAttribute.EntityKey == null)
                    entityAttribute.EntityKey = db.CreateEntityKey(entityAttribute.GetType().Name, entityAttribute);

                var attachedEntity = db.GetObjectByKey(entityAttribute.EntityKey) as Model.Attributes;

                if (Attribute.Mode != null)
                    entityAttribute.IdMode = Attribute.Mode.IdMode;

                if (Attribute.Archive != null)
                    entityAttribute.IdArchive = Attribute.Archive.IdArchive;

                if (Attribute.AttributeGroup != null)                
                    entityAttribute.IdAttributeGroup = Attribute.AttributeGroup.IdAttributeGroup;

                db.ApplyCurrentValues(entityAttribute.EntityKey.EntitySetName, entityAttribute);
                db.SaveChanges();
            }
        }

        public void AddAttribute(DocumentAttribute Attribute)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                Model.Attributes entityAttribute = Attribute.TryToConvertTo<Model.Attributes>(false);

                if (Attribute.Mode != null)
                    entityAttribute.IdMode = Attribute.Mode.IdMode;

                if (Attribute.Archive != null)
                    entityAttribute.IdArchive = Attribute.Archive.IdArchive;

                if (Attribute.AttributeGroup != null)
                    entityAttribute.IdAttributeGroup = Attribute.AttributeGroup.IdAttributeGroup;

                db.AddToAttributes(entityAttribute);
                db.SaveChanges();
            }
        }

        public void DeleteAttribute(Guid IdAttribute, bool logicalDelete = true)
        {
            using (Model.BiblosDS2010Entities db = new Model.BiblosDS2010Entities(BiblosDSConnectionString))
            {
                var entity = db.Attributes.Where(x => x.IdAttribute == IdAttribute).FirstOrDefault();
                if (entity == null)
                    throw new Exceptions.Attribute_Exception("Nessun attributo individuato");
                if (logicalDelete)
                    entity.IsVisible = 0;
                else
                    db.Attributes.DeleteObject(entity);
                db.SaveChanges();
            }

        }

        #endregion

        #region Private

        public static bool ShortToBool(short value)
        {
            return value.Equals((short)1);
        }

        public static bool ShortToBool(short? value)
        {
            return value != null && value.Equals((short)1);
        }

        #endregion

       
    }
}
