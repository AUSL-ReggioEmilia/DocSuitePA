using System;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Services;
using System.Net;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.Library.Common
{
    public class Logging: ServiceBase
    {
        public static void WriteLogEvent(LoggingSource Source, string ClassMethod, string Message, LoggingOperationType OperationType, LoggingLevel Level) 
        {
            try
            {
                WriteLogEvent(Source, ClassMethod, Message, OperationType, Level, null, null, null); 
            }
            catch
            {
                //PUT into Try to avoid exceptions on the call                
            }                     
        }

        public static void WriteError(LoggingSource Source, string ClassMethod, Exception ex, LoggingOperationType OperationType, Document document)
        {
            try
            {
                WriteLogEvent(Source, ClassMethod, ex.ToString(), OperationType, LoggingLevel.BiblosDS_Errors, document.Archive == null ? Guid.Empty : document.Archive.IdArchive , document.Storage == null ? Guid.Empty : document.Storage.IdStorage, null);
            }
            catch
            {
                //PUT into Try to avoid exceptions on the call                
            }
        }

        public static void WriteLogEvent(LoggingSource Source, string ClassMethod, string Message,
            LoggingOperationType OperationType, LoggingLevel Level,
            Nullable<Guid> IdArchive, Nullable<Guid> IdStorage, Nullable<Guid> IdCorrelation)
        {
            WriteLogEvent(Source, ClassMethod, Message,
            OperationType, Level,
            IdArchive, IdStorage, IdCorrelation, Dns.GetHostName(), string.Empty);
        }

        public static void WriteLogEvent(LoggingSource Source, string ClassMethod, string Message, 
            LoggingOperationType OperationType, LoggingLevel Level,
            Nullable<Guid> IdArchive, Nullable<Guid> IdStorage, Nullable<Guid> IdCorrelation, string Server, String Client) 
        {
            try
            {
                switch (Level) 
                {
                    case LoggingLevel.BiblosDS_Information : 
                    case LoggingLevel.BiblosDS_Trace : 
#if DEBUG
                        EventLogHelper.GetUniqueInstance.Write("Source : " + Source.ToString() +
                                           " ClassMethod : " + ClassMethod +
                                           "\nMessage : " + Message, System.Diagnostics.EventLogEntryType.Information); 
#endif
                        break ;
                    default : 
                    case LoggingLevel.BiblosDS_Warning :
                    case LoggingLevel.BiblosDS_Managed_Error :
                        EventLogHelper.GetUniqueInstance.Write("Source : " + Source.ToString() +
                                           " ClassMethod : " + ClassMethod +
                                           "\nMessage : " + Message, System.Diagnostics.EventLogEntryType.Warning); 
                        break ; 
                    case LoggingLevel.BiblosDS_Errors : 
                        EventLogHelper.GetUniqueInstance.Write("Source : " + Source.ToString() +
                                           " ClassMethod : " + ClassMethod +
                                           "\nMessage : " + Message, System.Diagnostics.EventLogEntryType.Error); 
                        break ;
                }
            }
            catch (Exception)
            {                
                //Ignore
            }
            
            try
            {
                if (ClassMethod.Contains("ConnectionString") == false)
                {
                    // scrive nel database  
                    DbProvider.AddLog(OperationType,
                        Guid.NewGuid(),
                        IdArchive == null ? Guid.Empty : (Guid)IdArchive,
                        IdStorage == null ? Guid.Empty : (Guid)IdStorage,
                        IdCorrelation == null ? Guid.Empty : (Guid)IdCorrelation,
                        Message, Server, Client);
                }
            }
            catch (Exception)
            {               
                //Ignore
            }
        }

        public static void AddLog(LoggingOperationType operationType, Guid idArchive, Guid idStorage, Guid idCorrelation, string message, string client)
        {
            DbProvider.AddLog(operationType, Guid.NewGuid(), idArchive, idStorage, idCorrelation, message, Dns.GetHostName(), client);
        }

        public static BindingList<Log> GetAllLogs(DateTime from, DateTime to)
        {
            return DbProvider.GetAllLogs(from, to);
        }

        public static BindingList<Log> GetArchiveLogs(DateTime from, Guid IdArchive)
        {
            return DbProvider.GetArchiveLogs(from, IdArchive);
        }

        public static BindingList<Log> GetLogsPaged(DateTime from, DateTime to, Guid? IdArchive, int skip, int take, out int totalRecord)
        {
            return DbProvider.GetLogsPaged(from, to, IdArchive, skip, take, out totalRecord);
        }


        public static BindingList<Guid> GetLogIDArchives()
        {
            return DbProvider.GetLogIDArchives();
        }
    }
}