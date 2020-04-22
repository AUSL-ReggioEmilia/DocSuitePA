using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;

namespace VecompSoftware.ServiceBus.Receiver
{
    [LogCategory(LogCategoryDefinition.SERVICEBUS)]
    public partial class Receiver : ServiceBase
    {
        #region [ Properties ]
        private ILogger _logger = new GlobalLogger();
        private ReceiverServiceBase _receiverServiceBase = null;
        protected static IEnumerable<LogCategory> _logCategories = null;
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Receiver));
                }
                return _logCategories;
            }
        }
        #endregion

        public Receiver()
        {
            InitializeComponent();
        }
        public void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _logger.WriteInfo(new LogMessage(string.Concat("OnStart receiver service ", ServiceName)), LogCategories);
            _receiverServiceBase = new ReceiverServiceBase(_logger);
            Task.Run(() => _receiverServiceBase.StartAsync().Wait());
        }

        protected override void OnStop()
        {
            if (_receiverServiceBase != null)
            {
                _receiverServiceBase.Dispose();
                _receiverServiceBase = null;
            }
            _logger.WriteInfo(new LogMessage("Stopping service"), LogCategories);
            _logger = null;
        }

    }
}
