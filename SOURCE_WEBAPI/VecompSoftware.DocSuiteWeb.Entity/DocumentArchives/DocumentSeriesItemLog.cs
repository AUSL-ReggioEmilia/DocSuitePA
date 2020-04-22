using System;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
{
    public class DocumentSeriesItemLog : DSWBaseLogEntity<DocumentSeriesItem, string>
    {
        #region [ Constructor ]
        public DocumentSeriesItemLog() : this(Guid.NewGuid()) { }

        public DocumentSeriesItemLog(Guid uniqueId)
            : base(uniqueId) { }

        #endregion

        public DateTime LogDate { get; set; }

        public string Program { get; set; }

        public int IdDocumentSeriesItem { get; set; }

    }
}
