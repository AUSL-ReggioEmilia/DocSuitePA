using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Objects.Response;
using System.ServiceModel.Activation;
using VecompSoftware.ServiceContract.BiblosDS.Logs;

/// <summary>
/// Summary description for Logs
/// </summary>
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Log : ILog
{
    public Log()
    {
    }

    public void DoWork() { }

    public BindingList<BiblosDS.Library.Common.Objects.Log> GetAllLogs(DateTime from, DateTime to)
    {
        return Logging.GetAllLogs(from, to);
    }

    public BindingList<BiblosDS.Library.Common.Objects.Log> GetArchiveLogs(DateTime from, Guid idArchive)
    {
        return Logging.GetArchiveLogs(from, idArchive);
    }

    public BindingList<Guid> GetLogIDArchives() 
    {
        return Logging.GetLogIDArchives();
    }

    public LogResponse GetLogsPaged(DateTime from, DateTime to, Guid? idArchive, int skip, int take)
    {
        List<Log> documents = new List<Log>();
        int logsInArchiveCount = 0;
        var items = Logging.GetLogsPaged(from, to, idArchive, skip, take, out logsInArchiveCount);        
        return new LogResponse { Logs = items, TotalRecords = logsInArchiveCount };
    }
}