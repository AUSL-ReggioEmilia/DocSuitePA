using VecompSoftware.DocSuiteWeb.Entity.OCharts;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.OCharts
{
    public class OChartItemMapper : BaseEntityMapper<OChartItem, OChartItem>, IOChartItemMapper
    {
        public override OChartItem Map(OChartItem entity, OChartItem entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Enabled = entity.Enabled;
            entityTransformed.Acronym = entity.Acronym;
            entityTransformed.Code = entity.Code;
            entityTransformed.FullCode = entity.FullCode;
            entityTransformed.Imported = entity.Imported;
            entityTransformed.Email = entity.Email;
            entityTransformed.Description = entity.Description;
            entityTransformed.Title = entity.Title;
            #endregion

            return entityTransformed;
        }

    }
}
