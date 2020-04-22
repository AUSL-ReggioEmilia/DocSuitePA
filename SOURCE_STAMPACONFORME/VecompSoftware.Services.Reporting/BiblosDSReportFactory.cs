using VecompSoftware.Commons.CommonEx;
using VecompSoftware.Commons.Configuration;
using VecompSoftware.Helpers.Reporting;

namespace VecompSoftware.Services.Reporting
{
    public static class BiblosDSReportFactory
    {

        public static RdlcContentInfo GetReport<T>() where T : IReport
        {
            var prefix = ConfigurationHelper.GetValueOrDefault("Reporting.CustomizationPrefix", string.Empty);
            var fileName = typeof(T).Name.ToPath().ChangeExtension(".rdlc");
            if (!prefix.IsNullOrWhiteSpace())
                fileName = prefix + fileName;

            return new RdlcContentInfo(fileName);
        }

    }
}
