using System;

namespace VecompSoftware.DocSuiteWeb.API
{
    public interface IDocumentSeriesItemDTO
    {
        #region [ Properties ]

        int? IdDocumentSeries { get; set; }

        int? Status { get; set; }

        DateTime? PublishingDate { get; set; }

        ICategoryDTO Category { get; set; }

        IDocumentDTO Document { get; set; }
        int MaxTimesError { get; set; }

        #endregion
    }
}
