namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataTextFilterModel : IMetadataFilterModel
    {
        #region [ Fields ]
        private readonly MetadataFinderModel _model;
        #endregion

        #region [ Constructor ]
        public MetadataTextFilterModel(MetadataFinderModel model)
        {
            _model = model;
        }
        #endregion

        public string ToFilter()
        {
            return _model.Value;
        }
    }
}
