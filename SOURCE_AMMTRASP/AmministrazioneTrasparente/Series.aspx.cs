using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using AmministrazioneTrasparente.Code;
using Telerik.Web.UI;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.Web.ExtensionMethods;
using System.Linq;
using System.Text;
using AmministrazioneTrasparente.Tools;
using AmministrazioneTrasparente.Services;
using Newtonsoft.Json;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using System.Web;
using Google.Apis.AnalyticsReporting.v4.Data;
using AmministrazioneTrasparente.Models;

namespace AmministrazioneTrasparente
{
    public partial class Series : BaseSeriesPage
    {
        #region [ Fields ]
        private const string ViewSeriePriority = "Priority";
        private const string ViewSerieLatestSeries = "LatestSeries";

        private readonly ParameterService _parameterService = new ParameterService();

        #endregion


        #region [ Properties ]

        public DocumentSeriesItemFinderWSO BaseFinder
        {
            get
            {
                // Inizializzo il finder
                DocumentSeriesItemFinderWSO finder = new DocumentSeriesItemFinderWSO
                {
                    EnablePaging = true,
                    Skip = 0,
                    IsPublished = true,
                    IsRetired = false,
                    Take = 10
                };
                if (IdSubSection.HasValue)
                {
                    finder.IdDocumentSeriesSubsections = new List<int>() { IdSubSection.Value };
                }
                // Leggo l'utente da utilizzare per la ricerca degli item
                var impersonatingUser = ConfigurationManager.AppSettings["ImpersonatingUser"];
                if (string.IsNullOrEmpty(impersonatingUser))
                {
                    throw new Exception("Inserire in appsettings l'impersonatingUser");
                }
                finder.ImpersonatingUser = impersonatingUser;

                finder.IdDocumentSeries = IdSeries;

                return finder;
            }
        }

        protected string CsvFileName
        {
            get
            {
                if (IdSubSection.HasValue)
                {
                    return string.Format("Export_Subsection_{0}.csv", IdSubSection);
                }
                else
                {
                    return string.Format("Export_{0}.csv", IdSeries);
                }
            }
        }
        protected Boolean ShowAllItems
        {
            get
            {
                return this._parameterService.GetBoolean("ShowAllItems");
            }
        }
        public DocumentSeriesItemFinderWSO PriorityFinder
        {
            get
            {
                var finder = BaseFinder;
                finder.IsPriority = true;
                return finder;
            }
        }

        public DocumentSeriesItemFinderWSO LatestSeriesFinder
        {
            get
            {
                var finder = BaseFinder;
                finder.Take = LatestSeriesGridElement;
                finder.LastModifiedSortingView = true;
                return finder;
            }
        }

        public DocumentSeriesItemFinderWSO Finder
        {
            get
            {
                // Inizializzo il finder
                var finder = BaseFinder;

                if (inYear.Value.HasValue)
                    finder.Year = (int)inYear.Value.Value;

                if (!string.IsNullOrEmpty(inSubject.Text))
                    finder.SubjectContains = inSubject.Text;

                finder.PublishingDateFrom = inDateFrom.UnsafeSelectedDate();

                finder.PublishingDateTo = inDateTo.UnsafeSelectedDate();

                var attribute = new List<AttributeWSO>();
                foreach (var dynamicData in Structure.ArchiveAttributes)
                {
                    var c = dynamicDataPlaceHolder.FindControlRecursive(string.Format("DYNAMIC_{0}", dynamicData.Name));
                    var val = GetControlValue(c);
                    if (val != null)
                    {
                        attribute.Add(new AttributeWSO
                        {
                            Key = dynamicData.Name,
                            Value = val.ToString(),
                            Operator = "Contains"
                        });

                    }

                }

                finder.DynamicData = attribute;

                return finder;

            }
        }

