using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.Services;
using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.DTO.WSSeries;

namespace AmministrazioneTrasparente
{
    public partial class Statics : BasePage
    {
        #region [ Fields ]


        #endregion

        #region Properties

        protected List<DocumentSeriesWSO> DocumentSeriesWSOs
        {
            get
            {
                if (Cache["StatisticsDocumentSeriesWSOs"] == null)
                {                    
                    Cache.Insert("StatisticsDocumentSeriesWSOs", MyMaster.Families.DocumentSeriesFamilies.SelectMany(f => f.DocumentSeries).ToList(), null, DateTime.Now.AddMinutes(CacheExpiration), TimeSpan.Zero);
                    MyMaster.Client.Close();
                }
                return (List<DocumentSeriesWSO>)Cache["StatisticsDocumentSeriesWSOs"];
            }
        }

        public List<StatisticModel> ViewStatistics { get; } = new List<StatisticModel>();

        private List<SeriesStatsModel> _seriesStats = new List<SeriesStatsModel>();
        public List<SeriesStatsModel> SeriesStats
        {
            get
            {
                return _seriesStats;
            }
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            if (!String.IsNullOrEmpty(MyMaster.GoogleAnalyticsCode))
            {
                GenerateSiteAccessReport();
                GetOrderedSeriesViews();

                StatisticsGrid.DataSource = ViewStatistics;
                StatisticsGrid.DataBind();

                SeriesStatisticsGrid.DataSource = SeriesStats;
                SeriesStatisticsGrid.DataBind();
            }
        }

        private void GenerateSiteAccessReport()
        {
            int i;
            StatisticModel tmpStat;

            AnalyticsReportService reportService = new AnalyticsReportService(MyMaster.GoogleReportingService);
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:year" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:pageviews" },
                                             new Metric { Expression = "ga:uniquePageviews"} };

            List<DateRange> dateRanges = new List<DateRange>();
            dateRanges.Add(new DateRange
            {
                StartDate = MyMaster.AnalyticsStartDate,
                EndDate = DateTime.Now.ToString("yyyy-MM-dd")
            });

            var rows = reportService.GetStatisticsByMetrics(dateRanges, dimensions, metrics, MyMaster.AnalyticsIDView);
            foreach (ReportRow row in rows)
            {
                for (i = 0; i < row.Dimensions.Count; i++)
                {
                    tmpStat = new StatisticModel(row.Dimensions[i], row.Metrics[0].Values[0], row.Metrics[0].Values[1]);
                    ViewStatistics.Add(tmpStat);
                }
            }
        }

        private void GetOrderedSeriesViews()
        {
            AnalyticsReportService reportService = new AnalyticsReportService(MyMaster.GoogleReportingService);
            var seriesList = reportService.GetSeriesViews(MyMaster.AnalyticsStartDate, MyMaster.AnalyticsIDView);
            seriesList.Sort((p1, p2) => p1.Value.CompareTo(p2.Value));
            seriesList = seriesList.OrderByDescending(o => o.Value).ToList<KeyValuePair<int, int>>();
            foreach (KeyValuePair<int, int> keyVal in seriesList.Where(f=> DocumentSeriesWSOs.Any(x=> x.Id == f.Key)))
            {
                _seriesStats.Add(new SeriesStatsModel(DocumentSeriesWSOs.SingleOrDefault(f=> f.Id == keyVal.Key).Name, keyVal.Value));
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        #endregion
    }
}