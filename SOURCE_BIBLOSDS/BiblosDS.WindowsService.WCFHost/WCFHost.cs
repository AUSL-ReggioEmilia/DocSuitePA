using System;
using System.ServiceProcess;
using BiblosDS.WindowsService.Library;
using log4net;

namespace BiblosDS.WindowsService.WCFHost
{
    public partial class WCFHost : ServiceBase
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WCFHost));
        private readonly WCFHostService _service;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WCFHost()
        {
            InitializeComponent();
            _service = new WCFHostService();
        }
        #endregion

        #region [ Methods ]
        protected override void OnStart(string[] args)
        {
            try
            {
                _logger.Info(string.Concat("OnStart service ", this.ServiceName));
                _service.InitializeServices();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error on start service ", this.ServiceName), ex);
                throw;
            }
        }

        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
            try
            {
                _logger.Info(string.Concat("OnStop service ", this.ServiceName));
                _service.StopServices();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error on stop service ", this.ServiceName), ex);
                throw;
            }
        }
        #endregion        
    }
}
