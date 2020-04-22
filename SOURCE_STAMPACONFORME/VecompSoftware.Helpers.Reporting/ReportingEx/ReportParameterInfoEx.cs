using System.Linq;
using Microsoft.Reporting.WebForms;

namespace VecompSoftware.Helpers.Reporting.ReportingEx
{
    public static class ReportParameterInfoEx
    {

        public static bool Comply(this ReportParameterInfoCollection source, object obj)
        {
            var temp1 = source.Any(p => p.Comply(obj));
            var temp2 = source.All(p => p.Comply(obj));
            return temp1 && temp2;
        }
        public static bool Comply(this ReportParameterInfo source, object obj)
        {
            var value = obj.GetType().GetProperty(source.Name).GetValue(obj, null).ToString();
            var temp1 = source.Values.Any(v => v.Equals(value));
            var temp2 = source.Values.All(v => v.Equals(value));
            return temp1 && temp2;
        }

    }
}
