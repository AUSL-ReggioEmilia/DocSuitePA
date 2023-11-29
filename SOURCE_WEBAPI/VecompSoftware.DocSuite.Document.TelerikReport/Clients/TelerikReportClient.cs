using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Document.TelerikReport.Verbs;

namespace VecompSoftware.DocSuite.Document.TelerikReport.Clients
{
    public class TelerikReportClient : ITelerikReportClient
    {
        #region [ Fields ] 
        public const string _uri_baseUrl = "http://bee-ws-87:83/";
        public static string _uri_serverApi = $"{_uri_baseUrl}api/reportserver/v2/";
        public static string _uri_allCategories = $"{_uri_baseUrl}api/reportserver/categories";
        public static string _uri_formats = $"{_uri_baseUrl}api/reports/formats";
        public static string _uri_documents = $"{_uri_serverApi}documents";
        public static string _uri_token = $"{_uri_baseUrl}Token";

        private const string Username = "Culai Chiritoiu";
        private const string Password = "parola123";

        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public TelerikReportClient()
        {
        }
        #endregion

        #region [ Methods ]
        public async Task<ICollection<T>> GetAsync<T>(GetAction action, string parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(TransformGetAction(action, parameter));
                ICollection<T> result = JsonConvert.DeserializeObject<ICollection<T>>(json);
                return result;
            }
        }

        public async Task<byte[]> PostAsync<T>(PostAction action, T data)
        {
            using (HttpClient client = new HttpClient())
            {
                await AuthenticateRequest(client);
                string dataToConvert = JsonConvert.SerializeObject(data);
                HttpContent content = new StringContent(dataToConvert, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponseMessage = await client.PostAsync(TransformPostAction(action, string.Empty), content);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Invalid report parameters");
                }
                string responseResult = JsonConvert.DeserializeObject<string>(await httpResponseMessage.Content.ReadAsStringAsync());
                return await client.GetByteArrayAsync(TransformPostAction(PostAction.GenerateReportDocument, responseResult));
            }
        }


        private string TransformPostAction(PostAction action, string parameter)
        {
            switch (action)
            {
                case PostAction.GenerateDocument:
                    {
                        return _uri_documents;
                    }
                case PostAction.GenerateReportDocument:
                    {
                        return $"{_uri_documents}/{parameter}?content-disposition=attachment";
                    }
                default:
                    return string.Empty;
            }
        }

        private string TransformGetAction(GetAction action, string parameter)
        {
            switch (action)
            {
                case GetAction.Categories:
                    {
                        return _uri_allCategories;
                    }
                case GetAction.Reports:
                    {
                        return $"{_uri_allCategories}/{parameter}/reports";
                    }
                case GetAction.Parameters:
                    {
                        return $"{_uri_serverApi}reports/{parameter}/parameters";
                    }
                case GetAction.Formats:
                    {
                        return _uri_formats;
                    }
                default:
                    return string.Empty;
            }
        }

        private async Task AuthenticateRequest(HttpClient httpClient)
        {
            using (HttpClient client = new HttpClient())
            {
                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username",Username),
                    new KeyValuePair<string, string>("password",Password),
                    new KeyValuePair<string, string>("grant_type","password"),
                };
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _uri_token)
                {
                    Content = new FormUrlEncodedContent(keyValues)
                };
                HttpResponseMessage response = await client.SendAsync(request);
                string jwt = await response.Content.ReadAsStringAsync();
                JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(jwt);
                string accessToken = jwtDynamic.Value<string>("access_token");
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }
        }

        #endregion
    }
}
