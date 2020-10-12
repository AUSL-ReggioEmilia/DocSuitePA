namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataNumberFilterModel : IMetadataFilterModel
    {
        #region [ Fields ]
        private readonly MetadataFinderModel _model;
        #endregion

        #region [ Constructor ]
        public MetadataNumberFilterModel(MetadataFinderModel model)
        {
            _model = model;
        }
        #endregion

        #region [ Properties ]
        public decimal? FromNumber
        {
            get
            {
                if (string.IsNullOrEmpty(_model.Value))
                {
                    return null;
                }
                return decimal.Parse(_model.Value);
            }
        }
        public decimal? ToNumber
        {
            get
            {
                if (string.IsNullOrEmpty(_model.ToValue))
                {
                    return null;
                }
                return decimal.Parse(_model.ToValue);
            }
        }
        #endregion

        #region [ Methods ]
        public string ToFilter()
        {
            return $"{(FromNumber.HasValue ? $"{FromNumber}" : string.Empty)}|{(ToNumber.HasValue ? $"{ToNumber}" : string.Empty)}";
        }
        #endregion
    }
}
