using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using AmministrazioneTrasparente.Code;
using VecompSoftware.Helpers;
using VecompSoftware.Services.Logging;
using AmministrazioneTrasparente.Services;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente
{
    public partial class Family : BasePage
    {
        private readonly ParameterService _parameterService = new ParameterService();

        #region [ Properties ]
        protected int IdFamily
        {
            get { return int.Parse(Request.QueryString["idFamily"]); }
        }

        protected DocumentSeriesFamilyWSO FamilyWSO
        {
            get
            {
                if (Cache[string.Concat("FamilyWSO_", IdFamily)] == null)
                {
                    var temp = MyMaster.Client.GetFamily(IdFamily, true, true, ArchiveRestriction);
                    Cache.Insert(string.Concat("FamilyWSO_", IdFamily), SerializationHelper.SerializeFromString<DocumentSeriesFamilyWSO>(temp), null, DateTime.Now.AddMinutes(CacheExpiration), TimeSpan.Zero);
                    MyMaster.Client.Close();
                }
                return (DocumentSeriesFamilyWSO)Cache[string.Concat("FamilyWSO_", IdFamily)];
            }
        }

        public bool SkipSingleSeriesList
        {
            get
            {
                bool tor;
                return bool.TryParse(ConfigurationManager.AppSettings["SkipSingleSeriesList"], out tor) && tor;
            }
        }
        #endregion

        #region [ Events ]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            MyMaster.SelectedIdFamily = IdFamily;

            // Se la Family ha un'unica serie faccio il Redirect diretto alla Serie
            if (SkipSingleSeriesList && FamilyWSO.DocumentSeries.Count == 1)
            {
                FileLogger.Debug(LoggerName, "Redirect per Family con una sola Serie Documentale.");
                if (FamilyWSO.DocumentSeries[0].DocumentSeriesSubsections.Count == 0)
                {
                    Response.Redirect(ResolveUrl(string.Concat("~/Series.aspx?idSeries=", FamilyWSO.DocumentSeries[0].Id, (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : ""))), true);
                }
                else
                {
                    Response.Redirect(ResolveUrl(string.Concat("~/SubSectionSelection.aspx?idSeries=" + FamilyWSO.DocumentSeries[0].Id, (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : ""))), true);
                }
                return;
            }

            lblFamilyName.Text = FamilyWSO.Name;

            SubMenuRepeater.DataSource = FamilyWSO.DocumentSeries;
            SubMenuRepeater.DataBind();

            InitializeCustomLinks();
        }
        #endregion

        #region Methods

        private void InitializeCustomLinks()
        {
            var links = new List<CustomFamilyLinks>();
            var familyParam = this._parameterService.GetString("CustomFamilyLink");

            if(String.IsNullOrEmpty(familyParam))
                return;

            var parameters = familyParam.Split('|');
            foreach (var item in parameters)
            {
                var content = Regex.Match(item, @"(?<=\{).*(?=\})").Value;
                var model = content.Split(',');
                if(model.Count()<2)
                    throw new Exception("Parametro CustomFamilyLink con valori non corretti.");

                var idFamily = int.Parse(model[0]);
                var text = model[1];
                var url = model[2];

                if(idFamily != IdFamily)
                    continue;

                links.Add(new CustomFamilyLinks(){IdFamily = idFamily, Text = text, Url = url});
            }

            if (!links.Any()) 
                return;

            customFamilyLinksRepeater.DataSource = links;
            customFamilyLinksRepeater.DataBind();
        }
        #endregion
    }
}