using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Net;
using System.Text;
using System;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Facade;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Services.CMVGroup.Models;

namespace VecompSoftware.DocSuiteWeb.Services.CMVGroup
{
    public class CMVGroup
    {
        #region [ Fields ]
        private string _boundary;
        private const string LoggerName = "CMVGroup";
        private CMVGroupParameters _parameters;
        private ResolutionFacade _reslFacade;
        #endregion

        #region [ Properties ]
        private string Boundary
        {
            get
            {
                if (string.IsNullOrEmpty(_boundary))
                {
                    _boundary = "--" + Guid.NewGuid().ToString("N");
                }
                return _boundary;
            }
        }

        private CMVGroupParameters Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    if (string.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.CmvGroupParameters))
                    {
                        throw new Exception($"CMVGroup -> configuration missing: {DocSuiteContext.Current.ResolutionEnv.CmvGroupParameters}");
                    }
                    _parameters = JsonConvert.DeserializeObject<CMVGroupParameters>(DocSuiteContext.Current.ResolutionEnv.CmvGroupParameters);
                }
                return _parameters;
            }
        }

        private ResolutionFacade ReslFacade
        {
            get
            {
                return _reslFacade ?? (_reslFacade = new ResolutionFacade());
            }
        }
        #endregion

        #region [ Methods ]
        public bool Publish(Resolution resolution, DocumentInfo document, out string message)
        {
            try
            {
                NewBoundary();
                FileLogger.Info(LoggerName, $"CMVGroup.Publish -> New boundary: {Boundary}");

                if (IsAlreadyPublished(resolution, out message))
                {
                    return false;
                }

                FileLogger.Info(LoggerName, $"CMVGroup.Publish -> Publication test result: {message}");
                string content = GetLegacyContent(resolution, document);
                FileLogger.Debug(LoggerName, content);
                byte[] encoded = Encoding.UTF8.GetBytes(content);
                FileLogger.Info(LoggerName, $"CMVGroup.Publish -> Publication test url: {Parameters.PostToRemoteUrl}");
                Uri uri = CreateUri(Parameters.PostToRemoteUrl);
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                request.Method = WebRequestMethods.Http.Post;
                request.ProtocolVersion = HttpVersion.Version11;
                request.ContentType = $"multipart/form-data; boundary={Boundary}";
                request.ContentLength = encoded.Length;
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.KeepAlive = true;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(encoded, 0, encoded.Length);
                }

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(stream))
                {
                    message = sr.ReadToEnd();
                }

                FileLogger.Info(LoggerName, $"CMVGroup.Publish -> Publish response: {message}");
                return message.StartsWith("OK", StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.Error(LoggerName, ex.Message, ex);
                throw;
            }
        }

        private string GetLegacyContent(Resolution resolution, DocumentInfo document)
        {
            StringBuilder sb = new StringBuilder();
            AppendFormattedMetadata(sb, "OGGETTO", resolution.ResolutionObject, true);
            AppendFormattedMetadata(sb, "TITOLO", resolution.Note);
            AppendFormattedMetadata(sb, "NUMERO", resolution.Number.ToString());
            AppendFormattedMetadata(sb, "ANNO", resolution.Year.ToString());
            string codiceDelibera = ReslFacade.GetResolutionNumber(resolution);
            AppendFormattedMetadata(sb, "CODICE_DELIBERA", codiceDelibera);
            AppendFormattedMetadata(sb, "DATA_INSERIMENTO", resolution.ProposeDate);
            AppendFormattedMetadata(sb, "DATA_ADOZIONE", resolution.AdoptionDate);
            AppendFormattedMetadata(sb, "DATA_PUBBLICAZIONE", resolution.PublishingDate);
            DateTime dataEsecutivita = resolution.EffectivenessDate ?? resolution.PublishingDate.Value.AddDays(10);
            AppendFormattedMetadata(sb, "DATA_ESECUTIVITA", dataEsecutivita);
            DateTime dataScadenza = resolution.PublishingDate.Value.AddDays(DocSuiteContext.Current.ResolutionEnv.PublicationEndDays - 5);
            AppendFormattedMetadata(sb, "DATA_SCADENZA", dataScadenza);
            string decodedType = resolution.Type.Id == 0 ? "Determina" : "Delibera";
            AppendFormattedMetadata(sb, "TIPO", decodedType);
            AppendDocumentMetadata(sb, "DOCUMENTO", document);
            sb.Append("--" + Boundary);
            return sb.ToString();
        }

        private void AppendFormattedMetadata(StringBuilder sb, string name, string value, bool base64 = false)
        {
            sb.AppendLine("--" + Boundary);
            sb.AppendFormat("Content-Disposition: form-data; name='{0}'{1}", name, Environment.NewLine);

            if (base64)
            {
                sb.AppendLine("Content-Transfer-Encoding: base64;");
                value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }
            sb.AppendLine();
            sb.AppendLine(value);
        }

        private void AppendFormattedMetadata(StringBuilder sb, string name, DateTime? value)
        {
            string formattedValue = value.HasValue ? value.Value.ToString("dd/MM/yyyy") : string.Empty;
            AppendFormattedMetadata(sb, name, formattedValue);
        }

        private void AppendDocumentMetadata(StringBuilder sb, string name, DocumentInfo document)
        {
            sb.AppendLine("--" + Boundary);
            sb.AppendFormat("Content-Disposition: form-data; name='{0}'; filename='{1}';{2}", name, document.Name, Environment.NewLine);
            sb.AppendLine("Content-Type: application/pdf;");
            sb.AppendLine("Content-Transfer-Encoding: base64;");
            sb.AppendLine();
            sb.AppendLine(Convert.ToBase64String(document.Stream));
        }

        private void NewBoundary()
        {
            _boundary = null;
        }

        private bool IsAlreadyPublished(Resolution resolution, out string message)
        {
            if (Parameters.PostToRemoteDisablePublicationTester)
            {
                message = "SKIPPED";
                return false;
            }

            string uriString = "{0}?ANNO={1}&NUMERO={2}&TIPO={3}&DATA_ADOZIONE={4:dd/MM/yyyy}";
            string decodedType = resolution.Type.Id == 0 ? "Determina" : "Delibera";
            uriString = string.Format(uriString, Parameters.PostToRemotePublicationTesterUrl,
                resolution.Year, resolution.Number, decodedType, resolution.AdoptionDate);
            FileLogger.Info(LoggerName, $"CMVGroup.Publish -> Publication test url: {uriString}");
            Uri uri = CreateUri(uriString);

            try
            {
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                request.Method = WebRequestMethods.Http.Get;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;

                using (WebResponse response = request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(stream))
                {
                    message = sr.ReadToEnd();
                }

                FileLogger.Info(LoggerName, $"CMVGroup.Publish -> isAlreadyPublished response: {message}");

                if (message == "KO: dati mancanti")
                {
                    throw new InvalidOperationException($"CMVGroup.Publish -> Insufficient parameters to make the request: " + message);
                }
                else if (message.StartsWith("OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.Error(LoggerName, ex.Message, ex);
                throw;
            }
            return false;
        }

        private Uri CreateUri(string uriString)
        {
            Uri uri = new Uri(uriString);
            List<string> valid = new List<string> { Uri.UriSchemeHttp, Uri.UriSchemeHttps };
            return valid.Any(s => s == uri.Scheme) ? uri : throw new InvalidCastException($"CMVGroup.Publish -> Invalid uri for path: " + uriString);
        }
        #endregion
    }
}
