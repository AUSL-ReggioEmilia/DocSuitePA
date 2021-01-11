using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using VecompSoftware.Services.Logging;

namespace AmministrazioneTrasparente.Tools
{
    public static class HtmlExtensions
    {
        private static List<Uri> _internalSites;
        private static List<Uri> _externalSites;
        private static Dictionary<string, string> _urlRewriterDictionary;

        private static IEnumerable<Uri> InternalSites
        {
            get
            {
                if (_internalSites != null) return _internalSites;
                _internalSites = new List<Uri>();
                var rewriteCondition = ConfigurationManager.AppSettings["LocalUrlRewriteCondition"];
                if (string.IsNullOrEmpty(rewriteCondition)) return _internalSites;
                var sites = rewriteCondition.Split('|');
                if (sites.Count() <= 1) return _internalSites;

                // Calcolo l'url interno
                foreach (var site in sites[0].Split(';'))
                    _internalSites.Add(new Uri(site));
                return _internalSites;
            }
        }

        private static IEnumerable<Uri> ExternalSites
        {
            get
            {
                if (_externalSites != null) return _externalSites;
                _externalSites = new List<Uri>();
                var rewriteCondition = ConfigurationManager.AppSettings["LocalUrlRewriteCondition"];
                if (string.IsNullOrEmpty(rewriteCondition)) return _externalSites;
                var sites = rewriteCondition.Split('|');
                if (sites.Count() <= 1) return _externalSites;

                // Calcolo l'url interno
                foreach (var site in sites[1].Split(';'))
                    _externalSites.Add(new Uri(site));
                return _externalSites;
            }
        }

        private static Dictionary<string, string> UrlRewriterDictionary
        {
            get
            {
                if (_urlRewriterDictionary != null) return _urlRewriterDictionary;
                _urlRewriterDictionary = new Dictionary<string, string>();
                var urlDictionary = ConfigurationManager.AppSettings["UrlRewriterDictionary"];
                if (string.IsNullOrEmpty(urlDictionary)) return _urlRewriterDictionary;
                foreach (var urlCouple in urlDictionary.Split(';').Select(s => s.Split('|')).Where(urlCouple => urlCouple.Count() > 1))
                {
                    _urlRewriterDictionary.Add(urlCouple[0], urlCouple[1]);
                }
                return _urlRewriterDictionary;
            }
            
        }

        /// <summary>
        /// Permette di formattare i link presenti in text con l'html per i url
        /// </summary>
        public static string UrlToAnchor(this string text)
        {
            var pattern = ConfigurationManager.AppSettings["URLRegex"];
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern))
                return text?? string.Empty;

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                var url = match.Value.UrlRewrite().CheckDictionary();
                text = text.Replace(match.Value, url);
                var protocol = string.Empty;
                if (url.StartsWith("www", StringComparison.InvariantCultureIgnoreCase))
                {
                    protocol = "http://";
                }
                text = text.Replace(url, "<a target='_blank' href='" + protocol + url + "'>" + match.Value + "</a>");
            }
            return text;
        }

        /// <summary>
        /// Metodo che si occupa di effettuare le sostituzioni regex dal parametro UrlRewriterDictionary
        /// </summary>
        /// <param name="urlToCheck"></param>
        /// <returns></returns>
        private static string CheckDictionary(this string urlToCheck)
        {
            foreach (var urlPattern in UrlRewriterDictionary)
            {
                var regex = new Regex(urlPattern.Key, RegexOptions.IgnoreCase);
                if (regex.Matches(urlToCheck).Count < 1) continue;
                return regex.Replace(urlToCheck,urlPattern.Value);
            }
            return urlToCheck;
        }

        /// <summary>
        /// Metodo che si occupa di effettuare le sostituzioni dei link standard del sito
        /// </summary>
        /// <param name="urlToCheck"></param>
        /// <returns></returns>
        private static string UrlRewrite(this string urlToCheck)
        {
            var url = new Uri(urlToCheck);
            var request = System.Web.HttpContext.Current.Request;
            String urlToRedirect = String.Format("{0}://{1}:{2}{3}", request.Url.Scheme, request.Url.Host, request.Url.Port, request.ApplicationPath.TrimEnd('/'));
            
            FileLogger.Debug("Application", String.Format("request.Url.Scheme: {0}", request.Url.Scheme));
            FileLogger.Debug("Application", String.Format("request.Url.Host: {0}", request.Url.Host));
            FileLogger.Debug("Application", String.Format("request.Url.Port: {0}", request.Url.Port));
            FileLogger.Debug("Application", String.Format("request.ApplicationPath.: {0}", request.ApplicationPath.TrimEnd('/')));
            FileLogger.Debug("Application", String.Format("final url before parse: {0}", urlToRedirect));

            var applicationUri = new Uri(urlToRedirect);

            FileLogger.Debug("Application", String.Format("CurrentUriBasePath: {0} - Type: {1}", applicationUri, applicationUri.GetUrlType()));
            return url.GetConsistentUrl(applicationUri).AbsoluteUri;
        }

        /// <summary>
        /// Verifica la tipologia di link (tra applicativo interno, applicativo esterno e altro)
        /// </summary>
        /// <param name="urlToCheck"></param>
        /// <returns></returns>
        private static UrlType GetUrlType(this Uri urlToCheck)
        {
            var isInternal = InternalSites.Any(
                internalSite => internalSite.Scheme == urlToCheck.Scheme &&
                    internalSite.Authority == urlToCheck.Authority && 
                    internalSite.Port == urlToCheck.Port && 
                    urlToCheck.PathAndQuery.IndexOf(internalSite.PathAndQuery, StringComparison.OrdinalIgnoreCase)>=0);
            // L'url è interno
            if (isInternal) return UrlType.ApplicationInternal;

            var isExternal = ExternalSites.Any(
                externalSite => externalSite.Scheme == urlToCheck.Scheme &&
                    externalSite.Authority == urlToCheck.Authority &&
                    externalSite.Port == urlToCheck.Port && 
                    urlToCheck.PathAndQuery.IndexOf(externalSite.PathAndQuery, StringComparison.OrdinalIgnoreCase)>=0);
            // L'url è esterno
            if (isExternal) return UrlType.ApplicationExternal;

            // L'url è genericamente altro
            return UrlType.Other;
        }

        /// <summary>
        /// Effettua la sostituzione del link riconosciuto con il corrispondente corrente
        /// </summary>
        /// <param name="urlToChange"></param>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        private static Uri GetConsistentUrl(this Uri urlToChange, Uri currentPath)
        {
            if (urlToChange.GetUrlType() == UrlType.Other) return urlToChange;

            var newPath = String.Format("{0}{1}", currentPath.PathAndQuery.TrimEnd('/'),
                            urlToChange.PathAndQuery.Substring(urlToChange.PathAndQuery.LastIndexOf('/')));
            return new Uri(String.Format("{0}://{1}:{2}{3}", currentPath.Scheme, currentPath.Host, currentPath.Port, newPath));
        }
        
        /// <summary>
        /// Tipologia di link
        /// </summary>
        private enum UrlType
        {
            ApplicationInternal,
            ApplicationExternal,
            Other
        }
    }
}