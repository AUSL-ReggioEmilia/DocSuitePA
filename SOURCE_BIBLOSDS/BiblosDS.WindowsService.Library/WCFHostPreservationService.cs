using System;
using System.Timers;
using BiblosDS.Library.Common.Preservation.Services;
using log4net;

namespace BiblosDS.WindowsService.Library
{
    public class WCFHostPreservationService : IWCFHostService
    {
        #region [ Fields ]
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WCFHostPreservationService));
        private readonly Timer _timer;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public WCFHostPreservationService(TimeSpan waitTimer)
        {
            _logger.DebugFormat("WCFHostPreservationService -> set interval {0}", waitTimer.TotalMilliseconds);
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
                _logger.Info("TimerCallback -> start preservation process");
                PreservationService.ProcessExpiredTask();                
                _logger.Info("TimerCallback -> end preservation process");
            }
            catch (Exception ex)
            {
                _logger.Error("TimerCallback -> error on preservation process", ex);
            }
            _timer.Start();
        }
        #endregion
    }
}
