using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.Services;
using AmministrazioneTrasparente.WSSeries;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers;

namespace AmministrazioneTrasparente.MasterPages
{
    public partial class DocumentSeries : MasterPage
    {
        #region [ Fields ]

        private const string Endpointname = "WSSeriesEndpoint";
        private WSSeriesClient _client;
        private readonly ParameterService _parameterService = new ParameterService();

        #endregion

        #region [ Properties ]

        private static AnalyticsReportingService _googleReportingService;
        public AnalyticsReportingService GoogleReportingService
        {
            get
            {
                if (_googleReportingService == null)
                {
                    AuthenticateServiceAccount();
                }

                return _googleReportingService;
            }
        }

        public RadAjaxManager AjaxManager
        {
            get { return MyAjaxManager; }
        }

        public RadAjaxLoadingPanel AjaxLoadingPanel
        {
            get { return MyAjaxLoadingPanel; }
        }

        public string LogoLink
        {
            get { return ConfigurationManager.AppSettings["LogoLink"]; }
        }

        public string LogoImg
        {
            get
            {
                var temp = ConfigurationManager.AppSettings["LogoImg"];
                return string.IsNullOrEmpty(temp) ? "LogoAzienda.gif" : temp;
            }
        }

        public string GoogleAnalyticsCode
        {
            get { return this._parameterService.GetString("GoogleAnalyticsCode"); }
        }

        public string AnalyticsStartDate
        {
            get { return this._parameterService.GetString("AnalyticsStartDate"); }
        }
        public string AnalyticsIDView
        {
            get { return this._parameterService.GetString("AnalyticsIDView"); }
        }

        public string ThemeConfiguration
        {
            get { return this._parameterService.GetString("ThemeConfiguration"); }
        }

        public string HeaderExternalUrl
        {
            get { return this._parameterService.GetString("HeaderExternalUrl"); }
        }

        public string FooterExternalUrl
        {
            get { return this._parameterService.GetString("FooterExternalUrl"); }
        }

        public bool HistoryEnable
        {
            get { return this._parameterService.GetBoolean("HistoryEnable"); }
        }

        public int? ArchiveRestriction
        {
            get
            {
                string idArchive = this._parameterService.GetString("ArchiveRestriction");
                if (string.IsNullOrEmpty(idArchive))
                    return null;

                int.TryParse(idArchive, out int val);
                return val;
            }
        }

        public ResultDocumentSeriesFamilyWSO Families
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                var tor = cache["Families"] as ResultDocumentSeriesFamilyWSO;
                if (tor == null)
                {
                    var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) };
                    var xml = Client.GetFamilies(true, true, false, ArchiveRestriction);
                    var temp = SerializationHelper.SerializeFromString<ResultDocumentSeriesFamilyWSO>(xml);
                    tor = new ResultDocumentSeriesFamilyWSO();
                    var familiesToHide = ConfigurationManager.AppSettings["FamiliesToHide"];

                    tor.DocumentSeriesFamilies = new List<DocumentSeriesFamilyWSO>();
                    foreach (var f in temp.DocumentSeriesFamilies)
                    {
                        if (!familiesToHide.Contains(string.Format("|{0}|", f.Name)))
                        {
                            tor.DocumentSeriesFamilies.Add(f);
                        }
                    }

                    cache.Set("Families", tor, policy);
                }
                return tor;
            }
        }

        public DocumentSeriesFamilyWSO SelectedFamily
        {
            get
            {
                if (!SelectedIdFamily.HasValue) return null;

                foreach (var family in Families.DocumentSeriesFamilies)
                {
                    if (family.Id == SelectedIdFamily.Value)
                    {
                        return family;
                    }
                }

                return null;
            }
            set { SelectedIdFamily = value.Id; }
        }

        public int? SelectedIdFamily
        {
            get
            {
                var selected = Session["SelectedIdFamily"];
                if (selected != null)
                {
                    return ((int)selected);

                }
                return null;
            }
            set { Session["SelectedIdFamily"] = value; }
        }

        public WSSeriesClient Client
        {
            get
            {
                if (_client != null)
                {
                    if (_client.State == CommunicationState.Opened)
                    {
                        return _client;
                    }
                    _client.Close();
                    _client = null;
                }

                var endpointAddress = ConfigurationManager.AppSettings["WebServiceURI"];
                _client = new WSSeriesClient(Endpointname, new EndpointAddress(endpointAddress));
                return _client;
            }
        }

        public bool StoricoEnabled
        {
            get
            {
                var storicoEnabled = Request.QueryString["history"];
                if (storicoEnabled != null)
                {
                    return bool.Parse(storicoEnabled);
                }
                return false;
            }
        }

        public string CurrentHistoryUrl
        {
            get
            {
                NameValueCollection query = null;
                if (!string.IsNullOrEmpty(Request.Url.Query))
                {
                    query = HttpUtility.ParseQueryString(Request.Url.Query);
                    query.Remove("history");
                }
                return string.Concat(Request.Url.AbsolutePath, query != null ? string.Concat("?", query.ToString(), "&") : "?", "history=", !StoricoEnabled);
            }
        }

        #endregion

        #region [ Events ]

        protected void Page_Load(object sender, EventArgs e)
        {
            SetThemeConfiguration();
            if (!IsPostBack)
            {
                // Recupero elenco famiglie
                MainMenuRepeater.DataSource = Families.DocumentSeriesFamilies;
                MainMenuRepeater.DataBind();           
                lblArchiveType.Text = StoricoEnabled ? "Archivio corrente" : "Archivio storico";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.Header.DataBind();
        }

        #endregion

        #region [ Methods ]

        public DocumentSeriesFamilyWSO GetFamilyBySeries(int idSeries)
        {
            return
                Families.DocumentSeriesFamilies.FirstOrDefault(
                    family => family.DocumentSeries.Any(series => series.Id == idSeries));
        }

        protected bool IsSelected(int idFamily)
        {
            if (SelectedIdFamily.HasValue)
            {
                return SelectedIdFamily.Value == idFamily;
            }
            return false;
        }

        private void SetThemeConfiguration()
        {
            if (String.IsNullOrEmpty(ThemeConfiguration))
                return;

            var theme = new CustomTheme().GetThemes(ThemeConfiguration);
            var customThemes = theme as IList<CustomTheme> ?? theme.ToList();
            if (!customThemes.Any())
                return;
            foreach (var customTheme in customThemes.OrderBy(x => x.Priority))
            {
                var link = new CustomTheme().GetStylesheetLink(customTheme, ThemeConfiguration);
                Page.Header.Controls.Add(new LiteralControl(link));
            }
        }

        private void AuthenticateServiceAccount()
        {
            using (FileStream jwtFileStream = new FileStream(Server.MapPath("~/App_Data/AnalyticsJWTSecret.json"), FileMode.Open, FileAccess.Read))
            {
                string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly };
                GoogleCredential _googleCredential = GoogleCredential.FromStream(jwtFileStream).CreateScoped(scopes);

                _googleReportingService = new AnalyticsReportingService(new AnalyticsReportingService.Initializer()
                {
                    HttpClientInitializer = _googleCredential,
                    ApplicationName = "Amministrazione trasparente"
                });
            }
        }



        #endregion
    }
}