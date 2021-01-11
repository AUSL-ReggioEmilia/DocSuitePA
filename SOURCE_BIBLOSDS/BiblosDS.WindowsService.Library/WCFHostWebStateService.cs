using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Enums;
using BiblosDS.Library.Common.Preservation.ServiceReferenceStorage;
using BiblosDS.WindowsService.Library.ServiceReferenceDocument;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VecompSoftware.BiblosDS.WindowsService.Common.Helpers;

namespace BiblosDS.WindowsService.Library
{
    public class WCFHostWebStateService : IWCFHostService
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WCFHostWebStateService));
        private readonly Timer _timer;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WCFHostWebStateService()
        {
            _timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds)
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

        void TimerCallback(object sender, ElapsedEventArgs e)
        {
            ClientActionHelper.TryCatchWithLogger<DocumentsClient, IDocuments>((DocumentsClient client) =>
            {
                if (!client.IsAlive())
                {
                    Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "WCFHost.GetAliveTimer_Tick", "ServiceDocument IsAlive() return false",
                        LoggingOperationType.BiblosDS_GetAlive, LoggingLevel.BiblosDS_Warning);
                }
            }, TimeSpan.FromMinutes(5), string.Empty, _logger);

            ClientActionHelper.TryCatchWithLogger<ServiceDocumentStorageClient, IServiceDocumentStorage>((ServiceDocumentStorageClient client) =>
            {
                if (!client.IsAlive())
                {
                    Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "WCFHost.GetAliveTimer_Tick", "ServiceDocumentStorage IsAlive() return false",
                            LoggingOperationType.BiblosDS_GetAlive, LoggingLevel.BiblosDS_Warning);
                }
            }, TimeSpan.FromMinutes(5), string.Empty, _logger);
            _timer.Start();
        }
        #endregion        
    }
}
