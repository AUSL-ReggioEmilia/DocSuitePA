using Microsoft.Reporting.WebForms;


namespace VecompSoftware.Helpers.Reporting.ReportingEx
{
    public static class ReportParameterEx
    {
        /// <summary>
        /// Crea un parametro da aggiungere al report
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ReportParameter Add(string parameterName, string value)
        {
            return new ReportParameter(parameterName, value);
        }
    }
}
