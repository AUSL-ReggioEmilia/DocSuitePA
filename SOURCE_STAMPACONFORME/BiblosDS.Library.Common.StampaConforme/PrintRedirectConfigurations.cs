using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BiblosDS.Library.Common.StampaConforme
{
    public enum ConverterType
    {
        OpenOffice = 1,
        Tif = 2,
        Redirect = 3,
        Image = 4,
        Txt = 5
    }

    public class ServiceLog
    {
        public int FailedCount { get; set; }
        public string WsUrl { get; set; }        
        public int Priority { get; set; }
        public DateTime LastCall { get; set; }
    }

    public sealed class PrintRedirectConfigurations
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PrintRedirectConfigurations));
        IDictionary<string, ConverterType> extensions;
        IDictionary<int, ServiceLog> services;
        IDictionary<int, ServiceLog> servicesOpenOffice;
        int maxFailedCount;

        public int ServicesCount { get { return services.Count(); } }

        private static PrintRedirectConfigurations _istance;

        private PrintRedirectConfigurations()
        {
            extensions = new Dictionary<string, ConverterType>();
            services = new Dictionary<int, ServiceLog>();
            servicesOpenOffice = new Dictionary<int, ServiceLog>();
            maxFailedCount = 5;
            if (ConfigurationManager.AppSettings["RedirectConvertMaxWsFailedCount"] != null)
                maxFailedCount = int.Parse(ConfigurationManager.AppSettings["RedirectConvertMaxWsFailedCount"].ToString());
            if (ConfigurationManager.AppSettings["RedirectConvertWsUrl"] != null)
            {
                var redirectUrl = ConfigurationManager.AppSettings["RedirectConvertWsUrl"].ToString().Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < redirectUrl.Count(); i++)
                {
                    services.Add(i, new ServiceLog { WsUrl = redirectUrl[i] });
                }
            }
            if (ConfigurationManager.AppSettings["RedirectConvertOpenOfficeWsUrl"] != null)
            {
                var redirectUrl = ConfigurationManager.AppSettings["RedirectConvertOpenOfficeWsUrl"].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < redirectUrl.Count(); i++)
                {
                    servicesOpenOffice.Add(i, new ServiceLog { WsUrl = redirectUrl[i] });
                }
            }
            if (ConfigurationManager.AppSettings["OpenOfficeExtension"] != null)
            {
                var openOfficeExtension = ConfigurationManager.AppSettings["OpenOfficeExtension"].ToString().Split(',');
                foreach (var item in openOfficeExtension)
                {
                    if (!extensions.ContainsKey(item.ToLower()))
                        extensions.Add(item.ToLower(), ConverterType.OpenOffice); 
                }              
            }
            if (ConfigurationManager.AppSettings["TifExtension"] != null)
            {
                var tifExtension = ConfigurationManager.AppSettings["TifExtension"].ToString().Split(',');
                foreach (var item in tifExtension)
                {
                    if (!extensions.ContainsKey(item.ToLower()))
                        extensions.Add(item.ToLower(), ConverterType.Tif);
                }
            }
            if (ConfigurationManager.AppSettings["RedirectExtension"] != null)
            {
                var tifExtension = ConfigurationManager.AppSettings["RedirectExtension"].ToString().Split(',');
                foreach (var item in tifExtension)
                {
                    if (!extensions.ContainsKey(item.ToLower()))
                        extensions.Add(item.ToLower(), ConverterType.Redirect);
                }
            }
        }

        public static PrintRedirectConfigurations GetIstance()
        {
            if (_istance == null)
                _istance = new PrintRedirectConfigurations();
            return _istance;
        }

        public static void SetWsUrlPriority(ConverterType convertType, string wsUrl)
        {
            var objIstance = GetIstance();
            ServiceLog objPriority;
            if (convertType == ConverterType.OpenOffice)
                objPriority = objIstance.servicesOpenOffice.Select(x => x.Value).Where(x => x.WsUrl == wsUrl).First();
            else
                objPriority = objIstance.services.Select(x => x.Value).Where(x => x.WsUrl == wsUrl).First();
            IncreasePriority(convertType, objIstance, objPriority);
            objPriority.FailedCount = 0;
            objPriority.LastCall = DateTime.Now;
        }

        public static void SetWsUrlFault(ConverterType convertType, string wsUrl)
        {
            var objIstance = GetIstance();
            ServiceLog failedCount;
            if (convertType == ConverterType.OpenOffice)
                failedCount = objIstance.servicesOpenOffice.Select(x => x.Value).Where(x => x.WsUrl == wsUrl).First();
            else
                failedCount = objIstance.services.Select(x => x.Value).Where(x => x.WsUrl == wsUrl).First();
            IncreasePriority(convertType, objIstance, failedCount);
            failedCount.FailedCount += 1;
            
        }

        private static void ResetPriority(ConverterType convertType, PrintRedirectConfigurations objIstance)
        {
            if (convertType == ConverterType.OpenOffice)
            {
                foreach (var item in objIstance.servicesOpenOffice.Values)
                {
                    item.Priority = 0;
                    item.FailedCount = 0;
                }
            }
            else
            {
                foreach (var item in objIstance.services.Values)
                {
                    item.Priority = 0;
                    item.FailedCount = 0;
                }
            }
        }

        private static void IncreasePriority(ConverterType convertType, PrintRedirectConfigurations objIstance, ServiceLog failedCount)
        {
            if (failedCount.Priority < Int32.MaxValue - 1)
                failedCount.Priority += 1;
            else
                ResetPriority(convertType, objIstance);
        }

        public static string GetWsUrl(ConverterType convertType, string wsToSkip = "")
        {
            ServiceLog objUrl = null;
            var objIstance = GetIstance();
            if (convertType == ConverterType.OpenOffice)
                objUrl = objIstance.servicesOpenOffice.Select(x => x.Value).Where(x => x.FailedCount < objIstance.maxFailedCount && (wsToSkip == "" || x.WsUrl != wsToSkip)).FirstOrDefault();
            else
                objUrl = objIstance.services.Select(x => x.Value).Where(x => x.FailedCount < objIstance.maxFailedCount && (wsToSkip == "" || x.WsUrl != wsToSkip)).FirstOrDefault();
            if (objUrl != null)
            {
                objUrl.LastCall = DateTime.Now;
                return objUrl.WsUrl;
            }
            else
            {
                objIstance.logger.Warn("Attenzione...nessun server di stampa conforme OFFICE disponibile per la chiamata. Try to call first....");
                if (convertType == ConverterType.OpenOffice)
                    return objIstance.servicesOpenOffice.Select(x => x.Value).OrderBy(x => x.LastCall).First().WsUrl;
                else
                    return objIstance.services.Select(x => x.Value).OrderBy(x => x.LastCall).First().WsUrl;
            }
        }

        public static ConverterType GetConverter(string extension)
        {
            var objIstance = GetIstance();
            if (objIstance.extensions.ContainsKey(extension.ToLower().Trim()))
                return objIstance.extensions[extension.ToLower().Trim()];
            else
            {
                switch (extension.ToLower().Trim())
                {
                    case ".doc":
                    case ".docx":                    
                    case ".htm":
                    case ".html":
                    case ".mht":
                    case ".xls":
                    case ".xlsx":
                    case ".xlsm":
                    case ".ppt":
                    case ".pptx":
                    case ".odt":
                        return ConverterType.OpenOffice;
                    case ".tif":
                    case ".tiff":
                        return ConverterType.Tif;
                    case ".png":
                    case ".gif":
                    case ".jpg":
                    case ".jpeg":
                        return ConverterType.Image;
                    case ".txt":
                        return ConverterType.Txt;
                }
            }
            throw new FormatException("Formato non supportato");
        }
    }
}
