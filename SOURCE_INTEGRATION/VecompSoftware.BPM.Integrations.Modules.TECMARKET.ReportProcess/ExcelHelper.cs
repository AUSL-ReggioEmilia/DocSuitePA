using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess.Models;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.ReportProcess
{
    public static class ExcelHelper
    {
        #region [ Fields ]
        private const string SERVER_HOST = "ServerHost";
        private const string Server_IP = "ServerIp";
        private const string EVENT_DATE = "EventDate";
        private const string LOG_DATE = "LogDate";
        private const string LOG_TYPE = "LogType";
        private const string LOG_SOURCE = "LogSource";
        private const string LOG_DESCRIPTION = "LogDescription";
        #endregion

        #region [ Methods ]
        public static byte[] BuildExcel(List<EventModel> eventModels, string name)
        {
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add(name);

            CreateExcelHeader(workSheet);
            CreateExcelBody(eventModels, workSheet);

            return excel.GetAsByteArray();
        }
        #region [ Helper methods ]
        private static void CreateExcelHeader(ExcelWorksheet workSheet)
        {
            // Setting the properties of the first row 
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            workSheet.Cells[1, 1].Value = SERVER_HOST;
            workSheet.Cells[1, 2].Value = Server_IP;
            workSheet.Cells[1, 3].Value = EVENT_DATE;
            workSheet.Cells[1, 4].Value = LOG_DATE;
            workSheet.Cells[1, 5].Value = LOG_TYPE;
            workSheet.Cells[1, 6].Value = LOG_SOURCE;
            workSheet.Cells[1, 7].Value = LOG_DESCRIPTION;

            //LogDescription is taking too much space so the header is to be align to the left.
            workSheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        }
        private static void CreateExcelBody(List<EventModel> eventModels, ExcelWorksheet workSheet)
        {
            int recordIndex = 2;
            foreach (var item in eventModels.SelectMany(evt => evt.EventLogs.Select(evtLog => new { Evt = evt, EvtLog = evtLog })))
            {
                workSheet.Cells[recordIndex, 1].Value = item.Evt.ServerHost;
                workSheet.Cells[recordIndex, 2].Value = item.Evt.ServerIP;
                workSheet.Cells[recordIndex, 3].Value = item.Evt.EventDate;
                workSheet.Cells[recordIndex, 4].Value = item.EvtLog.LogDate;
                workSheet.Cells[recordIndex, 5].Value = item.EvtLog.LogType;
                workSheet.Cells[recordIndex, 6].Value = item.EvtLog.LogSource;
                workSheet.Cells[recordIndex, 7].Value = item.EvtLog.LogDescription;
                HighlightRow(workSheet, recordIndex, item.EvtLog.LogType);
                recordIndex++;
            }
            AutoFitColumns(workSheet);
        }
        private static void HighlightRow(ExcelWorksheet workSheet, int recordIndex, string logType)
        {
            workSheet.Row(recordIndex).Style.Fill.PatternType = ExcelFillStyle.Solid;
            if (logType.Equals(nameof(EventLogEntryType.Error)))
            {
                workSheet.Row(recordIndex).Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 199, 206));
            }
            if (logType.Equals(nameof(EventLogEntryType.Warning)))
            {
                workSheet.Row(recordIndex).Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 217, 102));
            }
            if (logType.Equals(nameof(EventLogEntryType.Information)))
            {
                workSheet.Row(recordIndex).Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 219, 219));
            }
        }
        private static void AutoFitColumns(ExcelWorksheet workSheet)
        {
            for (int i = 1; i <= 7; i++)
            {
                workSheet.Column(i).AutoFit();
            }
        }
        #endregion

        #endregion
    }
}