        private string SerializedPriorityResult
        {
            get
            {
                if (ViewState["SerializedPriorityResult"] == null)
                {
                    var xmlFinder = SerializationHelper.SerializeToString(PriorityFinder);
                    var serializedResult = MyMaster.StoricoEnabled ? MyMaster.Client.SearchRetired(xmlFinder, true) : MyMaster.Client.Search(xmlFinder, true);
                    MyMaster.Client.Close();
                    ViewState["SerializedPriorityResult"] = serializedResult;

                }
                return (string)ViewState["SerializedPriorityResult"];
            }
        }
        private int PrioritySeriesResultCount
        {
            get
            {
                if (ViewState["PrioritySeriesResultCount"] == null)
                {
                    var xmlFinder = SerializationHelper.SerializeToString(PriorityFinder);
                    int itemscount = MyMaster.StoricoEnabled ? MyMaster.Client.SearchCountRetired(xmlFinder) : MyMaster.Client.SearchCount(xmlFinder);
                    MyMaster.Client.Close();
                    ViewState["PrioritySeriesResultCount"] = itemscount;

                }
                return (int)ViewState["PrioritySeriesResultCount"];
            }
        }

        private string SerializedLatestSeriesResult
        {
            get
            {
                if (ViewState["SerializedLatestSeriesResult"] == null)
                {
                    var xmlFinder = SerializationHelper.SerializeToString(LatestSeriesFinder);
                    var serializedResult = MyMaster.StoricoEnabled ? MyMaster.Client.SearchRetired(xmlFinder, true) : MyMaster.Client.Search(xmlFinder, true);
                    MyMaster.Client.Close();
                    ViewState["SerializedLatestSeriesResult"] = serializedResult;
                }
                return (string)ViewState["SerializedLatestSeriesResult"];
            }
        }
        private int TotalSeriesResultCount
        {
            get
            {
                if (ViewState["TotalSeriesResultCount"] == null)
                {
                    var xmlFinder = SerializationHelper.SerializeToString(Finder);
                    int itemscount = MyMaster.StoricoEnabled ? MyMaster.Client.SearchCountRetired(xmlFinder) : MyMaster.Client.SearchCount(xmlFinder);
                    MyMaster.Client.Close();
                    ViewState["TotalSeriesResultCount"] = itemscount;
                }
                return (int)ViewState["TotalSeriesResultCount"];
            }
        }
        private int NumerOfItems(int nitems)
        {

            int numerOfItems = 0;
            if (ShowAllItems)
            {
                numerOfItems = TotalSeriesResultCount;
            }
            else
            { numerOfItems = nitems; }
            return numerOfItems;

        }

        public bool ShowSeriesCountDetails
        {
            get { return this._parameterService.GetBoolean("ShowSeriesCountDetails"); }
        }
        public string DefaultViewSeries
        {
            get { return this._parameterService.GetString("DefaultViewSeries"); }
        }
        public SeriesPreviewConfigurationModel SeriesPreviewConfiguration
        {
            get
            {
                string json = this._parameterService.GetString("SeriesPreviewBehaviours");
                if (string.IsNullOrEmpty(json))
                    return new SeriesPreviewConfigurationModel();

                return JsonConvert.DeserializeObject<SeriesPreviewConfigurationModel>(json);
            }
        }

        public int LatestSeriesGridElement
        {
            get
            {
                int element = this._parameterService.GetInteger("LatestSeriesGridElement");
                if (element == 0)
                    return 10;
                return element;
            }
        }

        #endregion

        #region [ Events ]

        protected override void OnInit(EventArgs e)
        {
            RadGrid grid;
            if (MyMaster.HistoryEnable)
            {
                grid = DynamicColumnsInPriorityGrid ? DocumentSeriesRadGrid.NewGrid(Structure, MyMaster.StoricoEnabled) : DocumentSeriesRadGrid.NewGrid(null, MyMaster.StoricoEnabled);
            }
            else
            {
                grid = DynamicColumnsInPriorityGrid ? DocumentSeriesRadGrid.NewGrid(Structure) : DocumentSeriesRadGrid.NewGrid();
            }

            grid.ClientSettings.Scrolling.AllowScroll = true;
            grid.ClientSettings.Scrolling.UseStaticHeaders = false;
            grid.AllowPaging = ShowAllItems;
            grid.NeedDataSource += GridNeedDataSource;

            GridPlaceHolder.Controls.Add(grid);
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Initialize();
                InitCsvDocument();
            }

