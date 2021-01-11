using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Metadata;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class MetadataValueService : BaseService<MetadataValue>, IMetadataValueService
    {
        #region [ Field ]
        #endregion

        #region [ Constructor ]

        public MetadataValueService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IMetadataRepositoryRuleset MetadataRepositoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, MetadataRepositoryRuleset, mapperUnitOfWork, security)
        {
        }

        #endregion

        #region [ Method ]
        public static MetadataValue CreateMetadataValue(MetadataDesignerModel metadataDesignerModel, MetadataValueModel metadataValueModel)
        {
            MetadataValue metadataValue = new MetadataValue();
            metadataValue.Name = metadataValueModel.KeyName;
            if (metadataDesignerModel.TextFields.Any(x => x.KeyName == metadataValueModel.KeyName)
                || metadataDesignerModel.DiscussionFields.Any(x => x.KeyName == metadataValueModel.KeyName)
                || metadataDesignerModel.EnumFields.Any(x => x.KeyName == metadataValueModel.KeyName))
            {
                metadataValue.ValueString = metadataValueModel.Value;
                metadataValue.PropertyType = MetadataPropertyType.PropertyString;
            }
            if (metadataDesignerModel.BoolFields.Any(x => x.KeyName == metadataValueModel.KeyName))
            {
                if (bool.TryParse(metadataValueModel.Value, out bool res))
                {
                    metadataValue.ValueBoolean = res;
                }
                metadataValue.PropertyType = MetadataPropertyType.PropertyBoolean;
            }
            if (metadataDesignerModel.DateFields.Any(x => x.KeyName == metadataValueModel.KeyName))
            {
                if (DateTime.TryParse(metadataValueModel.Value, out DateTime res))
                {
                    metadataValue.ValueDate = res;
                }
                metadataValue.PropertyType = MetadataPropertyType.PropertyDate;
            }
            if (metadataDesignerModel.NumberFields.Any(x => x.KeyName == metadataValueModel.KeyName))
            {
                if (int.TryParse(metadataValueModel.Value, out int res))
                {
                    metadataValue.ValueInt = res;
                }
                metadataValue.PropertyType = MetadataPropertyType.PropertyInt;
            }
            return metadataValue;
        }
        #endregion
    }
}
