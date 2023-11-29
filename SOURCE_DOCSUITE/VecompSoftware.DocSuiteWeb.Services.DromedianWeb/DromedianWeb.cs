using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.DromedianWeb.Models;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.DromedianWeb
{
    public class DromedianWeb
    {
        #region [ Constants ]
        private const string LoggerName = "DromedianWeb";
        private const string _authentication_header_scheme = "Bearer";
        #endregion

        #region [ Fields ]
        private ResolutionFacade _reslFacade;
        private DromedianWebParameters _parameters;
        #endregion

        #region [ Properties ]
        private ResolutionFacade ReslFacade
        {
            get
            {
                return _reslFacade ?? (_reslFacade = new ResolutionFacade());
            }
        }
        private DromedianWebParameters Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    if (string.IsNullOrEmpty(DocSuiteContext.Current.ResolutionEnv.DromedianWebParameters))
                    {
                        throw new Exception($"DromedianWeb -> configuration missing: {DocSuiteContext.Current.ResolutionEnv.DromedianWebParameters}");
                    }
                    _parameters = JsonConvert.DeserializeObject<DromedianWebParameters>(DocSuiteContext.Current.ResolutionEnv.DromedianWebParameters);
                }
                return _parameters;
            }
        }
        #endregion

        #region [ Methods ]
        public bool Publish(Resolution resolution, DocumentInfo document, out string message)
        {
            FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Start publishing resolution with id: {resolution.Id.ToString(CultureInfo.InvariantCulture)}");
            message = string.Empty;
            try
            {
                return Task.Run(() => Publish(resolution, document)).Result;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.Error(LoggerName, ex.Message, ex);
                throw;
            }
        }

        private async Task<bool> Publish(Resolution resolution, DocumentInfo document)
        {
            using (HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(Parameters.BaseUrl) })
            {
                FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Requesting authorization token...");
                HttpResponseMessage requestToken = await httpClient.PostAsync(new Uri($"{Parameters.RequestTokenUrl}?username={Parameters.Username}&password={Parameters.Password}", UriKind.Relative), null);
                (RequestTokenResponseModel requestTokenResponse, string stringResponseToken) = await GetResponse<RequestTokenResponseModel>(requestToken, Parameters.RequestTokenUrl);
                if (string.IsNullOrEmpty(requestTokenResponse.Token))
                {
                    FileLogger.Error(LoggerName, $"DromedianWeb.Publish -> Request was successful but authorization token in response was null or empty. Response content: {stringResponseToken}");
                    throw new Exception($"DromedianWeb.Publish -> Request was successful but authorization token in response was null or empty");
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authentication_header_scheme, requestTokenResponse.Token);

                FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Publishing resolution...");
                HttpResponseMessage publishResolution = await httpClient.PostAsync(new Uri(Parameters.PublishResolutionUrl, UriKind.Relative), GetContent(resolution, document));
                (PublishResolutionResponseModel publishResolutionResponse, string stringResponsePublish) = await GetResponse<PublishResolutionResponseModel>(publishResolution, Parameters.PublishResolutionUrl);

                if (!publishResolutionResponse.Response)
                {
                    FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Request was successful but publishing failed with error: {publishResolutionResponse.Error}. Response content: {stringResponsePublish}");
                    throw new Exception($"DromedianWeb.Publish -> Request was successful but publishing failed with error: {publishResolutionResponse.Error}.");
                }

                FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Resolution published successfully. Status code {publishResolution.StatusCode}. Response content: {stringResponsePublish}");
                return true;
            }
        }

        private async Task<(T, string)> GetResponse<T>(HttpResponseMessage request, string endpoint)
        {
            string stringResponse = await request.Content.ReadAsStringAsync();
            if (!request.IsSuccessStatusCode)
            {
                FileLogger.Error(LoggerName, $"DromedianWeb.Publish -> External web api failed while calling {endpoint} endpoint. Status code: {request.StatusCode}. Response content: {stringResponse}");
                throw new Exception($"DromedianWeb.Publish -> External web api failed while calling {endpoint} endpoint. Status code: {request.StatusCode}");
            }
            if (string.IsNullOrEmpty(stringResponse))
            {
                FileLogger.Error(LoggerName, $"DromedianWeb.Publish -> External web api returned an empty response while calling {endpoint} endpoint");
                throw new Exception($"DromedianWeb.Publish -> External web api returned an empty response while calling {endpoint} endpoint");
            }
            T response = JsonConvert.DeserializeObject<T>(stringResponse);
            if (response == null)
            {
                FileLogger.Error(LoggerName, $"DromedianWeb.Publish -> External web api was successful while calling {endpoint} endpoint but returned an unexpected result. Status code {request.StatusCode}. Response content: {stringResponse}");
                throw new Exception($"DromedianWeb.Publish -> External web api was successful while calling {endpoint} endpoint but returned an unexpected result. Status code {request.StatusCode}");
            }
            return (response, stringResponse);
        }

        private MultipartFormDataContent GetContent(Resolution resolution, DocumentInfo document)
        {
            MultipartFormDataContent requestContent = new MultipartFormDataContent();
            string dataAdozione = resolution.AdoptionDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            string tipoAtto = resolution.Type.Id == 0 ? "Determinazione" : "Deliberazione";
            string codiceAtto = ReslFacade.GetResolutionNumber(resolution);
            string titolo = $"{tipoAtto} {codiceAtto} del {dataAdozione}";
            AddParameter(requestContent, "oggetto", resolution.ResolutionObject);
            AddParameter(requestContent, "titolo", titolo);
            AddParameter(requestContent, "numero", resolution.Number.ToString());
            AddParameter(requestContent, "anno", resolution.Year.ToString());
            AddParameter(requestContent, "codice_atto", codiceAtto);
            AddParameter(requestContent, "data_inserimento", resolution.ProposeDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            AddParameter(requestContent, "data_adozione", dataAdozione);
            AddParameter(requestContent, "data_pubblicazione", resolution.PublishingDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            string dataEsecutivita = resolution.EffectivenessDate.GetValueOrDefault(resolution.PublishingDate.Value.AddDays(10)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            AddParameter(requestContent, "data_esecutivita", dataEsecutivita);
            string dataScadenza = resolution.PublishingDate.Value.AddDays(DocSuiteContext.Current.ResolutionEnv.PublicationEndDays - 5).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            AddParameter(requestContent, "data_scadenza", dataScadenza);
            AddParameter(requestContent, "categoria_atto", resolution.Type.Id == 0 ? "determinazioni dirigenziali" : "deliberazioni");
            AddDocumentParameter(requestContent, "allegato", document.Stream);
            return requestContent;
        }

        public void AddParameter(MultipartFormDataContent requestContent, string key, string value)
        {
            FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Adding parameter {key} with value {value}");
            requestContent.Add(new StringContent(value), key);
        }

        public void AddDocumentParameter(MultipartFormDataContent requestContent, string key, byte[] content)
        {
            FileLogger.Info(LoggerName, $"DromedianWeb.Publish -> Adding document parameter {key} with length {content.Length}");
            requestContent.Add(new StringContent(Convert.ToBase64String(content)), key);
        }
        #endregion
    }
}
