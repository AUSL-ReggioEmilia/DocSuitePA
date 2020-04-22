using System;
using System.Linq;
using System.Timers;
using VecompSoftware.BPM.Integrations.Model.Configurations;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.ServiceWake.Periodic
{
    public class PeriodicServiceWake : ServiceWakeBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly GlobalConfiguration _configuration;
        private readonly Timer _timer;
        private DateTime _timerStartDate;
        private DateTime _timerEndDate;

        private Action _executeAction;
        private Action _closeAction;
        private PeriodConfiguration _periodicConfiguration;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public PeriodicServiceWake(ILogger logger, GlobalConfiguration configuration)
            : base(logger)
        {
            _timer = new Timer();
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region [ Methods ]
        public override void OnAddModule()
        {
            _periodicConfiguration = _configuration.PeriodConfigurations.SingleOrDefault(x => x.Name == CurrentModule.Timer);
            if (_periodicConfiguration == null)
            {
                _logger.WriteWarning(new LogMessage($"PeriodicServiceWake.OnAddModule() -> timer {CurrentModule.Timer} is not defined for module {CurrentModule}."), LogCategories);
                return;
            }
            _timerStartDate = DateTime.Today.AddMilliseconds(_periodicConfiguration.StartHour.TotalMilliseconds);
            _timerEndDate = DateTime.Today.AddMilliseconds(_periodicConfiguration.EndHour.TotalMilliseconds);
            double interval = _periodicConfiguration.Period.TotalMilliseconds;
            if (DateTime.Now < _timerStartDate)
            {
                interval = (_timerStartDate - DateTime.Now).TotalMilliseconds;
            }

            _timer.Interval = interval;
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _logger.WriteInfo(new LogMessage($"PeriodicServiceWake -> module {CurrentModule.Name} load timer {CurrentModule.Timer} correctly. Starting at {_timerStartDate} to closing at {_timerEndDate} with interval {_periodicConfiguration.Period.TotalMilliseconds} millisecond."), LogCategories);
        }

        public override void OnStart(Action executeAction, Action closeAction)
        {
            _logger.WriteInfo(new LogMessage("PeriodicServiceWake.OnStart() -> start timer"), LogCategories);
            _executeAction = executeAction;
            _closeAction = closeAction;
            _timer.Start();
        }

        public override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("PeriodicServiceWake.OnStop() -> stop timer"), LogCategories);
            _timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Interval = _periodicConfiguration.Period.TotalMilliseconds;
            bool skipExecution = DateTime.Now >= _timerEndDate;
            if (skipExecution)
            {
                _closeAction();
                _timerStartDate = _timerStartDate.AddDays(1);
                _timerEndDate = _timerEndDate.AddDays(1);
                _timer.Interval = (_timerStartDate - DateTime.Now).TotalMilliseconds;
                _logger.WriteInfo(new LogMessage($"PeriodicServiceWake -> module {CurrentModule.Name} setted new wakeup at {_timerStartDate} to closing at {_timerEndDate}"), LogCategories);
            }
            else
            {
                _executeAction();
            }

        }


        #endregion
    }
}
