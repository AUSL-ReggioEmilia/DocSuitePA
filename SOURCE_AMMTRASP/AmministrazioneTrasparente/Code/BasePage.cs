using System;
using System.Configuration;
using System.Web.UI;
using AmministrazioneTrasparente.MasterPages;
using VecompSoftware.Services.Logging;
using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.Services;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AmministrazioneTrasparente
{
    public class BasePage : Page
    {
        #region [ Fields ]

        public const string LoggerName = "Application";
        public const int DefaultElementForPage = 10;
        private readonly ParameterService _parameterService;

        #endregion

        #region [ Constructor ]
        public BasePage()
        {
            this._parameterService = new ParameterService();
        }
        #endregion

        #region [ Properties ]

        public DocumentSeries MyMaster
        {
            get { return Master as DocumentSeries; }
        }

        public int ElementForPage
        {
            get
            {
                int elementForPage;
                return Int32.TryParse(ConfigurationManager.AppSettings["ElementForPage"], out elementForPage)
                    ? elementForPage
                    : DefaultElementForPage;
            }
        }

        public int CacheExpiration
        {
            get
            {
                int temp;
                return Int32.TryParse(ConfigurationManager.AppSettings["CacheExpiration"], out temp) ? temp : 0;
            }
        }

        public bool DynamicColumnsInPriorityGrid
        {
            get
            {
                bool tor;
                return Boolean.TryParse(ConfigurationManager.AppSettings["DynamicColumnsInPriorityGrid"], out tor) &&
                       tor;
            }
        }

        public bool DynamicColumnsInGrid
        {
            get
            {
                bool tor;
                return Boolean.TryParse(ConfigurationManager.AppSettings["DynamicColumnsInGrid"], out tor) && tor;
            }
        }

        public string ContactMail
        {
            get { return ConfigurationManager.AppSettings["ContactMail"]; }
        }

        public static string ConfiguredTitle
        {
            get { return ConfigurationManager.AppSettings["Title"]; }
        }

        public bool OneDocumentSeriesItemEnable
        {
            get { return this._parameterService.GetBoolean("OneDocumentSeriesItemEnable"); }
        }

        public bool SimpleSearchEnable
        {
            get { return this._parameterService.GetBoolean("SimpleSearchEnable"); }
        }

        public IDictionary<string, string> DocumentsHeaderLabel
        {
            get
            {
                string json = this._parameterService.GetString("DocumentsHeaderLabel");
                if (string.IsNullOrEmpty(json))
                    return new Dictionary<string, string>();

                return JsonConvert.DeserializeObject<IDictionary<string, string>>(json);
            }
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

        public bool SeriesResultsByConstraintEnabled
        {
            get { return this._parameterService.GetBoolean("SeriesResultsByConstraintEnabled"); }
        }

        #endregion

        #region [ Events ]

        protected override void OnLoad(EventArgs e)
        {
            FileLogger.Debug(LoggerName, string.Format("Request: {0}", Request.RawUrl));

            if (!Page.IsPostBack)
            {
                Title = string.IsNullOrWhiteSpace(ConfiguredTitle) ? "Amministrazione Trasparente" : ConfiguredTitle;
            }

            base.OnLoad(e);
        }

        protected override void OnError(EventArgs e)
        {
            var ex = Server.GetLastError();
            FileLogger.Error(LoggerName, "Errore non gestito.", ex);

            base.OnError(e);
        }

        #endregion
    }
}