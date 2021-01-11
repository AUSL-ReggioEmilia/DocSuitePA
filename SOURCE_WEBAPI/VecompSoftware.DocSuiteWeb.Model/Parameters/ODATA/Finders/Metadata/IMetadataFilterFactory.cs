namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public interface IMetadataFilterFactory
    {
        IMetadataFilterModel CreateMetadataFilter(MetadataFinderModel model);
    }
}
