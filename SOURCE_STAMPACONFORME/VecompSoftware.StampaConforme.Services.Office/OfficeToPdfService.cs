using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.StampaConforme.Services.Office
{
    public abstract class OfficeToPdfService
    {
        #region [ Fields ]
        private readonly string _applicationProgId;
        private const int _retry_tentative = 5;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public OfficeToPdfService(string applicationProgId)
        {
            _applicationProgId = applicationProgId;
        }
        #endregion

        #region [ Methods ]
        public virtual dynamic GetRuntimeInstance()
        {
            Type applicationClass = Type.GetTypeFromProgID(_applicationProgId);
            return (dynamic)Activator.CreateInstance(applicationClass);
        }

        public T RetryingPolicyAction<T>(Func<T> func, string proccesName, ILogger _logger, IEnumerable<LogCategory> LogCategories, int step = 1)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                _logger.WriteError(new LogMessage("BiblosDS.Library.Common.Converter.Office.OfficeToPdfConverter.RetryingPolicyAction: retry policy expired maximum tentatives"), LogCategories);
                throw new Exception("OpenOfficeToPdfConverter retry policy expired maximum tentatives");
            }
            try
            {
#if DEBUG
                if (step < 3)
                {
                    throw new COMException();
                }
#endif
                return func();
            }
            catch (COMException ex)
            {
                _logger.WriteWarning(new LogMessage(string.Concat($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} failed. Killing Office process before retry.", ex)), LogCategories);
                //localContext = null;
                KillOfficeProcesses(proccesName, _logger, LogCategories);
                return RetryingPolicyAction(func, proccesName, _logger, LogCategories, ++step);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat($"RetryingPolicyAction: exception type not manageable.", ex)), LogCategories);
                throw ex;
            }
        }

        private void KillOfficeProcesses(string proccesName, ILogger _logger, IEnumerable<LogCategory> LogCategories)
        {
            Process[] officeProcesses = Process.GetProcessesByName(proccesName);
            if ((officeProcesses == null || officeProcesses.Length == 0))
            {
                return;
            }
            foreach (Process officeProcess in officeProcesses)
            {
                try
                {
                    if (!officeProcess.HasExited)
                    {
                        officeProcess.Kill();
                    }
                }
                catch (Exception ex)
                {
                    _logger.WriteWarning(new LogMessage(string.Concat($"KillOfficeProcesses:Office process with id {officeProcess.Id} not killed", ex)), LogCategories);
                }
            }
        }

        #endregion
    }
}
