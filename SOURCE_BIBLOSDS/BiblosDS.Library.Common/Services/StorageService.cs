using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using System;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Utility;

namespace BiblosDS.Library.Common.Services
{
    public class StorageService : ServiceBase
    {
        static object objLockStorageArea = new object();
        #region Storage

        public static DocumentStorage GetStorage(Guid IdStorage)
        {
            try
            {
                return DbProvider.GetStorage(IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static DocumentStorage GetStorageWithServer(Guid IdStorage)
        {
            try
            {
                return DbProvider.GetStorageWithServer(IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }


        public static BindingList<DocumentStorage> GetStorages()
        {
            try
            {
                return DbProvider.GetStorages();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorage> GetStoragesWithServer()
        {
            try
            {
                return DbProvider.GetStoragesWithServer();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorage> GetStorageActiveFromArchive(DocumentArchive Archive)
        {
            try
            {
                return DbProvider.GetStoragesActiveFromArchive(Archive.IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorage> GetStorageActiveFromArchiveServer(DocumentArchive archive, Server server)
        {
            try
            {
                return DbProvider.GetStoragesActiveFromArchiveServer(archive.IdArchive, server.IdServer);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorage> GetStorageFromArchive(DocumentArchive Archive)
        {
            try
            {
                return DbProvider.GetStoragesFromArchive(Archive.IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static Guid AddStorage(DocumentStorage Storage)
        {
            // EnableFulText è sempre valorizzato
            if (Storage.StorageType == null
                || Storage.StorageType.IdStorageType == Guid.Empty)
                throw new Exception("Impossibile proseguire. StorageType non assegnato.");

            return DbAdminProvider.AddStorage(Storage);
        }

        public static void UpdateStorage(DocumentStorage Storage)
        {
            // EnableFulText è sempre valorizzato
            if (Storage.IdStorage == Guid.Empty)
                throw new Exception("Impossibile proseguire. IdStorage non valido.");
            if (Storage.StorageType == null
                || Storage.StorageType.IdStorageType == Guid.Empty)
                throw new Exception("Impossibile proseguire. StorageType non assegnato.");

            DbAdminProvider.UpdateStorage(Storage);
        }

        public static void AddStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable)
        {
            try
            {
                DbAdminProvider.AddStorageToArchive(IdStorage, IdArchive, Enable);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static void EnableDisableStorageToArchive(Guid IdStorage, Guid IdArchive, bool Enable)
        {
            DbAdminProvider.UpdateStorageToArchive(IdStorage, IdArchive, Enable);
        }

        public static void InitializeSQLStorage(DocumentStorage storage)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(storage.Name);
                DbAdminProvider.InitializeSQLStorage(safeStorageName, storage.MainPath);
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService.InitializeSQLStorage",
                                        ex.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static void InitializeFullTextStorage(DocumentStorage storage)
        {
            try
            {
                string safeStorageName = UtilityService.SafeSQLName(storage.Name);
                DbAdminProvider.InitializeFullTextStorage(safeStorageName, storage.MainPath);
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General,
                                        "StorageService.InitializeFullTextStorage",
                                        ex.ToString(),
                                        LoggingOperationType.BiblosDS_General,
                                        LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }
        #endregion

        #region Storage Rule

        public static BindingList<DocumentStorageRule> GetStorageRule(DocumentStorage Storage)
        {
            try
            {
                return DbProvider.GetStorageRulesFromStorage(Storage.IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }

        }

        public static BindingList<DocumentStorageRule> GetStorageRulesFromStorage(System.Guid IdStorage)
        {
            try
            {
                return DbProvider.GetStorageRulesFromStorage(IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }

        }

        public static BindingList<DocumentStorageRule> GetStorageRuleFromStorageArchive(System.Guid IdStorage, Guid IdArchive)
        {
            try
            {
                return DbProvider.GetStorageRuleFromStorageArchive(IdStorage, IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }

        }

        public static BindingList<DocumentStorageAreaRule> GetStorageRulesFromStorageArea(System.Guid IdStorageArea)
        {
            try
            {
                return DbProvider.GetStorageRulesFromStorageArea(IdStorageArea);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorageAreaRule> GetStorageRulesFromStorageAreaArchive(System.Guid IdStorageArea, Guid IdArchive)
        {
            try
            {
                return DbProvider.GetStorageRulesFromStorageAreaArchive(IdStorageArea, IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static DocumentStorageRule GetStorageRule(System.Guid IdStorage, Guid IdAttribute)
        {
            try
            {
                return DbProvider.GetStorageRule(IdStorage, IdAttribute);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static void AddStorageRule(DocumentStorageRule StorageRule)
        {
            if (StorageRule.Storage == null
                || StorageRule.Attribute == null
                || StorageRule.Storage.IdStorage == Guid.Empty
                || StorageRule.Attribute.IdAttribute == Guid.Empty)
                throw new Exception("Requisiti non sufficienti per l'operazione");
            DbAdminProvider.AddStorageRule(StorageRule);
        }

        public static void UpdateStorageRule(DocumentStorageRule StorageRule)
        {
            DbAdminProvider.UpdateStorageRule(StorageRule);
        }

        #endregion

        #region StorageArea

        public static BindingList<DocumentStorageArea> GetAllStorageAreaFromStorage(Guid IdStorage)
        {
            try
            {
                return DbAdminProvider.GetAllStorageAreaFromStorage(IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorage> GetStorageFromArchive(System.Guid IdArchive)
        {
            try
            {
                return DbProvider.GetStoragesFromArchive(IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorageArea> GetStorageAreaFromStorage(System.Guid IdStorage)
        {
            try
            {
                return DbProvider.GetStorageAreaFromStorage(IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorageArea> GetStorageAreaActiveFromStorage(System.Guid IdStorage)
        {
            try
            {
                return DbProvider.GetStorageAreaActiveFromStorage(IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }
        
        public static void UpdateStorageAreaSize(System.Guid IdStorageArea, long Size)
        {
            try
            {
                lock (objLockStorageArea)
                {
                    DbProvider.UpdateStorageAreaSize(IdStorageArea, Size);
                }
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorage> GetStoragesNotRelatedToArchive(DocumentArchive Archive)
        {
            try
            {
                return DbProvider.GetStoragesNotRelatedToArchive(Archive.IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                    "StorageService.GetStoragesNotRelatedToArchive", e.ToString(), BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                    BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);

                throw e;
            }
        }

        public static DocumentStorageArea GetStorageArea(Guid IdStorageArea)
        {
            try
            {
                return DbAdminProvider.GetStorageArea(IdStorageArea);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentStorageArea> GetStorageAreasFromStorageArchive(Guid IdStorage, Guid IdArchive)
        {
            try
            {
                return DbAdminProvider.GetStorageAreasFromStorageArchive(IdStorage, IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<Status> GetAllStorageAreaStatus()
        {
            try
            {
                return DbAdminProvider.GetAllStorageAreaStatus();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static Guid AddStorageArea(DocumentStorageArea StorageArea)
        {
            if (string.IsNullOrEmpty(StorageArea.Name)
                || StorageArea.Storage == null
                || StorageArea.Storage.IdStorage == Guid.Empty)
                throw new Exception("Requisiti non sufficienti per l'operazione");
            
            return DbAdminProvider.AddStorageArea(StorageArea);
        }

        public static void UpdateStorageArea(DocumentStorageArea StorageArea)
        {
            if (StorageArea.IdStorageArea == Guid.Empty)
                throw new Exception("Impossibile proseguire. IdStorageArea non valido.");
            if (string.IsNullOrEmpty(StorageArea.Name)
                || StorageArea.Storage == null
                || StorageArea.Storage.IdStorage == Guid.Empty)
                throw new Exception("Requisiti non sufficienti per l'operazione");

            DbAdminProvider.UpdateStorageArea(StorageArea);
        }

        #endregion

        #region StorageArea Rule

        public static BindingList<DocumentStorageAreaRule> GetStorageAreaRulesFromStorageArea(Guid IdStorageArea)
        {
            try
            {
                return DbAdminProvider.GetStorageAreaRuleFromStorageArea(IdStorageArea);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static DocumentStorageAreaRule GetStorageAreaRule(Guid IdStorageArea, Guid IdAttribute)
        {
            try
            {
                return DbAdminProvider.GetStorageAreaRule(IdStorageArea, IdAttribute);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static BindingList<DocumentRuleOperator> GetRuleOperators()
        {
            try
            {
                return DbAdminProvider.GetRuleOperators();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static void AddStorageAreaRule(DocumentStorageAreaRule StorageAreaRule)
        {
            if (StorageAreaRule.StorageArea == null
                || StorageAreaRule.StorageArea.IdStorageArea == Guid.Empty
                || StorageAreaRule.Attribute == null
                || StorageAreaRule.Attribute.IdAttribute == Guid.Empty)
                throw new Exception("Requisiti non sufficienti per l'operazione");
            DbAdminProvider.AddStorageAreaRule(StorageAreaRule);
        }

        public static void UpdateStorageAreaRule(DocumentStorageAreaRule StorageAreaRule)
        {
            if (StorageAreaRule.StorageArea == null
                || StorageAreaRule.StorageArea.IdStorageArea == Guid.Empty
                || StorageAreaRule.Attribute == null
                || StorageAreaRule.Attribute.IdAttribute == Guid.Empty)
                throw new Exception("Requisiti non sufficienti per l'operazione");
            DbAdminProvider.UpdateStorageAreaRule(StorageAreaRule);
        }

        #endregion

        #region StorageType

        public static BindingList<DocumentStorageType> GetStoragesType()
        {
            try
            {
                return DbProvider.GetStoragesType();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        public static DocumentStorageType GetStorageTypeByStorage(Guid idStorage)
        {
            try
            {
                return DbProvider.GetStorageTypeByStorage(idStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "StorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        #endregion
    }
}