            foreach (var dynamicData in Structure.ArchiveAttributes)
            {
                var tr = new TableRow();
                var tdLabel = new TableCell { CssClass = "td-label" };
                tdLabel.Text = dynamicData.Description;
                var tdField = new TableCell { CssClass = "td-field" };
                tr.Cells.Add(tdLabel);

                if (dynamicData.DataType == "System.DataTime")
                {
                    var control = new RadDatePicker
                    {
                        ID = string.Format("DYNAMIC_{0}", dynamicData.Name),
                        CssClass = "form-control input-sm"
                    };
                    tdField.Controls.Add(control);
                }
                else
                {
                    var control = new RadTextBox();
                    control.ID = string.Format("DYNAMIC_{0}", dynamicData.Name);
                    control.CssClass = "form-control input-sm";
                    tdField.Controls.Add(control);
                }
                tr.Cells.Add(tdField);

                dynamicDataPlaceHolder.Controls.Add(tr);
            }
        }

        protected void btnSearchType_Click(object sender, EventArgs e)
        {
            dynamicDataPlaceHolder.Visible = !dynamicDataPlaceHolder.Visible;
            btnSearchType.Text = "Ricerca avanzata";
            if (dynamicDataPlaceHolder.Visible)
            {
                btnSearchType.Text = "Ricerca semplice";
            }
        }

        protected void hlPriority_Click(object sender, EventArgs e)
        {
            BindPriority();
        }

        protected void hlLastModifiedSeries_Click(object sender, EventArgs e)
        {
            BindLatestItem();
        }

        #endregion

        #region [ Methods ]

