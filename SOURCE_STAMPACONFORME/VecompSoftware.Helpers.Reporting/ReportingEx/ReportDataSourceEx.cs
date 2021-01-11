using System.Collections.Generic;
using System.Linq;
using Microsoft.Reporting.WebForms;

namespace VecompSoftware.Helpers.Reporting.ReportingEx
{
    public static class ReportDataSourceEx
    {

        public static ReportDataSource ApplyParameters(this ReportDataSource source, ReportParameterInfoCollection parameters)
        {
            var values = (IEnumerable<object>)source.Value;
            var result = values.Where(i => parameters.Comply(i));
            return new ReportDataSource(source.Name, result);
        }

    }
}
