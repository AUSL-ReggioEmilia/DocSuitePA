using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    public class TemplateReport : DSWBaseEntity
    {
        #region [ Contructor ]
        public TemplateReport() : this(Guid.NewGuid()) { }
        public TemplateReport(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public string Name { get; set; }
        public Guid IdArchiveChain { get; set; }
        public TemplateReportStatus Status { get; set; }
        public int Environment { get; set; }
        public string ReportBuilderJsonModel { get; set; }

        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
