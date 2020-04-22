using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.Common.Objects;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Services
{
    public class ArchiveStorageService : ServiceBase
    {
        /// <summary>
        /// Ritorna il DocumentArchive individuato dai GUID Archive e Storage
        /// </summary>
        /// <param name="IdArchive">GUID del document Archive </param>
        /// <param name="IdStorage">GUID del document Storage </param>
        /// <returns>DocumentArchiveStorage</returns>
        public static DocumentArchiveStorage GetArchiveStorage(Guid IdArchive, Guid IdStorage)
        {
            try
            {
                return DbProvider.GetArchiveStorage(IdArchive,IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "ArchiveStorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }


        /// <summary>
        /// Restituisce l'elenco di tutte le relazioni storage-archive per l'archive selezionato
        /// </summary>
        /// <param name="Archive">DocumentArchive di cui ottenere le relazioni agli storages</param>
        /// <returns>BindingList<DocumentArchiveStorage></returns>
        public static BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromArchive(DocumentArchive Archive)
        {
            try
            {
                return DbAdminProvider.GetStorageArchiveRelationsFromArchive(Archive.IdArchive);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "ArchiveStorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }


        /// <summary>
        ///  Restituisce l'elenco di tutte le relazioni storage-archive
        /// </summary>
        /// <returns>BindingList<DocumentArchiveStorage></returns>
        public static BindingList<DocumentArchiveStorage> GetAllStorageAndArchive()
        {
            try
            {
                return DbAdminProvider.GetAllStorageAndArchive();
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "ArchiveStorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        /// <summary>
        /// Restituisce l'elenco di tutte le relazioni storage-archive per lo storage selezionato
        /// </summary>
        /// <param name="Storage">DocumentStorage di cui ottenere le relazioni con gli archives</param>
        /// <returns>BindingList<DocumentArchiveStorage></returns>
        public static BindingList<DocumentArchiveStorage> GetStorageArchiveRelationsFromStorage(DocumentStorage Storage)
        {
            try
            {
                return DbAdminProvider.GetStorageArchiveRelationsFromStorage(Storage.IdStorage);
            }
            catch (Exception e)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                                        "ArchiveStorageService." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                                        e.ToString(),
                                        BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                                        BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw e;
            }
        }

        #region ArchiveStorage Admin

        /// <summary>
        /// Inserisce una nuova relazione archive-storage
        /// </summary>
        /// <param name="ArchiveStorage">Relazione ArchiveStorage da inserire</param>
        public static void AddArchiveStorage(DocumentArchiveStorage ArchiveStorage)
        {
            if (ArchiveStorage.Archive == null
                || ArchiveStorage.Archive.IdArchive == null
                || ArchiveStorage.Storage == null
                || ArchiveStorage.Storage.IdStorage == null)
                throw new Exception("Impossibile proseguire. Archive e Storage devono essere valorizzati");

            DbAdminProvider.AddArchiveStorage(ArchiveStorage);
        }

        /// <summary>
        /// Salva le modifiche a una relazione archive-storage
        /// </summary>
        /// <param name="ArchiveStorage">Relazione ArchiveStorage modificata</param>
        public static void UpdateArchiveStorage(DocumentArchiveStorage ArchiveStorage)
        {
            if (ArchiveStorage.Archive == null
                || ArchiveStorage.Archive.IdArchive == null
                || ArchiveStorage.Storage == null
                || ArchiveStorage.Storage.IdStorage == null)
                    throw new Exception("Impossibile proseguire. Archive e Storage devono essere valorizzati");

            DbAdminProvider.UpdateArchiveStorage(ArchiveStorage);
        }

        /// <summary>
        /// Elimina permanentemente una relazione Archive-Storage
        /// </summary>
        /// <param name="ArchiveStorage">DocumentArchiveStorage da eliminare</param>
        public static void DeleteArchiveStorage(DocumentArchiveStorage ArchiveStorage)
        {
            if (ArchiveStorage.Archive == null
                || ArchiveStorage.Archive.IdArchive == null
                || ArchiveStorage.Storage == null
                || ArchiveStorage.Storage.IdStorage == null)
                    throw new Exception("Impossibile proseguire. Archive e Storage devono essere valorizzati");

            DbAdminProvider.DeleteArchiveStorage(ArchiveStorage);
        }

        #endregion
    }
}
