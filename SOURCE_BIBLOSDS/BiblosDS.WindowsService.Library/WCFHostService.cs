using log4net;
using System;
using System.Collections.Generic;
using VecompSoftware.BiblosDS.WindowsService.Common.Configurations;

namespace BiblosDS.WindowsService.Library
{
    public class WCFHostService
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WCFHostService));
        private readonly ServiceHostManager _serviceHostManager;
        private ICollection<IWCFHostService> _wcfServices;
        #endregion        

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WCFHostService()
        {
            _serviceHostManager = new ServiceHostManager();
            _wcfServices = new List<IWCFHostService>();
        }
        #endregion

        #region [ Methods ]
        public void InitializeServices()
        {
            try
            {
                _logger.Info("InitializeServices -> initialize configured services");
                _serviceHostManager.StartServiceHosts();

                if (ServiceConfiguration.EnableTransito)
                {
                    _logger.Info("InitializeServices - start transit service");
                    WCFHostTransitoService transitoService = new WCFHostTransitoService(TimeSpan.FromMinutes(ServiceConfiguration.TransitoTimerWaitMinute));
                    transitoService.Start();
                    _wcfServices.Add(transitoService);
                    _logger.Info("InitializeServices - transit service started correctly");
                }
                else
                {
                    _logger.Info("InitializeServices - transit service disabled");
                }                    

                if (ServiceConfiguration.EnablePreservation)
                {
                    _logger.Info("InitializeServices - start preservation service");
                    WCFHostPreservationService preservationService = new WCFHostPreservationService(TimeSpan.FromMinutes(ServiceConfiguration.PreservationTimerWaitMinute));
                    preservationService.Start();
                    _wcfServices.Add(preservationService);
                    _logger.Info("InitializeServices - preservation service started correctly");
                }
                else
                {
                    _logger.Info("InitializeServices - preservation service disabled");
                }

                if (ServiceConfiguration.EnableCleanDocuments)
                {
                    _logger.Info("InitializeServices - start clean service");
                    WCFHostCleanService cleanService = new WCFHostCleanService(TimeSpan.FromMinutes(ServiceConfiguration.CleanDocumentsTimerWaitMinute));
                    cleanService.FromDate = ServiceConfiguration.CleanDocumentsFromDate;
                    cleanService.ToDate = ServiceConfiguration.CleanDocumentsToDate;
                    cleanService.Start();
                    _wcfServices.Add(cleanService);
                    _logger.Info("InitializeServices - clean service started correctly");
                }
                else
                {
                    _logger.Info("InitializeServices - clean service disabled");
                }

                _logger.Info("InitializeServices - start check alive service");
                WCFHostWebStateService webStateService = new WCFHostWebStateService();
                webStateService.Start();
                _wcfServices.Add(webStateService);
                _logger.Info("InitializeServices - check alive service started correctly");
            }
            catch (Exception ex)
            {
                _logger.Error("InitializeServices -> error on initialize WCFHost services", ex);
                throw;
            }
        }

        public void StopServices()
        {
            _logger.Info("StopServices -> stopping services...");
            StopInternalServices();
            _serviceHostManager.StopServiceHosts();
            _logger.Info("StopServices -> services stopped");
        }

        private void StopInternalServices()
        {
            foreach (IWCFHostService wcfService in _wcfServices)
            {
                wcfService.Stop();
            }
        }
        #endregion        
    }
}
