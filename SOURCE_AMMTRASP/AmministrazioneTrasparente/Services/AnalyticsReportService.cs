using AmministrazioneTrasparente.Code;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmministrazioneTrasparente.Services
{
    public class AnalyticsReportService
    {
        private readonly AnalyticsReportingService _reportingService;

        public AnalyticsReportService(AnalyticsReportingService reportingService)
        {
            this._reportingService = reportingService;
        }

        /// <summary>
        /// Send a generic request based on provided Dimensions and Metrics lists, calculated from configuration startDate
        /// </summary>
        /// <param name="dateRanges">Range of dates for calculate views</param>
        /// <param name="dimensions">See https://developers.google.com/analytics/devguides/reporting/core/dimsmets#cats=session</param>
        /// <param name="metrics">See https://developers.google.com/analytics/devguides/reporting/core/dimsmets#cats=session</param>
        /// <param name="viewId">VIEW_ID specified in the Analytics console</param>
        /// /// <returns>A list of rows filled with response data</returns>
        public List<ReportRow> GetStatisticsByMetrics(List<DateRange> dateRanges,
                                                                  List<Dimension> dimensions,
                                                                  List<Metric> metrics,
                                                                  string viewId)
        {
            GetReportsResponse response = SendReportRequest(dateRanges, dimensions, metrics, viewId);
            List<List<string>> responseList = new List<List<string>>();
            Report report = response.Reports[0];
            List<ReportRow> rows = (List<ReportRow>)report.Data.Rows;

            return rows;
        }

        /// <summary>
        /// Get a collection of document series pages whit their numebr of views
        /// </summary>
        /// <param name="startDate">the date whence the count starts </param>
        /// <param name="viewId">VIEW_ID specified in the Analytics console</param>
        /// <returns>A dictionary containing how many views every document series had</returns>
        public List<KeyValuePair<int, int>> GetSeriesViews(string startDate,
                                                         string viewId)
        {
            int i, documentId;
            UriBuilder uriBuilder;
            string queryString;
            var dimensions = new List<Dimension> { new Dimension { Name = "ga:pagePath" } };
            var metrics = new List<Metric> { new Metric { Expression = "ga:pageviews" } };

            DateRange dateRange = new DateRange
            {
                StartDate = startDate,
                EndDate = DateTime.Now.ToString("yyyy-MM-dd")
            };

            List<DateRange> dateRanges = new List<DateRange> { dateRange };
            GetReportsResponse response = SendReportRequest(dateRanges, dimensions, metrics, viewId);
            List<KeyValuePair<int, int>> _pageVisitCount = new List<KeyValuePair<int, int>>();

            KeyValuePair<int, int> tmpModel;
            List<ReportRow> rows;
            foreach (var report in response.Reports)
            {
                rows = (List<ReportRow>)report.Data.Rows;
                foreach (ReportRow row in rows)
                {
                    for (i = 0; i < row.Metrics.Count(); i++)
                    {
                        if (row.Dimensions[i].Contains("/Series.aspx?idSeries"))
                        {
                            uriBuilder = new UriBuilder(HttpContext.Current.Request.Url.Host, row.Dimensions[i]);
                            queryString = uriBuilder.Uri.Query.Substring(0, uriBuilder.Uri.Query.LastIndexOf('/'));
                            var queryDictionary = HttpUtility.ParseQueryString(queryString);
                            if (queryDictionary.GetValues("idSeries") != null)
                            {
                                bool success = int.TryParse(queryDictionary.GetValues("idSeries")[0], out documentId);
                                if (success)
                                {
                                    tmpModel = new KeyValuePair<int, int>(documentId, int.Parse(row.Metrics[i].Values[0]));
                                    _pageVisitCount.Add(tmpModel);
                                }
                            }
                        }
                    }
                }
            }
            return _pageVisitCount;
        }

        /// <summary>
        /// Get the number of views for a specific page
        /// </summary>
        /// <param name="currentUrl">the page of which calculate views numer, this a relative URL is comprhensive of query parameter </param>
        /// <param name="startDate">the date whence the count starts </param>
        /// <param name="viewId">VIEW_ID specified in the Analytics console</param>
        /// <param name="_dimensions">See https://developers.google.com/analytics/devguides/reporting/core/dimsmets#cats=session</param>
        /// <param name="_metrics">See https://developers.google.com/analytics/devguides/reporting/core/dimsmets#cats=session</param>
        /// <returns>A string representing views count</returns>
        public string GetVisitorsCount(string currentUrl,
                                       string startDate,
                                       string viewId,
                                       List<Dimension> dimensions,
                                       List<Metric> metrics)
        {
            int _pageVisitCount = 0, i;
            DateRange dateRange = new DateRange
            {
                StartDate = startDate,
                EndDate = DateTime.Now.ToString("yyyy-MM-dd")
            };

            List<DateRange> dateRanges = new List<DateRange> { dateRange };

            GetReportsResponse response = SendReportRequest(dateRanges, dimensions, metrics, viewId);

            foreach (var report in response.Reports)
            {
                List<ReportRow> rows = (List<ReportRow>)report.Data.Rows;
                foreach (ReportRow row in rows)
                {
                    for (i = 0; i < row.Metrics.Count(); i++)
                    {
                        if (row.Dimensions[i] == currentUrl)
                        {
                            _pageVisitCount += int.Parse(row.Metrics[i].Values[0]);
                        }
                    }
                }
            }

            return _pageVisitCount.ToString();
        }

        private GetReportsResponse SendReportRequest(List<DateRange> dateRanges,
                                                     List<Dimension> dimensions,
                                                     List<Metric> metrics,
                                                     string viewId)
        {

            var reportRequest = new ReportRequest
            {
                DateRanges = dateRanges,
                Dimensions = dimensions,
                Metrics = metrics,
                ViewId = viewId
            };

            var getReportRequest = new GetReportsRequest();
            getReportRequest.ReportRequests = new List<ReportRequest> { reportRequest };

            try
            {
                return _reportingService.Reports.BatchGet(getReportRequest).Execute();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}