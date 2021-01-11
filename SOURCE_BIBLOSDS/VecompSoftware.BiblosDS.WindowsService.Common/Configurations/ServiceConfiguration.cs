using System;
using System.Configuration;
using System.Globalization;

namespace VecompSoftware.BiblosDS.WindowsService.Common.Configurations
{
    public static class ServiceConfiguration
    {
        #region [ Fields ]
        private static int? _transitoTimerWaitMinute;
        private static int? _preservationTimerWaitMinute;
        private static int? _cleanDocumentsTimerWaitMinute;
        private static bool? _enableTransito;
        private static bool? _enablePreservation;
        private static bool? _enableCleanDocuments;
        private static DateTime? _cleanDocumentsFromDate;
        private static DateTime? _cleanDocumentsToDate;
        private static string _webAPIUrl = "http://localhost:9000/WCFHost.WebAPI";
        #endregion

        #region [ Properties ]
        public static int TransitoTimerWaitMinute
        {
            get
            {
                if (!_transitoTimerWaitMinute.HasValue)
                {
                    int waitValue;
                    _transitoTimerWaitMinute = 60;
                    if (int.TryParse(ConfigurationManager.AppSettings["TransitoTimerWaitMinute"], out waitValue))
                    {
                        _transitoTimerWaitMinute = waitValue;
                    }
                }
                return _transitoTimerWaitMinute.Value;
            }
        }

        public static int PreservationTimerWaitMinute
        {
            get
            {
                if (!_preservationTimerWaitMinute.HasValue)
                {
                    int waitValue;
                    _preservationTimerWaitMinute = 60;
                    if (int.TryParse(ConfigurationManager.AppSettings["PreservationTimerWaitMinute"], out waitValue))
                    {
                        _preservationTimerWaitMinute = waitValue;
                    }
                }
                return _preservationTimerWaitMinute.Value;
            }
        }

        public static int CleanDocumentsTimerWaitMinute
        {
            get
            {
                if (!_cleanDocumentsTimerWaitMinute.HasValue)
                {
                    int waitValue;
                    _cleanDocumentsTimerWaitMinute = 60;
                    if (int.TryParse(ConfigurationManager.AppSettings["CleanDocumentsTimerWaitMinute"], out waitValue))
                    {
                        _cleanDocumentsTimerWaitMinute = waitValue;
                    }
                }
                return _cleanDocumentsTimerWaitMinute.Value;
            }
        }

        public static bool EnableTransito
        {
            get
            {
                if (!_enableTransito.HasValue)
                {
                    bool val;
                    _enableTransito = false;
                    if (bool.TryParse(ConfigurationManager.AppSettings["EnableTransitoTimer"], out val))
                    {
                        _enableTransito = val;
                    }
                }
                return _enableTransito.Value;
            }
        }

        public static bool EnablePreservation
        {
            get
            {
                if (!_enablePreservation.HasValue)
                {
                    bool val;
                    _enablePreservation = false;
                    if (bool.TryParse(ConfigurationManager.AppSettings["EnablePreservationTimer"], out val))
                    {
                        _enablePreservation = val;
                    }
                }
                return _enablePreservation.Value;
            }
        }

        public static bool EnableCleanDocuments
        {
            get
            {
                if (!_enableCleanDocuments.HasValue)
                {
                    bool val;
                    _enableCleanDocuments = false;
                    if (bool.TryParse(ConfigurationManager.AppSettings["EnableCleanDocumentsTimer"], out val))
                    {
                        _enableCleanDocuments = val;
                    }
                }
                return _enableCleanDocuments.Value;
            }
        }

        public static DateTime? CleanDocumentsFromDate
        {
            get
            {
                if (!_cleanDocumentsFromDate.HasValue)
                {
                    DateTime val;
                    _cleanDocumentsFromDate = null;
                    if (DateTime.TryParseExact(ConfigurationManager.AppSettings["CleanDocumentsFromDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out val))
                    {
                        _cleanDocumentsFromDate = val;
                    }
                }
                return _cleanDocumentsFromDate;
            }
        }

        public static DateTime? CleanDocumentsToDate
        {
            get
            {
                if (!_cleanDocumentsToDate.HasValue)
                {
                    DateTime val;
                    _cleanDocumentsToDate = null;
                    if (DateTime.TryParseExact(ConfigurationManager.AppSettings["CleanDocumentsToDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out val))
                    {
                        _cleanDocumentsToDate = val;
                    }
                }
                return _cleanDocumentsToDate;
            }
        }

        public static string WebAPIUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebAPIUrl"]) ? _webAPIUrl : ConfigurationManager.AppSettings["WebAPIUrl"];
            }
        }

        #endregion
    }
}
