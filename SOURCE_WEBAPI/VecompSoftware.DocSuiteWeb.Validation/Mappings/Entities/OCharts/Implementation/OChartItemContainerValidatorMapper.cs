using VecompSoftware.DocSuiteWeb.Entity.OCharts;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.OCharts;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.OCharts
{
    public class OChartItemContainerValidatorMapper : BaseMapper<OChartItemContainer, OChartItemContainerValidator>, IOChartItemContainerValidatorMapper
    {
        public OChartItemContainerValidatorMapper() { }

        public override OChartItemContainerValidator Map(OChartItemContainer entity, OChartItemContainerValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.Master = entity.Master;
            entityTransformed.Rejection = entity.Rejection;
            #endregion

            #region [ Base ]
            entityTransformed.OChartItem = entity.OChartItem;
            entityTransformed.Container = entity.Container;
            #endregion


            return entityTransformed;
        }

    }
}
