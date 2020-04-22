using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VecompSoftware.Commons.BiblosInterfaces;
using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Commons.Configuration;
using VecompSoftware.Commons.Infos;

namespace VecompSoftware.Services.Reporting.CommonEx
{
    public static class ISignContentEx
    {

        public static List<SignContentReport> GetSignContentReports(this ISignContent signContent)
        {
            var heading = ConfigurationHelper.GetValueOrDefault("Reporting.SignContentReport.Heading",
                "LA PRESENTE COPIA E' CONFORME ALL'ORIGINALE DEPOSITATO.\r\nElenco firme associate al file con impronta SHA1 (hex):");
            heading = Regex.Unescape(heading);
            var sha1 = BitConverter.ToString(signContent.Content.ComputeSHA1());
            int i = 1;
            var results = GetReportRecursive(signContent.Signatures);
            var rowCount = results.Count();
            foreach (var item in results)
            {
                item.Id = i++;
                item.RowCount = rowCount;
                item.Heading = heading;
                item.SHA1 = sha1;
            }
            return results;
        }

        private static List<SignContentReport> GetReportRecursive(IEnumerable<SignInfo> signs, int level)
        {
            var result = new List<SignContentReport>();
            foreach (var item in signs)
            {
                var report = new SignContentReport(item) { Level = level };
                result.Add(report);
                if (item.HasChildren)
                    result.AddRange(GetReportRecursive(item.Children, report.Level + 1));
            }
            return result;
        }
        private static List<SignContentReport> GetReportRecursive(IEnumerable<SignInfo> signs)
        {
            return GetReportRecursive(signs, 0);
        }

    }
}
