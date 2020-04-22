using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Objects;
using System.Configuration;

namespace BiblosDS.Library.Common
{
    public class Journaling: ServiceBase
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Journaling));

        public static void WriteJournaling(LoggingSource Source, string ClassMethod, string Message,
           LoggingOperationType OperationType, LoggingLevel Level, Nullable<Guid> IdCorrelation, Document document)
        {
            if (document == null)
                document = new Document();
            WriteJournaling(Source, "", ClassMethod, Message, LoggingOperationType.BiblosDS_General, Level, 
                document.Archive == null ? Guid.Empty : document.Archive.IdArchive, 
                document.Storage == null ? Guid.Empty : document.Storage.IdStorage, IdCorrelation);
        }

        public static void WriteJournaling(LoggingSource Source, string UserAgent, string ClassMethod, string Message,
            LoggingOperationType OperationType, LoggingLevel Level,
            Nullable<Guid> IdArchive, Nullable<Guid> IdStorage, Nullable<Guid> IdCorrelation)
        {
            try
            {
                if (ConfigurationManager.AppSettings["JournalEnabled"] == null || ConfigurationManager.AppSettings["JournalEnabled"].ToString() == "true")
                {
                    DbProvider.AddJournal(OperationType,
                    UserAgent,
                    Guid.NewGuid(),
                    IdArchive == null ? Guid.Empty : (Guid)IdArchive,
                    IdStorage == null ? Guid.Empty : (Guid)IdStorage,
                    IdCorrelation == null ? Guid.Empty : (Guid)IdCorrelation,
                    Message);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }            
        }
    }
}