        private void InitializeAjax()
        {
            MyMaster.AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchType, pnlSearch);
            RadGrid grid = GridPlaceHolder.FindControl(DocumentSeriesRadGrid.DefaultControlId) as RadGrid;
            MyMaster.AjaxManager.AjaxSettings.AddAjaxSetting(grid, grid, MyMaster.AjaxLoadingPanel);
            MyMaster.AjaxManager.AjaxSettings.AddAjaxSetting(hlLastModifiedSeries, grid, MyMaster.AjaxLoadingPanel);
            MyMaster.AjaxManager.AjaxSettings.AddAjaxSetting(hlPriority, grid, MyMaster.AjaxLoadingPanel);
            MyMaster.AjaxManager.AjaxSettings.AddAjaxSetting(hlPriority, hlLastModifiedSeries, null, UpdatePanelRenderMode.Inline);
            MyMaster.AjaxManager.AjaxSettings.AddAjaxSetting(hlLastModifiedSeries, hlPriority, null, UpdatePanelRenderMode.Inline);
        }

        private void Initialize()
        {
            string resultFinder = SerializationHelper.SerializeToString(Finder);
            btnAllPublishedSeries.PostBackUrl = string.Concat("SeriesGrid.aspx?idSeries=", IdSeries, "&noFilter=true", (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : ""));

            string seriesName = SeriesWso.Name;
            if (IdSubSection.HasValue)
            {
                seriesName = string.Concat(seriesName, " - ", SeriesWso.DocumentSeriesSubsections.Single(ss => ss.Id == IdSubSection.Value).Description);
            }

            if (ShowSeriesCountDetails)
            {
                if (SeriesPreviewConfiguration.PriorityEnabled && PrioritySeriesResultCount > 0)
                {
                    lblSeriesName.Text = string.Format("{0} ({1} {2} di cui {3} in Primo Piano)", seriesName, TotalSeriesResultCount,
                        TotalSeriesResultCount == 1 ? "elemento" : "elementi", PrioritySeriesResultCount);
                }
                else
                {
                    lblSeriesName.Text = string.Format("{0} ({1} {2})", seriesName, TotalSeriesResultCount,
                       TotalSeriesResultCount == 1 ? "elemento" : "elementi");
                }
            }
            else
            {
                lblSeriesName.Text = seriesName.ToString();
            }


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


            DocumentSeriesHeader headerItem = Singleton.Instance.DocumentSeriesHeaders.SingleOrDefault(t => t.IdSeries.HasValue && t.IdSeries.Value.Equals(IdSeries));
            if (IdSubSection.HasValue)
            {
                headerItem = Singleton.Instance.DocumentSeriesHeaders.SingleOrDefault(t => t.IdSeries.HasValue && t.IdSeries.Value.Equals(IdSeries) && t.IdSubSection.HasValue && t.IdSubSection.Value.Equals(IdSubSection.Value));
            }
            lblHeader.Text = headerItem != null ? headerItem.Header : string.Empty;

            inYear.NumberFormat.DecimalDigits = 0;
            inYear.NumberFormat.GroupSeparator = string.Empty;

            dynamicDataPlaceHolder.Visible = !SimpleSearchEnable;
            btnSearchType.Visible = SimpleSearchEnable;
            btnSearchType.Text = "Ricerca avanzata";
            btnAllPublishedSeries.Visible = !ShowAllItems;

            btnSearch.PostBackUrl = string.Format("{0}?IdSeries={1}{2}{3}", btnSearch.PostBackUrl, IdSeries, IdSubSection.HasValue ? "&IdSubSection=" + IdSubSection : "", (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : ""));

            var grid = GridPlaceHolder.FindControl(DocumentSeriesRadGrid.DefaultControlId) as RadGrid;
            switch (TotalSeriesResultCount)
            {
                case 0:
                    pnlPreviewSeries.Visible = false;
                    btnSearch.Enabled = OneDocumentSeriesItemEnable ? false : true;
                    break;
                case 1:
                    pnlPreviewSeries.Visible = true;
                    btnSearch.Enabled = OneDocumentSeriesItemEnable ? false : true;
                    string serializedItems = MyMaster.StoricoEnabled ? MyMaster.Client.SearchRetired(resultFinder, true) : MyMaster.Client.Search(resultFinder, true);
                    MyMaster.Client.Close();
                    DocumentSeriesItemResultWSO items = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(serializedItems);

                    if (grid == null)
                        return;
                    grid.VirtualItemCount = TotalSeriesResultCount;

                    hlLastModifiedSeries.Visible = false;
                    hlPriority.Visible = false;
                    lblLastModifiedSeries.Visible = false;
                    lblPriority.Visible = false;
                    DocumentSeriesItemWSO currentItem = items.DocumentSeriesItems.First();
                    pnlPreviewSeries.Visible = SeriesPreviewConfiguration.LatestSeriesEnabled || SeriesPreviewConfiguration.PriorityEnabled;
                    if (SeriesPreviewConfiguration.LatestSeriesEnabled)
                    {
                        DateTime? lastUpdateDate = (currentItem.PublishingDate > currentItem.LastChangedDate || !currentItem.LastChangedDate.HasValue) ? currentItem.PublishingDate : currentItem.LastChangedDate;
                        lblLastModifiedSeries.Visible = true;
                        lblLastModifiedSeries.Text = string.Format("Informazioni recenti ({0:dd/MM/yyyy})", lastUpdateDate);
                    }

                    if (SeriesPreviewConfiguration.PriorityEnabled && currentItem.Priority.HasValue && currentItem.Priority.Value)
                    {
                        lblPriority.Visible = true;
                        lblPriority.Text = "In Primo Piano";
                    }
                    break;
                default:
                    pnlPreviewSeries.Visible = SeriesPreviewConfiguration.PriorityEnabled || SeriesPreviewConfiguration.LatestSeriesEnabled;
                    lblPriority.Visible = false;
                    lblLastModifiedSeries.Visible = false;
                    hlLastModifiedSeries.Visible = false;
                    hlPriority.Visible = false;
                    if (SeriesPreviewConfiguration.LatestSeriesEnabled)
                    {
                        DocumentSeriesItemResultWSO latestItemResult = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(SerializedLatestSeriesResult);
                        DocumentSeriesItemWSO latestItem = latestItemResult.DocumentSeriesItems.First();
                        DateTime? lastUpdateDate = (latestItem.PublishingDate > latestItem.LastChangedDate || !latestItem.LastChangedDate.HasValue) ? latestItem.PublishingDate : latestItem.LastChangedDate;
                        hlLastModifiedSeries.Visible = true;
                        hlLastModifiedSeries.Text = string.Format("Informazioni recenti ({0:dd/MM/yyyy})", lastUpdateDate);
                        if ((!SeriesPreviewConfiguration.PriorityEnabled || PrioritySeriesResultCount == 0 || DefaultViewSeries == ViewSerieLatestSeries) && grid != null)
                        {
                            BindLatestItem();
                        }
                    }

                    if (SeriesPreviewConfiguration.PriorityEnabled && PrioritySeriesResultCount > 0)
                    {
                        // DocumentSeriesItemResultWSO priorityItems = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(SerializedPriorityResult);
                        hlPriority.Visible = true;
                        hlPriority.Text = "In Primo Piano";
                        if (grid == null)
                            return;
                        if (DefaultViewSeries == ViewSeriePriority || DefaultViewSeries == "")
                        {
                            BindPriority();
                        }
                    }
                    break;
            }
        }

        private void InitCsvDocument()
        {
            var path = this._parameterService.GetString("CsvFilesPath");
            if (String.IsNullOrEmpty(path))
            {
                csvLink.Visible = false;
                return;
            }

            var files = Directory.GetFiles(path);

            var doc = files.SingleOrDefault(x => Path.GetFileName(x).Eq(CsvFileName));
            if (String.IsNullOrEmpty(doc))
            {
                csvLink.Visible = false;
                return;
            }

            var fi = new FileInfo(doc);
            csvLink.NavigateUrl = ResolveUrl("~/FileDocument?name=" + CsvFileName);
            csvLink.ToolTip = String.Format("Esporta in formato CSV (ultimo aggiornamento {0}", fi.LastWriteTime);
        }

        private static object GetControlValue(Control source)
        {
            var radTextBox = source as RadTextBox;
            if ((radTextBox != null) && !string.IsNullOrEmpty(radTextBox.Text))
            {
                return radTextBox.Text;
            }

            var radNumericTextBox = source as RadNumericTextBox;
            if ((radNumericTextBox != null) && radNumericTextBox.Value.HasValue)
            {
                return radNumericTextBox.Value;
            }

            var radDatePicker = source as RadDatePicker;
            if ((radDatePicker != null) && radDatePicker.SelectedDate.HasValue)
            {
                return radDatePicker.SelectedDate.Value;
            }

            var label = source as Label;
            if ((label != null) && !string.IsNullOrEmpty(label.Text))
            {
                return label.Text;
            }

            return null;

        }
        void GridNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var grid = sender as RadGrid;
            if (grid == null)
            {
                return;
            }
            var myFinder = Finder;
            if (myFinder == null)
            {
                grid.DataSource = new List<DocumentSeriesItemWSO>();
                return;
            }

            myFinder.Skip = grid.CurrentPageIndex * grid.PageSize;
            myFinder.Take = grid.PageSize;

            var temp = SerializationHelper.SerializeToStringWithoutNamespace(myFinder);
            var serializedResult = MyMaster.StoricoEnabled ? MyMaster.Client.SearchRetired(temp, true) : MyMaster.Client.Search(temp, true);
            MyMaster.Client.Close();
            var result = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(serializedResult);


            grid.DataSource = result.DocumentSeriesItems;
        }



        protected void BindLatestItem()
        {
            RadGrid grid = GridPlaceHolder.FindControl(DocumentSeriesRadGrid.DefaultControlId) as RadGrid;
            if (grid == null)
                return;
            SethlPriorityHigh(false);
            DocumentSeriesItemResultWSO latestItemResult = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(SerializedLatestSeriesResult);
            grid.DataSource = latestItemResult.DocumentSeriesItems;
            grid.CurrentPageIndex = 0;
            grid.AllowPaging = ShowAllItems;
            grid.VirtualItemCount = NumerOfItems(latestItemResult.TotalRowCount);
            grid.DataBind();
        }

        protected void BindPriority()
        {
            RadGrid grid = GridPlaceHolder.FindControl(DocumentSeriesRadGrid.DefaultControlId) as RadGrid;
            if (grid == null)
                return;
            SethlPriorityHigh(true);
            DocumentSeriesItemResultWSO priorityItemResult = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(SerializedPriorityResult);
            grid.DataSource = priorityItemResult.DocumentSeriesItems;
            grid.CurrentPageIndex = 0;
            grid.AllowPaging = false;
            grid.VirtualItemCount = priorityItemResult.TotalRowCount;
            grid.DataBind();
        }

        protected void SethlPriorityHigh(bool setit)
        {
            hlPriority.Font.Italic = !setit;
            hlPriority.Font.Bold = setit;
            hlLastModifiedSeries.Font.Italic = setit;
            hlLastModifiedSeries.Font.Bold = !setit;
        }


        #endregion        
    }
}