using BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces;
using log4net;
using System;

namespace BiblosDS.LegalExtension.AdminPortal.Infrastructure.Services.Common
{
    public class LoggerService : ILogger
    {
        #region [ Fields ]
        private readonly ILog _logger;
        #endregion

        #region [ Constructor ]
        public LoggerService()
        {
            _logger = LogManager.GetLogger(typeof(MvcApplication));
        }

        public LoggerService(ILog logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]        
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message, exception);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }
        #endregion
    }
}