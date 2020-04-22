using BiblosDS.Library.Common;
using BiblosDS.Library.Common.Enums;
using BiblosDS.WCF.Documents;
using BiblosDS.WCF.Preservation;
using BiblosDS.WCF.Storage;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BiblosDS.WindowsService.Library
{
    public class ServiceHostManager
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ServiceHostManager));
        private ServiceHost _serviceHostDocument;
        private ServiceHost _serviceHostPreservation;
        private ServiceHost _serviceHostStorage;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public ServiceHostManager()
        {

        }
        #endregion

        #region [ Methods ]
        public void StartServiceHosts()
        {
            StartServiceHostDocument();
            StartServiceHostPreservation();
            StartServiceHostStorage();
        }

        public void StopServiceHosts()
        {
            StopServiceHostDocument();
            StopServiceHostPreservation();
            StopServiceHostStorage();
        }

        private void StartServiceHostDocument()
        {
            try
            {
                _logger.Info("StartServiceHostDocument -> hosting and start service document...");
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WindowsService_WCFHost, "StartServiceHostDocument", string.Empty, "Start",
                     LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, null, null, null);

                if (_serviceHostDocument != null)
                {
                    _serviceHostDocument.Close();
                }

                _serviceHostDocument = new ServiceHost(typeof(ServiceDocument));
                _serviceHostDocument.Open();
                _logger.Info("StartServiceHostDocument -> service document started correctly");
            }
            catch (Exception ex)
            {
                _logger.Error("StartServiceHostDocument -> error on start service document", ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "StartServiceHostDocument",
                    string.Concat("Error occured when start service document : ", ex.ToString()), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
            }
        }

        private void StartServiceHostPreservation()
        {
            try
            {
                _logger.Info("StartServiceHostPreservation -> hosting and start service preservation...");
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WindowsService_WCFHost, "StartServiceHostPreservation", string.Empty, "Start",
                     LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, null, null, null);

                if (_serviceHostPreservation != null)
                {
                    _serviceHostPreservation.Close();
                }

                _serviceHostPreservation = new ServiceHost(typeof(ServicePreservation));
                _serviceHostPreservation.Open();
                _logger.Info("StartServiceHostPreservation -> service preservation started correctly");
            }
            catch (Exception ex)
            {
                _logger.Error("StartServiceHostPreservation -> error on start service preservation", ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "StartServiceHostPreservation",
                    string.Concat("Error occured when start service preservation : ", ex.ToString()), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
            }
        }

        private void StartServiceHostStorage()
        {
            try
            {
                _logger.Info("StartServiceHostStorage -> hosting and start service document storage...");
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WindowsService_WCFHost, "StartServiceHostStorage", string.Empty, "Start",
                     LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace, null, null, null);

                if (_serviceHostStorage != null)
                {
                    _serviceHostStorage.Close();
                }

                _serviceHostStorage = new ServiceHost(typeof(ServiceDocumentStorage));
                _serviceHostStorage.Open();
                _logger.Info("StartServiceHostStorage -> service document storage started correctly");
            }
            catch (Exception ex)
            {
                _logger.Error("StartServiceHostStorage -> error on start service document storage", ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "StartServiceHostStorage",
                    string.Concat("Error occured when start service document storage : ", ex.ToString()), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
            }
        }

        private void StopServiceHostDocument()
        {
            try
            {
                _logger.Info("StopServiceHostDocument -> stopping service document...");
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WindowsService_WCFHost,
                   "StopServiceDocument", string.Empty, "Stop", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace,
                     null, null, null);

                if (_serviceHostDocument != null)
                {
                    _serviceHostDocument.Close();
                    _serviceHostDocument = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("StopServiceHostDocument -> error on stop service document", ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "StopServiceHostDocument",
                    string.Concat("Error occured when stop service document : ", ex.ToString()), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Managed_Error);
            }
        }

        private void StopServiceHostPreservation()
        {
            try
            {
                _logger.Info("StopServiceHostPreservation -> stopping service preservation...");
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WindowsService_WCFHost,
                   "StopServiceHostPreservation", string.Empty, "Stop", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace,
                     null, null, null);

                if (_serviceHostPreservation != null)
                {
                    _serviceHostPreservation.Close();
                    _serviceHostPreservation = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("StopServiceHostPreservation -> error on stop service document", ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "StopServiceHostPreservation",
                    string.Concat("Error occured when stop service preservation : ", ex.ToString()), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Managed_Error);
            }
        }

        private void StopServiceHostStorage()
        {
            try
            {
                _logger.Info("StopServiceHostStorage -> stopping service document storage...");
                Journaling.WriteJournaling(LoggingSource.BiblosDS_WindowsService_WCFHost,
                   "StopServiceHostStorage", string.Empty, "Stop", LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Trace,
                     null, null, null);

                if (_serviceHostStorage != null)
                {
                    _serviceHostStorage.Close();
                    _serviceHostStorage = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("StopServiceHostStorage -> error on stop service document storage", ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_WindowsService_WCFHost, "StopServiceHostStorage",
                    string.Concat("Error occured when stop service document storage : ", ex.ToString()), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Managed_Error);
            }
        }
        #endregion
    }
}
