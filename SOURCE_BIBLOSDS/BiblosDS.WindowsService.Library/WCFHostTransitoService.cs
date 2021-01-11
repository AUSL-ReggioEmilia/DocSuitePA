using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using BiblosDS.WindowsService.Library.ServiceReferenceStorage;
using log4net;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Timers;
using VecompSoftware.BiblosDS.WindowsService.Common.Helpers;

namespace BiblosDS.WindowsService.Library
{
    public class WCFHostTransitoService : IWCFHostService
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WCFHostTransitoService));
        private readonly TimeSpan _waitTimer;
        private readonly Timer _timer;
        private const string SERVICE_STORAGE_ENDPOINT_NAME = "ServiceDocumentStorage";
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WCFHostTransitoService(TimeSpan waitTimer)
        {
            _waitTimer = waitTimer;
            _logger.DebugFormat("WCFHostTransitoService -> set interval {0}", waitTimer.TotalMilliseconds);
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
            _logger.Info("TimerCallback -> init process transit documents");
            ClientActionHelper.TryCatchWithLogger<ServiceDocumentStorageClient, IServiceDocumentStorage>((ServiceDocumentStorageClient client) => 
            { 
                _logger.Info("TimerCallback -> get documents from transit");
                BindingList<Document> documents = DocumentService.GetDocumentInTransito(3);
                _logger.DebugFormat("TimerCallback -> found {0} documents on transit", documents.Count);
                Document document;
                foreach (Document item in documents)
                {
                    try
                    {
                        document = DocumentService.GetDocument(item.IdDocument);
                        _logger.InfoFormat("TimerCallback -> processing document {0}", item.IdDocument);
                        client.AddDocument(document);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(string.Concat("TimerCallback -> error on process document ", item.IdDocument), ex);
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WS, "ProcessCheckInTransitoDocument", e.ToString(), LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                                LoggingLevel.BiblosDS_Errors);

                        if (client.State == CommunicationState.Faulted)
                        {
                            client.Close();
                            client = new ServiceDocumentStorageClient(SERVICE_STORAGE_ENDPOINT_NAME);
                            client.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(30);
                            client.Open();
                        }
                    }
                }

                _logger.Info("TimerCallback -> get document attaches from transit");
                BindingList<DocumentAttach> documentAttaches = DocumentService.GetDocumentAttachesInTransito(3);
                _logger.DebugFormat("TimerCallback -> found {0} document attaches on transit", documentAttaches.Count);
                
                foreach (DocumentAttach attach in documentAttaches)
                {
                    try
                    {
                        _logger.InfoFormat("processing document", attach.IdDocument);
                        client.AddAttachToDocument(attach);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(string.Concat("TimerCallback -> error on process document ", attach.IdDocument), ex);
                        Logging.WriteLogEvent(LoggingSource.BiblosDS_WS, "ProcessCheckInTransitoDocument", e.ToString(), LoggingOperationType.BiblosDS_CheckInTransitoDocument,
                                LoggingLevel.BiblosDS_Errors);

                        if (client.State == CommunicationState.Faulted)
                        {
                            client.Close();
                            client = new ServiceDocumentStorageClient(SERVICE_STORAGE_ENDPOINT_NAME);
                            client.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(30);
                            client.Open();
                        }
                    }
                }
            }, TimeSpan.FromMinutes(30), SERVICE_STORAGE_ENDPOINT_NAME, _logger);
           
            _timer.Start();
        }
        #endregion
    }
}
