namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataContactFilterModel : IMetadataFilterModel
    {
        #region [ Fields ]
        private readonly MetadataFinderModel _model;
        #endregion

        #region [ Constructor ]
        public MetadataContactFilterModel(MetadataFinderModel model)
        {
            _model = model;
        }
        #endregion

        #region [ Properties ]
        
        #endregion

        #region [ Methods ]
        public string ToFilter()
        {
            return $"{_model.Value}";
        }
        #endregion
    }
}
