using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using VecompSoftware.Helpers;
using System.Linq;
using System.Text;
using AmministrazioneTrasparente.Tools;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;
using System.Web;
using AmministrazioneTrasparente.Services;
using Google.Apis.AnalyticsReporting.v4.Data;

namespace AmministrazioneTrasparente
{
    public partial class SeriesGrid : BaseSeriesPage
    {
        #region [ Fields ]
        private readonly ParameterService _parameterService = new ParameterService();
        #endregion

        #region [ Properties ]

        private string SerializedFinder
        {
            get
            {
                return (string)ViewState["SerializedFinder"];
            }
            set { ViewState["SerializedFinder"] = value; }
        }

        private DocumentSeriesItemFinderWSO Finder
        {
            get
            {
                if (string.IsNullOrEmpty(SerializedFinder))
                {
                    return null;
                }
                return SerializationHelper.SerializeFromString<DocumentSeriesItemFinderWSO>(SerializedFinder);
            }
        }

        private bool NoFilter
        {
            get
            {
                if (string.IsNullOrEmpty(Request.QueryString["noFilter"]))
                    return false;

                return bool.Parse(Request.QueryString["noFilter"]);
            }
        }
        protected bool EnableHeaderComFixing
        {
            get { return this._parameterService.GetBoolean("EnableHeaderComFixing"); }
        }
        #endregion

        #region [ Events ]  
        protected override void OnInit(EventArgs e)
        {
            if (!SeriesResultsByConstraintEnabled)
            {
                uscSeriesGrid.StoricoEnabled = MyMaster.StoricoEnabled;
                uscSeriesGrid.DynamicColumnsInGrid = DynamicColumnsInGrid;
                uscSeriesGrid.GridStructure = Structure;
                uscSeriesGrid.NeedDataSource += GridNeedDataSource;
                uscSeriesGrid.NeedItemCount += GridNeedItemCount;
            }            
            else
            {
                if (!IsPostBack)
                {
                    var xmlFinder = NoFilter ? SerializationHelper.SerializeToString(PreviousPage.BaseFinder) : SerializationHelper.SerializeToString(PreviousPage.Finder);
                    string serializedResult = MyMaster.StoricoEnabled ? MyMaster.Client.SearchConstraintsRetired(xmlFinder) : MyMaster.Client.SearchConstraints(xmlFinder);
                    MyMaster.Client.Close();
                    uscSeriesGridConstraints.Constraints = SerializationHelper.SerializeFromString<string[]>(serializedResult);
                }

                uscSeriesGridConstraints.StoricoEnabled = MyMaster.StoricoEnabled;
                uscSeriesGridConstraints.DynamicColumnsInGrid = DynamicColumnsInGrid;
                uscSeriesGridConstraints.GridStructure = Structure;
                uscSeriesGridConstraints.NeedDataSource += GridNeedDataSource;
                uscSeriesGridConstraints.NeedItemCount += GridNeedItemCount;
            }                                    
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!Page.IsPostBack)
            {
                if (PreviousPage == null)
                {
                    string subSection = IdSubSection.HasValue ? "&IdSubSection=" + IdSubSection : "";
                    Response.Redirect(ResolveUrl(string.Format("~/Series.aspx?idSeries={0}{1}{2}", IdSeries, subSection, (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : ""))), true);
                    return;
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

                var seriesName = new StringBuilder(SeriesWso.Name);
                if (IdSubSection.HasValue)
                {
                    seriesName.AppendFormat(" - {0}", SeriesWso.DocumentSeriesSubsections.Single(ss => ss.Id == IdSubSection.Value).Description);
                }
                lblSeriesName.Text = seriesName.ToString();
                uscSeriesGrid.Visible = !SeriesResultsByConstraintEnabled;
                uscSeriesGridConstraints.Visible = SeriesResultsByConstraintEnabled;
                // Salvo in ViewState il Finder dalla pagina precedente
                SerializedFinder = NoFilter ? SerializationHelper.SerializeToString(PreviousPage.BaseFinder) : SerializationHelper.SerializeToString(PreviousPage.Finder);

                RadScriptBlock.Visible = EnableHeaderComFixing;
            }
        }

        private void GridNeedItemCount(object sender, EventArgs e)
        {
            RadGrid grid = sender as RadGrid;
            if (grid == null)
            {
                return;
            }

            var myFinder = Finder;
            if (SeriesResultsByConstraintEnabled)
            {
                string finderAttribute = grid.Attributes[UserControls.uscSeriesGridConstraints.GRID_CONSTRAINT_ATTRIBUTE];
                myFinder.FindByConstraints = true;
                myFinder.Constraint = finderAttribute;
            }            
            var temp = SerializationHelper.SerializeToStringWithoutNamespace(myFinder);
            grid.VirtualItemCount = MyMaster.StoricoEnabled ? MyMaster.Client.SearchCountRetired(temp) : MyMaster.Client.SearchCount(temp);
            MyMaster.Client.Close();
        }

        private void GridNeedDataSource(object sender, EventArgs e)
        {
            RadGrid grid = sender as RadGrid;
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

            if (SeriesResultsByConstraintEnabled)
            {
                string finderAttribute = grid.Attributes[UserControls.uscSeriesGridConstraints.GRID_CONSTRAINT_ATTRIBUTE];
                myFinder.FindByConstraints = true;
                myFinder.Constraint = finderAttribute;
            }            

            var temp = SerializationHelper.SerializeToStringWithoutNamespace(myFinder);
            var serializedResult = MyMaster.StoricoEnabled ? MyMaster.Client.SearchRetired(temp, true) : MyMaster.Client.Search(temp, true);
            MyMaster.Client.Close();
            var result = SerializationHelper.SerializeFromString<DocumentSeriesItemResultWSO>(serializedResult);

            grid.DataSource = result.DocumentSeriesItems;
        }

        #endregion
    }
}