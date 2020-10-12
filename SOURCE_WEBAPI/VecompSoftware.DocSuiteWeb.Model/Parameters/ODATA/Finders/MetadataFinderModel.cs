using VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders
{
    public class MetadataFinderModel
    {
        #region [ Constructor ]
        public MetadataFinderModel()
        {

        }
        #endregion

        #region [ Properties ]
        public MetadataFinderType MetadataType { get; set; }
        public string KeyName { get; set; }
        public string Value { get; set; }
        public string ToValue { get; set; }
        #endregion
    }
}
