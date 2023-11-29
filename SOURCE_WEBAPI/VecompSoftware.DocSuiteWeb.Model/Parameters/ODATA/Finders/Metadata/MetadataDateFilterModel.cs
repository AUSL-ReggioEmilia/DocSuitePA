using System;
using System.Globalization;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataDateFilterModel : IMetadataFilterModel
    {
        #region [ Fields ]
        private readonly MetadataFinderModel _model;
        #endregion

        #region [ Constructor ]
        public MetadataDateFilterModel(MetadataFinderModel model)
        {
            _model = model;
        }
        #endregion

        #region [ Properties ]
        public DateTime? FromDate
        {
            get
            {
                if (string.IsNullOrEmpty(_model.Value))
                {
                    return null;
                }
                return DateTime.ParseExact(_model.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }
        public DateTime? ToDate
        {
            get
            {
                if (string.IsNullOrEmpty(_model.ToValue))
                {
                    return null;
                }
                return DateTime.ParseExact(_model.ToValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region [ Methods ]
        public string ToFilter()
        {
            return $"{(FromDate.HasValue ? $"{FromDate:yyyy-MM-dd'T'HH:mm:ss.FFFK}" : string.Empty)}|{(ToDate.HasValue ? $"{ToDate:yyyy-MM-dd'T'HH:mm:ss.FFFK}" : string.Empty)}";
        }
        #endregion
    }
}
