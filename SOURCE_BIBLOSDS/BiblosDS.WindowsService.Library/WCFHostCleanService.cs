using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Preservation.ServiceReferenceStorage;
using BiblosDS.Library.Common.Services;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using VecompSoftware.BiblosDS.WindowsService.Common.Helpers;

namespace BiblosDS.WindowsService.Library
{
    public class WCFHostCleanService : IWCFHostService
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WCFHostCleanService));
        private readonly Timer _timer;
        private const string SERVICE_STORAGE_ENDPOINT_NAME = "ServiceDocumentStorage";
        private readonly string _modulePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(WCFHostCleanService)).Location);
        private const string ARCHIVE_RESTRICTIONS_FILENAME = "ArchiveRestrictions.json";
        #endregion

        #region [ Properties ]
        public ICollection<Guid> ArchiveRestrictions => JsonConvert.DeserializeObject<ICollection<Guid>>(File.ReadAllText(Path.Combine(_modulePath, "Config", ARCHIVE_RESTRICTIONS_FILENAME)));

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
        #endregion

        #region [ Constructor ]
        public WCFHostCleanService(TimeSpan waitTimer)
        {
            _timer = new Timer(waitTimer.TotalMilliseconds)
            {
                AutoReset = false
            };
            _timer.Elapsed += TimerCallback;
        }
        #endregion

        #region [ Methods ]
        public void Start()
        {
            if (_timer != null)
            {
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        private void TimerCallback(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger.Debug("TimerCallback -> init clean service process");
                _logger.Info("TimerCallback -> read archives from db");
                ICollection<DocumentArchive> archives = ArchiveService.GetArchives();
                if (archives == null || archives.Count == 0)
                {
                    _logger.Info("TimerCallback -> no archives found from db");
                    _timer.Start();
                    return;
                }

                if (ArchiveRestrictions != null && ArchiveRestrictions.Count > 0)
                {
                    _logger.Info("TimerCallback -> restrict archives from configuration");
                    archives = archives.Where(x => ArchiveRestrictions.Any(xx => xx == x.IdArchive)).ToList();
                }

                _logger.InfoFormat("TimerCallback -> found {0} archives to process", archives.Count);
                foreach (DocumentArchive archive in archives)
                {
                    _logger.InfoFormat("TimerCallback -> process archive {0}({1})", archive.Name, archive.IdArchive);
                    ICollection<Document> documentsToDelete = DocumentService.GetRemovableDetachedDocuments(archive.IdArchive, FromDate, ToDate);
                    _logger.InfoFormat("TimerCallback -> found {0} documents detached from archive {1}({2})", documentsToDelete.Count, archive.Name, archive.IdArchive);
                    foreach (Document document in documentsToDelete)
                    {
                        ClientActionHelper.TryCatchWithLogger<ServiceDocumentStorageClient, IServiceDocumentStorage>((ServiceDocumentStorageClient client) =>
                        {
                            _logger.InfoFormat("TimerCallback -> deleting document {0} from storage {1}...", document.IdDocument, document.Storage?.Name);
                            client.DeleteDocument(document.IdDocument);
                            _logger.InfoFormat("TimerCallback -> deleting document {0} from db...", document.IdDocument);
                            DocumentService.DeleteDocumentDetached(document.IdDocument);

                            if (document.DocumentParentVersion != null)
                            {
                                _logger.InfoFormat("TimerCallback -> document {0} has parent version. Remove all versioned document.", document.IdDocument);
                                ICollection<Document> allVersionedDocuments = DocumentService.GetAllVersionedDocuments(document.DocumentParentVersion);
                                allVersionedDocuments = allVersionedDocuments.Where(x => x.IdDocument != document.IdDocument).ToList();
                                _logger.InfoFormat("TimerCallback -> found {0} versioned documents to delete.", allVersionedDocuments.Count);
                                foreach (Document versionDocument in allVersionedDocuments)
                                {
                                    _logger.InfoFormat("TimerCallback -> deleting document {0} with version {1} from storage...", versionDocument.IdDocument, versionDocument.Version);
                                    client.DeleteDocument(versionDocument.IdDocument);
                                    _logger.InfoFormat("TimerCallback -> deleting document {0} with version {1} from db...", versionDocument.IdDocument, versionDocument.Version);
                                    DocumentService.DeleteDocumentDetached(versionDocument.IdDocument);
                                }
                            }

                            _logger.InfoFormat("TimerCallback -> document {0} deleted", document.IdDocument);
                        }, TimeSpan.FromMinutes(5), SERVICE_STORAGE_ENDPOINT_NAME, _logger);
                    }
                }
                _logger.Debug("TimerCallback -> end clean service process");
            }
            catch (Exception ex)
            {
                _logger.Error("TimerCallback -> error on clean detached document process", ex);
            }
            _timer.Start();
        }
        #endregion        
    }
}
