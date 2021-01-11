namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataBoolFilterModel : IMetadataFilterModel
    {
        #region [ Fields ]
        private readonly MetadataFinderModel _model;
        #endregion

        #region [ Constructor ]
        public MetadataBoolFilterModel(MetadataFinderModel model)
        {
            _model = model;
        }
        #endregion

        #region [ Properties ]
        public bool? Value
        {
            get
            {
                if (string.IsNullOrEmpty(_model.Value))
                {
                    return null;
                }
                return bool.Parse(_model.Value);
            }
        }
        #endregion

        #region [ Methods ]
        public string ToFilter()
        {
            return $"{Value}";
        }
        #endregion
    }
}
