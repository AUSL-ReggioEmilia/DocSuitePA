using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Response;

namespace VecompSoftware.ServiceContract.BiblosDS.Logs
{
    /// <summary>
    /// Interfaccia per la gestione dei logs su DB.
    /// </summary>
    [ServiceContract(Namespace = "http://Vecomp.BiblosDs.Log")]
    public interface ILog
    {
        [OperationContract]
        void DoWork();
        
        #region Get
        [OperationContract]
        BindingList<Log> GetAllLogs(DateTime from, DateTime to);
        [OperationContract]
        BindingList<Log> GetArchiveLogs(DateTime from, Guid idArchive);

        [OperationContract]
        LogResponse GetLogsPaged(DateTime from, DateTime to, Guid? idArchive, int skip, int take);

        [OperationContract]
        BindingList<Guid> GetLogIDArchives();
        #endregion

        #region Add
        #endregion

        #region Update
        #endregion

        #region Delete
        #endregion
    }
}
