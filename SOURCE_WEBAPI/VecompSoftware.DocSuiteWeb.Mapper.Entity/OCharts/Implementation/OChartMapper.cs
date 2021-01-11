using VecompSoftware.DocSuiteWeb.Entity.OCharts;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.OCharts
{
    public class OChartMapper : BaseEntityMapper<OChart, OChart>, IOChartMapper
    {
        public override OChart Map(OChart entity, OChart entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Enabled = entity.Enabled;
            entityTransformed.StartDate = entity.StartDate;
            entityTransformed.EndDate = entity.EndDate;
            entityTransformed.Imported = entity.Imported;
            entityTransformed.Description = entity.Description;
            entityTransformed.Title = entity.Title;
            #endregion

            #region [ Navigation Properties ]

            #endregion

            return entityTransformed;

        }

    }
}
