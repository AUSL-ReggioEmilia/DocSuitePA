using VecompSoftware.DocSuiteWeb.Entity.OCharts;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.OCharts
{
    public class OChartItemContainerMapper : BaseEntityMapper<OChartItemContainer, OChartItemContainer>, IOChartItemContainerMapper
    {
        public override OChartItemContainer Map(OChartItemContainer entity, OChartItemContainer entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Master = entity.Master;
            entityTransformed.Rejection = entity.Rejection;
            #endregion

            return entityTransformed;
        }

    }
}
