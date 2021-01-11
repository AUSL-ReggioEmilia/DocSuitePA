using System;

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
                return DateTime.Parse(_model.Value);
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
                return DateTime.Parse(_model.ToValue);
            }
        }
        #endregion

        #region [ Methods ]
        public string ToFilter()
        {
            return $"{(FromDate.HasValue ? $"{FromDate:dd/MM/yyyy}" : string.Empty)}|{(ToDate.HasValue ? $"{ToDate:dd/MM/yyyy}" : string.Empty)}";
        }
        #endregion
    }
}
