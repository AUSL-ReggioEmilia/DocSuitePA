using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using BiblosDS.Library.IStorage;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using System.ComponentModel;
using System.Reflection;
using BiblosDS.Library.Common;


namespace BiblosDS.WCF.Storage
{
    public class StorageAssemblyLoader
    {
        static object objLock = new object();
        static StorageAssemblyLoader storageAssemblyLoader;
        Dictionary<Guid, IStorage> storages;

        private StorageAssemblyLoader()
        {
            storages = new Dictionary<Guid, IStorage>();
            try
            {
                BindingList<DocumentStorageType> storagesType = StorageService.GetStoragesType();
                Assembly a;
                foreach (DocumentStorageType item in storagesType)
                {
                    a = Assembly.Load(new AssemblyName(item.StorageAssembly));
                    IStorage assemblyStore = (IStorage)a.CreateInstance(item.StorageAssembly + "." + item.StorageClassName);                    
                    if (assemblyStore != null)
                        storages.Add(item.IdStorageType, assemblyStore);
                    else
                        Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_WCF_Storage,
                            "StorageAssemblyLoader",
                            "Configurazione non trovata nell'assembly: " + item.StorageAssembly + "." + item.StorageClassName,
                             BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                              BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_WCF_Storage,
                            "StorageAssemblyLoader",
                            ex.ToString(),
                             BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                              BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);
                throw;
            }            
        }

        /// <summary>
        /// Get the istance loader
        /// </summary>
        /// <returns></returns>
        public static StorageAssemblyLoader GetIstance()
        {
            lock (objLock)
            {
                if (storageAssemblyLoader == null)
                    storageAssemblyLoader = new StorageAssemblyLoader();
            }
            return storageAssemblyLoader;
        }

        /// <summary>
        /// Retrive the storage assembly loaded
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static IStorage GetStorage(Guid IdStorageType)
        {
            if (GetIstance().storages.ContainsKey(IdStorageType))
            {
                return GetIstance().storages[IdStorageType];
            }
            else
                throw new BiblosDS.Library.Common.Exceptions.StorageConfiguration_Exception("Nessuno storage caricato con questo nome.");
        }
    }
}
