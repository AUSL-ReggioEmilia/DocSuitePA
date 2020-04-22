using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.CMVGroup
{
    public class CmvGroup
    {

        #region [ Constants ]

        private const string LoggerName = "CMVGroup";

        #endregion

        #region [ Fields ]

        private string _boundary;
        private ResolutionFacade _reslFacade;

        #endregion

        #region [ Properties ]

        private string Boundary
        {
            get
            {
                if (string.IsNullOrEmpty(_boundary))
                    _boundary = "--" + Guid.NewGuid().ToString("N");
                return _boundary;
            }
        }
        private ResolutionFacade ReslFacade
        {
            get { return _reslFacade ?? (_reslFacade = new ResolutionFacade()); }
        }

        #endregion

        #region [ Methods ]

        private void NewBoundary()
        {
            _boundary = null;
        }

        private string FormatMetadata(string name, string value, bool base64)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--" + Boundary);
            sb.AppendFormat("Content-Disposition: form-data; name='{0}'{1}", name, Environment.NewLine);

            var content = value;
            if (base64)
            {
                sb.AppendLine("Content-Transfer-Encoding: base64;");
                content = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }
            sb.AppendLine();
            sb.AppendLine(content);

            return sb.ToString();
        }
        private string FormatMetadata(string name, string value)
        {
            return FormatMetadata(name, value, false);
        }
        private string FormatMetadata(string name, DateTime? value)
        {
            return FormatMetadata(name, value.HasValue ? value.Value.ToString("dd/MM/yyyy") : string.Empty, false);
        }
        private string FormatMetadata(string name, DocumentInfo document)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--" + Boundary);
            sb.AppendFormat("Content-Disposition: form-data; name='{0}'; filename='{1}';{2}", name, document.Name, Environment.NewLine);
            sb.AppendLine("Content-Type: application/pdf;");
            sb.AppendLine("Content-Transfer-Encoding: base64;");
            sb.AppendLine();
            sb.AppendLine(Convert.ToBase64String(document.Stream));

            return sb.ToString();
        }

        private Uri createUri(string uriString)
        {
            var uri = new Uri(uriString);
            var valid = new List<string> { Uri.UriSchemeHttp, Uri.UriSchemeHttps };
            if (!valid.Any(s => s == uri.Scheme))
                throw new InvalidCastException("Uri non valido per il percorso: " + uriString);
            return uri;
        }
        private bool IsAlreadyPublished(Resolution resolution, out string message)
        {
            if (DocSuiteContext.Current.ResolutionEnv.PostToRemoteDisablePublicationTester)
            {
                message = "SKIPPED";
                return false;
            }

            var uriString = "{0}?ANNO={1}&NUMERO={2}&TIPO={3}&DATA_ADOZIONE={4:dd/MM/yyyy}";
            var decodedType = resolution.Type.Id == 0 ? "Determina" : "Delibera";
            uriString = string.Format(uriString, DocSuiteContext.Current.ResolutionEnv.PostToRemotePublicationTesterUrl,
                resolution.Year, resolution.Number, decodedType, resolution.AdoptionDate);
            FileLogger.Info("Url test pubblicazione: ", uriString);
            var uri = createUri(uriString);

            try
            {
                var request = HttpWebRequest.Create(uri) as HttpWebRequest;
                request.Method = WebRequestMethods.Http.Get;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                    message = sr.ReadToEnd();
                FileLogger.Info(LoggerName, "isAlreadyPublished response: " + message);

                if (message == "KO: dati mancanti")
                    throw new InvalidOperationException("Parametri insufficienti per effettuare la richiesta: " + message);
                else if (message.StartsWith("OK", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.Error(LoggerName, ex.Message, ex);
                throw;
            }
            return false;
        }
        private string GetContent(Resolution resolution, DocumentInfo document)
        {
            var sb = new StringBuilder();
            sb.Append(FormatMetadata("OGGETTO", resolution.ResolutionObject, true));
            sb.Append(FormatMetadata("TITOLO", resolution.Note));
            sb.Append(FormatMetadata("NUMERO", resolution.Number.ToString()));
            sb.Append(FormatMetadata("ANNO", resolution.Year.ToString()));

            var codiceDelibera = ReslFacade.GetResolutionNumber(resolution);
            sb.Append(FormatMetadata("CODICE_DELIBERA", codiceDelibera));

            sb.Append(FormatMetadata("DATA_INSERIMENTO", resolution.ProposeDate));
            sb.Append(FormatMetadata("DATA_ADOZIONE", resolution.AdoptionDate));
            sb.Append(FormatMetadata("DATA_PUBBLICAZIONE", resolution.PublishingDate));

            var dataEsecutivita = resolution.EffectivenessDate
                .GetValueOrDefault(resolution.PublishingDate.Value.AddDays(10));
            sb.Append(FormatMetadata("DATA_ESECUTIVITA", dataEsecutivita));
            var dataScadenza = resolution.PublishingDate.Value.AddDays(DocSuiteContext.Current.ResolutionEnv.PublicationEndDays - 5);
            sb.Append(FormatMetadata("DATA_SCADENZA", dataScadenza));

            var decodedType = resolution.Type.Id == 0 ? "Determina" : "Delibera";
            sb.Append(FormatMetadata("TIPO", decodedType));

            sb.Append(FormatMetadata("DOCUMENTO", document));

            sb.Append("--" + Boundary);
            return sb.ToString();
        }

        public bool Publish(Resolution resolution, DocumentInfo document, out string message)
        {
            FileLogger.Info(LoggerName, "Inizio pubblicazione IdResolution: " + resolution.Id.ToString(CultureInfo.InvariantCulture));
            NewBoundary();
            FileLogger.Info(LoggerName, "Nuovo boundary: " + Boundary);
            if (IsAlreadyPublished(resolution, out message))
                return false;

            FileLogger.Info(LoggerName, "Esito test pubblicazione: " + message);
            try
            {
                var content = GetContent(resolution, document);
                FileLogger.Debug(LoggerName, content);
                var encoded = Encoding.UTF8.GetBytes(content);

                FileLogger.Info("Url test pubblicazione: ", DocSuiteContext.Current.ResolutionEnv.PostToRemoteUrl);
                var uri = createUri(DocSuiteContext.Current.ResolutionEnv.PostToRemoteUrl);
                var request = HttpWebRequest.Create(uri) as HttpWebRequest;

                request.Method = WebRequestMethods.Http.Post;
                request.ProtocolVersion = HttpVersion.Version11;
                request.ContentType = "multipart/form-data; boundary=" + Boundary;
                request.ContentLength = encoded.Length;
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.KeepAlive = true;

                using (var stream = request.GetRequestStream())
                    stream.Write(encoded, 0, encoded.Length);

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                    message = sr.ReadToEnd();
                FileLogger.Info(LoggerName, "Publish response: " + message);

                if (message.StartsWith("OK"))
                    return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.Error(LoggerName, ex.Message, ex);
                throw;
            }
            return false;
        }

        #endregion

    }

}
