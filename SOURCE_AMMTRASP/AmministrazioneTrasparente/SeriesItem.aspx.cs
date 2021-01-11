using System;
using System.Linq;
using VecompSoftware.Helpers;
using AmministrazioneTrasparente.Tools;
using VecompSoftware.Services.Logging;
using AmministrazioneTrasparente.Services;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using VecompSoftware.Helpers.ExtensionMethods;
using System.Web;
using Google.Apis.AnalyticsReporting.v4.Data;
using System.Collections.Generic;

namespace AmministrazioneTrasparente
{
    public partial class SeriesItem : BasePage
    {
        #region [ Fields ]

        private ResultArchiveAttributeWSO _structure;
        private DocumentSeriesWSO _seriesWso;
        private DocumentSeriesItemWSO _itemWso;
        private readonly ParameterService _parameterService = new ParameterService();

        #endregion

        #region [ Properties ]

        protected int IdParentSeries
        {
            get { return ItemWso.IdDocumentSeries; }
        }

        protected string IdSeriesItem
        {
            get
            {
                if (ViewState["IdSeriesItem"] == null)
                {
                    ViewState["IdSeriesItem"] = Request["IdSeriesItem"];
                }
                return ViewState["IdSeriesItem"] as string;
            }
        }

        protected DocumentSeriesItemWSO ItemWso
        {
            get
            {
                if (_itemWso == null)
                {
                    if (string.IsNullOrEmpty(IdSeriesItem))
                    {
                        throw new Exception("Nessuna serie selezionata");
                    }
                    var serialized = MyMaster.Client.ConsultationNew(Convert.ToInt32(IdSeriesItem), true, true, false, false, !MyMaster.StoricoEnabled);
                    _itemWso = SerializationHelper.SerializeFromString<DocumentSeriesItemWSO>(serialized);
                    MyMaster.Client.Close();
                }
                return _itemWso;
            }
        }

        protected DocumentSeriesWSO SeriesWso
        {
            get
            {
                if (_seriesWso == null)
                {
                    var temp = MyMaster.Client.GetDocumentSeries(ItemWso.IdDocumentSeries, false, ArchiveRestriction);
                    _seriesWso = SerializationHelper.SerializeFromString<DocumentSeriesWSO>(temp);
                    MyMaster.Client.Close();
                }
                return _seriesWso;
            }
        }
        private string SerializedStructure
        {
            get
            {
                if (string.IsNullOrEmpty(ViewState["SerializedStructure"] as string))
                {
                    ViewState["SerializedStructure"] = MyMaster.Client.GetDynamicData(ItemWso.IdDocumentSeries);
                    MyMaster.Client.Close();
                }
                return ViewState["SerializedStructure"] as string;
            }
        }

        protected ResultArchiveAttributeWSO Structure
        {
            get
            {
                return _structure ??
                       (_structure =
                        SerializationHelper.SerializeFromString<ResultArchiveAttributeWSO>(SerializedStructure));
            }
        }

        protected bool ViewDocumentFullName
        {
            get { return this._parameterService.GetBoolean("ViewDocumentFullName"); }
        }

        protected bool EnableHeaderComFixing
        {
            get { return this._parameterService.GetBoolean("EnableHeaderComFixing"); }
        }


        #endregion

        #region [ Events ]

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }

        #endregion

        #region [ Methods ]

        private void Initialize()
        {
            if (!MyMaster.StoricoEnabled && DateTime.Today >= ItemWso.RetireDate.GetValueOrDefault(DateTime.MaxValue))
            {
                FileLogger.Error(LoggerName, String.Format("La registrazione [{0}] risulta ritirata il giorno {1}", ItemWso.Id, ItemWso.RetireDate.Value.ToString("dd/MM/yyyy")));
                Response.Redirect("~/");
            }

            MyMaster.SelectedFamily = MyMaster.GetFamilyBySeries(SeriesWso.Id);

            lblHeader.Text = SeriesWso.Name;

            lblYear.Text = ItemWso.Year.ToString();

            tdSubject.InnerHtml = ItemWso.Subject.UrlToAnchor();

            if (ItemWso.PublishingDate != null)
                lblPublishingDate.Text = ItemWso.PublishingDate.Value.ToString("dd/MM/yyyy");

            if (ItemWso.LastChangedDate.HasValue)
            {
                lblLastChangedDate.Text = ItemWso.LastChangedDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                lblLastChangedDate.Text = ItemWso.PublishingDate.Value.ToString("dd/MM/yyyy");
            }

            if (ItemWso.RetireDate.HasValue)
                lblRetireDate.Text = ItemWso.RetireDate.Value.ToString("dd/MM/yyyy");

            if (!String.IsNullOrEmpty(MyMaster.GoogleAnalyticsCode))
            {
                AnalyticsReportService reportService = new AnalyticsReportService(MyMaster.GoogleReportingService);
                var dimensions = new List<Dimension> { new Dimension { Name = "ga:pagePath" } };
                var metrics = new List<Metric> { new Metric { Expression = "ga:pageviews" } };

                lblAnalyticsCounter.Visible = true;
                lblAnalyticsCounter.Text = "Numero di visite: " + reportService.GetVisitorsCount(HttpContext.Current.Request.Url.PathAndQuery,
                                                                                                 MyMaster.AnalyticsStartDate,
                                                                                                 MyMaster.AnalyticsIDView,
                                                                                                 dimensions,
                                                                                                 metrics);
            }

            DynamicRows.DataSource = ItemWso.DynamicData;
            DynamicRows.DataBind();

            DocumentsRepeater.DataSource = ItemWso.MainDocs;
            DocumentsRepeater.DataBind();

            AnnexedRepeater.DataSource = ItemWso.AnnexedDocs;
            AnnexedRepeater.DataBind();

            pnlHeaderAnnexed.Visible = false;
            if (DocumentsHeaderLabel.Count > 0)
            {
                if (DocumentsHeaderLabel.ContainsKey("MainChain"))
                {
                    lblMainDocuments.Text = DocumentsHeaderLabel["MainChain"];
                }

                if (DocumentsHeaderLabel.ContainsKey("AnnexedChain") && !ItemWso.AnnexedDocs.IsNullOrEmpty())
                {
                    pnlHeaderAnnexed.Visible = true;
                    lblAnnexed.Text = DocumentsHeaderLabel["AnnexedChain"];
                }
            }

            if (ItemWso.MainDocs.Count == 0 && ItemWso.AnnexedDocs.Count == 0)
            {
                DocumentsPanel.Visible = false;
            }

            RadScriptBlock.Visible = EnableHeaderComFixing;
        }

        protected string GetAttributeDescription(string key)
        {
            var temp = Structure.ArchiveAttributes.SingleOrDefault(a => a.Name == key);
            if (temp != null)
            {
                return temp.Description;
            }
            return key;
        }

        #endregion
    }
}