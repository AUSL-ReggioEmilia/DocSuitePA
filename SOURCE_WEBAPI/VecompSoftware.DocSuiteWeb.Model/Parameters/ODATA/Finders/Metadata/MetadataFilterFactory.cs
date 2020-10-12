using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Parameters.ODATA.Finders.Metadata
{
    public class MetadataFilterFactory : IMetadataFilterFactory
    {
        #region [ Fields ]
        private readonly IDictionary<MetadataFinderType, Func<MetadataFinderModel, IMetadataFilterModel>> _metadata_filters_instances;
        #endregion

        #region [ Constructor ]
        public MetadataFilterFactory()
        {
            _metadata_filters_instances = new Dictionary<MetadataFinderType, Func<MetadataFinderModel, IMetadataFilterModel>>()
            {
                { MetadataFinderType.Text, (model) => new MetadataTextFilterModel(model) },
                { MetadataFinderType.Number, (model) => new MetadataNumberFilterModel(model) },
                { MetadataFinderType.Bool, (model) => new MetadataBoolFilterModel(model) },
                { MetadataFinderType.Date, (model) => new MetadataDateFilterModel(model) },
                { MetadataFinderType.Enum, (model) => new MetadataEnumFilterModel(model) },
                { MetadataFinderType.Contact, (model) => new MetadataContactFilterModel(model) },
                { MetadataFinderType.Discussion, (model) => new MetadataDiscussionFilterModel(model) }
            };
        }
        #endregion

        #region [ Methods ]
        public IMetadataFilterModel CreateMetadataFilter(MetadataFinderModel model)
        {
            if (!_metadata_filters_instances.ContainsKey(model.MetadataType))
            {
                throw new ArgumentException($"MetadataType {model.MetadataType} is not valid");
            }
            return _metadata_filters_instances[model.MetadataType](model);
        }
        #endregion        
    }
}
