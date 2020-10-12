namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataEnumFilterModel : IMetadataFilterModel
    {
        #region [ Fields ]
        private readonly MetadataFinderModel _model;
        #endregion

        #region [ Constructor ]
        public MetadataEnumFilterModel(MetadataFinderModel model)
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
