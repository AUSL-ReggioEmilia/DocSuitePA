using BiblosDS.Library.IStorage;
using BiblosDS.Library.Storage.Azure;
using BiblosDS.Library.Storage.BDSCom;
using BiblosDS.Library.Storage.FileSystem;
using BiblosDS.Library.Storage.Ftp;
using BiblosDS.Library.Storage.SharePoint;
using BiblosDS.Library.Storage.SharePoint2010;
using BiblosDS.Library.Storage.SharePoint2010DS;
using BiblosDS.Library.Storage.SQL;
using System;
using System.Collections.Generic;

namespace VecompSoftware.BiblosDS.Service.Storage
{
    internal class StorageInstanceBuilder
    {
        #region [ Fields ]
        private readonly IDictionary<string, Func<IStorage>> _storageInstances;
        private readonly string _storageType;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public StorageInstanceBuilder(string storageType)
        {
            _storageType = storageType;
            _storageInstances = new Dictionary<string, Func<IStorage>>()
            {
                { "BiblosDS.Library.Storage.Azure.AzureStorage", () => new AzureStorage() },
                { "BiblosDS.Library.Storage.SharePoint2010DS.SharePointStorage2010DS", () => new SharePointStorage2010DS() },
                { "BiblosDS.Library.Storage.SharePoint.SharePointStorage", () => new SharePointStorage() },
                { "BiblosDS.Library.Storage.SharePoint2010.SharePointStorage2010", () => new SharePointStorage2010() },
                { "BiblosDS.Library.Storage.Ftp.FtpStorage", () => new FtpStorage() },
                { "BiblosDS.Library.Storage.SQL.SQL2008Storage", () => new SQL2008Storage() },
                { "BiblosDS.Library.Storage.FileSystem.FileSystem", () => new FileSystem() },
                { "BiblosDS.Library.Storage.BDSCom.BDSComStorage", () => new BDSComStorage() },
                { "BiblosDS.Library.Storage.SQL.SQL2014Storage", () => new SQL2014Storage() }
            };
        }
        #endregion

        #region [ Methods ]
        public IStorage BuildStorage()
        {
            if (_storageInstances.ContainsKey(_storageType))
            {
                return _storageInstances[_storageType]();
            }
            return null;
        }
        #endregion
    }
}
